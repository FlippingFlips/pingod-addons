﻿using BasicGameGodot.addons.pingod_memorymap;
using Godot;
using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Memory Mapping file (windows) which the COM controller also has access to. <para/>
/// Saves events, states of switches, coils, lamps + leds <para/>
/// Mutex=pingod_vp_mutex | MapName=pingod_vp
/// </summary>
public class MemoryMap : IDisposable
{
    const int MAP_SIZE = 2048;
    internal static System.Threading.Mutex mutex;    
    private readonly int readStates;
    private readonly int writeStates;
    private int _offsetLamps;
    private int _offsetLeds;
    private int _offsetSwitches;
    private MemoryMappedViewAccessor _switchMapping;
    private Task _readStatesTask;
    private Task _writeStatesTask;
    private MemoryMappedFile mmf;
    bool mutexCreated;
    byte[] switchBuffer;
    private CancellationTokenSource tokenSource;
    public readonly int TOTAL_COIL;
    public readonly int TOTAL_LAMP;
    public readonly int TOTAL_LED; //Led 3 = Num, State, Color (ole)
    public readonly int TOTAL_SWITCH;
    private MemoryMappedViewAccessor viewAccessor;

    public delegate void MemorySwitchHandler (object sender, SwitchEventArgs sw);
    public event MemorySwitchHandler MemorySwitchEventHandler;  

    /// <summary>
    /// Sets up memory mapping and offsets. Counts for machine items are added to get memory address offsets <para/>
    /// Coil 0 memory map will be using 0,1, Coil 1: 2,3
    /// </summary>
    /// <param name="coilCount">coil count will be * 2</param>
    /// <param name="lampCount">lamp count will be * 2</param>
    /// <param name="ledCount">led count will be * 3</param>
    /// <param name="switchCount">switch count will be * 2</param>
    /// <param name="writeStates">delay write states. -1 is off</param>
    /// <param name="readStates">read states to memory. -1 is off</param>
    public MemoryMap(string mutexName = "pingod_vp_mutex", string mapName = "pingod_vp",
        int writeStates = -1, int readStates = -1,
        byte coilCount = 32, byte lampCount = 64, byte ledCount = 64, byte switchCount = 64,
        byte? vpCommandSwitch = null)
    {
        MutexName = mutexName;
        MapName = mapName;
        this.TOTAL_COIL = coilCount * 2;
        this.TOTAL_LAMP = lampCount * 2;
        this.TOTAL_LED = ledCount * 3;
        this.TOTAL_SWITCH = switchCount * 2;
        switchBuffer = new byte[TOTAL_SWITCH];
        this.writeStates = writeStates;
        this.readStates = readStates;
        VpCommandSwitch = vpCommandSwitch;
        _offsetLamps = TOTAL_COIL;
        _offsetLeds = TOTAL_COIL + TOTAL_LAMP;
        //normal count to _offseLeds then 
        _offsetSwitches = _offsetLeds + (TOTAL_LED * sizeof(int));

        CreateMutexAndMapping();        
    }

    public string MutexName { get; }
    public string MapName { get; }
    public byte? VpCommandSwitch { get; }

    /// <summary>
    /// Dispose and releases the mutex and stops the read/write states thread <see cref="tokenSource"/>
    /// </summary>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// Starts the <see cref="_writeStatesTask"/>.
    /// </summary>
    /// <param name="writeDelay"></param>
    public void Start()
    {
        if (_writeStatesTask != null || _readStatesTask != null)
        {
            GD.Print("memory map already initialized!");
            return;
        }
        
        tokenSource = new CancellationTokenSource();
        //write states to memory
        if (writeStates > -1)
        {
            _writeStatesTask = Task.Run(async () =>
            {
                Logger.Info(nameof(MemoryMap), ":running Write States Task");
                while (!tokenSource.IsCancellationRequested)
                {
                    WriteStates();
                    await Task.Delay(writeStates);
                }

                //await Task.Delay(1000);
                Logger.Info(nameof(MemoryMap), ":writes states stopped");
            }, tokenSource.Token);
        }
        //read states from memory
        if (readStates > -1)
        {
            _readStatesTask = Task.Run(async () =>
            {
                Logger.Info(nameof(MemoryMap), ":running Read States Task");
                while (!tokenSource.IsCancellationRequested)
                {
                    ReadStates();
                    await Task.Delay(readStates);
                }

                //await Task.Delay(1000);
                Logger.Info(nameof(MemoryMap), ":read states stopped");
            }, tokenSource.Token);
        }
    }

    /// <summary>
    /// Stops the <see cref="_writeStatesTask"/> thread
    /// </summary>
    public void Stop()
    {
        if (!tokenSource.IsCancellationRequested)
        {
            tokenSource?.Cancel();
        }
    }

    /// <summary>
    /// Disposes of the memory map and mutex
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {            
            mmf?.Dispose();
            if (mutexCreated)
            {
                try { mutex?.ReleaseMutex(); } catch { }
            }            
        }
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Reads the buffer size <see cref="TOTAL_SWITCH"/> switch states from the memory map buffer at position 0. <para/>
    /// Acts on any new switch events found. <para/>
    /// The switch state gets converted to a Godot InputEventAction with the name sw{Num} and fed into the game<para/>
    /// It is overidden if a VpCommand found. <para/>
    /// Switch 0 changed will process game states, switch zero used with <see cref="GameSyncState"/>.
    /// </summary>
    private void ReadStates()
    {
        byte[] buffer = new byte[TOTAL_SWITCH];
        _switchMapping.ReadArray(0, buffer, 0, buffer.Length);

        if (switchBuffer != buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                //last state in buffer changed
                if (buffer[i] != switchBuffer[i])
                {                   
                    //override sending switch if this is a visual pinball command
                    if(VpCommandSwitch.HasValue && VpCommandSwitch.Value > 0 && VpCommandSwitch.Value == i)
                    {
                        //TODO: emit vpswitch, not using godot
                        //pinGodGame?.EmitSignal(nameof(PinGodBase.VpCommandEventHandler), nameof(PinGodBase.VpCommandEventHandler), buffer[i]);
                    }
                    else if (i > 0)
                    {
                        Logger.Info("read states run. switch changed");
                        //set switch IsEnabled and send signal
                        //Logger.Verbose(nameof(MemoryMap), $":sw:{i}:{buffer[i]}");

                        //TODO
                        //pinGodGame.SetSwitch(i, buffer[i]);

                        MemorySwitchEventHandler?.Invoke(this, new SwitchEventArgs(i, buffer[i]));

                        //Feed switch into Godot action
                        //bool actionState = (bool)GD.Convert(buffer[i], Variant.Type.Bool);
                        //var ev = new InputEventAction() { Action = $"sw{i}", Pressed = actionState };
                        //Input.ParseInputEvent(ev);
                    }
                    else // Use Switch 0 for game GameSyncState
                    {                        
                        var syncState = (GameSyncState)buffer[i];
                        var action = ProcessGameState(syncState);
                        Logger.Debug(nameof(MemoryMap), ": processed GameSyncState action=", action);
                    }
                }
            }
        }

        switchBuffer = buffer;
    }

    /// <summary>
    /// Process a gameSyncState into a input event, then feed into Godot input
    /// </summary>
    /// <param name="syncState"></param>
    /// <returns></returns>
    private string ProcessGameState(GameSyncState syncState)
    {
        var ev = new InputEventAction() { Action = "", Pressed = true };
        switch (syncState)
        {
            case GameSyncState.quit:
                ev.Action = GameSyncState.quit.ToString();
                break;
            case GameSyncState.pause: //pause / resume on a toggle, not held down
            case GameSyncState.resume:
                ev.Action = GameSyncState.pause.ToString();
                ev.Pressed = true;
                break;
            case GameSyncState.None:
            default:
                break;
        }

        if (!string.IsNullOrWhiteSpace(ev.Action))
        {
            Input.ParseInputEvent(ev);
        }
        return ev.Action;
    }

    /// <summary>
    /// Creates a mutex and opens the memory map. Creates the <see cref="viewAccessor"/> and <see cref="_switchMapping"/>. 
    /// </summary>
    private void CreateMutexAndMapping()
    {
        if (writeStates > -1 || readStates > -1)
        {
            if (mutex != null) return; //already created

            mutexCreated = System.Threading.Mutex.TryOpenExisting(MutexName, out mutex);
            if (!mutexCreated)
            {
                //Logger.Debug(nameof(MemoryMap), ":couldn't find mutex:", MutexName, " creating new");
                mutex = new System.Threading.Mutex(true, MutexName, out mutexCreated);
            }
            else
            {
                //Logger.Debug(nameof(MemoryMap),":mutex found:", MutexName);
            }

            //todo: can't make this cross platform
            mmf = MemoryMappedFile.CreateOrOpen(MapName, MAP_SIZE);
            
            viewAccessor = mmf.CreateViewAccessor(0, MAP_SIZE, MemoryMappedFileAccess.ReadWrite);
            _switchMapping = mmf.CreateViewAccessor(_offsetSwitches, TOTAL_SWITCH * 2, MemoryMappedFileAccess.Read);
            //GD.Print("offset for switches: ", _offsetSwitches);
        }
        else
        {
            Logger.Info("mem_map: read/write states disabled");
        }
    }

    /// <summary>
    /// Writes coils, lamps and leds
    /// </summary>
    void WriteStates()
    {
        //var start = OS.GetTicksMsec();        
        //GD.Print("write states");

        //get game machine states
        var coilBytes = Machine.Coils.GetStatesArray(TOTAL_COIL);
        var lampsBytes = Machine.Lamps.GetStatesArray(TOTAL_LAMP);
        var ledArray = Machine.Leds.GetLedStatesArray(TOTAL_LED);

        //write states
        viewAccessor.WriteArray(0, coilBytes, 0, coilBytes.Length);
        viewAccessor.WriteArray(_offsetLamps, lampsBytes, 0, lampsBytes.Length);
        viewAccessor.WriteArray(_offsetLeds, ledArray, 0, ledArray.Length);

        //Print("states written in:", OS.GetTicksMsec() - start);
    }

    public enum GameSyncState
    {
        /// <summary>
        /// No state
        /// </summary>
        None,
        /// <summary>
        /// quit godot
        /// </summary>
        quit,
        /// <summary>
        /// pause godot
        /// </summary>
        pause,
        /// <summary>
        /// resume godot
        /// </summary>
        resume
    }
}