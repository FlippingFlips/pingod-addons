using Godot;
using PinGod.Base;
using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace PinGod.Core.Service
{

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
        protected int _offsetLamps;
        protected int _offsetLeds;
        protected int _offsetSwitches;        
        protected MemoryMappedViewAccessor _gameStateAccess;
        private MemoryMappedViewAccessor _switchMapping;
        private MemoryMappedViewAccessor _switchWriteMapping;
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
        protected MemoryMappedViewAccessor viewAccessor;

        public delegate void MemorySwitchHandler(object sender, SwitchEventArgs sw);
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
            TOTAL_COIL = coilCount * 2;
            TOTAL_LAMP = lampCount * 2;
            TOTAL_LED = ledCount * 3;
            TOTAL_SWITCH = switchCount * 2;
            switchBuffer = new byte[TOTAL_SWITCH];
            this.writeStates = writeStates;
            this.readStates = readStates;
            VpCommandSwitch = vpCommandSwitch;

            int offset = 1;
            //get offset position after coils and initial offset
            _offsetLamps = offset + TOTAL_COIL;
            _offsetLeds = _offsetLamps + TOTAL_LAMP;
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
                Logger.Info("memory map already initialized!");
                return;
            }

            tokenSource = new CancellationTokenSource();
            //write states to memory
            if (writeStates > -1)
            {
                _writeStatesTask = Task.Run(async () =>
                {
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
                _gameStateAccess.Write(0, (byte)1);

                _readStatesTask = Task.Run(async () =>
                {
                    Logger.Info(nameof(MemoryMap), ":running Read States Task");
                    while (!tokenSource.IsCancellationRequested)
                    {
                        ReadStates();
                        await Task.Delay(readStates);
                    }

                    //write the memory game state zero
                    _gameStateAccess.Write(0, (byte)0);

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
        protected virtual void ReadStates()
        {
            //get switches from memory
            //not the same as in the buffer check which switches have changed and push them onto handlers
            byte[] buffer = new byte[TOTAL_SWITCH];
            _switchMapping.ReadArray(0, buffer, 0, buffer.Length);            
            if (switchBuffer != buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    //last state in buffer changed
                    if (buffer[i] != switchBuffer[i])
                    {
                        //event to anyone listening for incoming memory maps
                        MemorySwitchEventHandler?.Invoke(this, new SwitchEventArgs(i, buffer[i]));
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
                    ev.Action = syncState.ToString();
                    break;
                case GameSyncState.pause: //pause / resume on a toggle, not held down
                case GameSyncState.resume:
                case GameSyncState.reset:
                    ev.Action = syncState.ToString();
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
                _gameStateAccess = mmf.CreateViewAccessor(0,1,MemoryMappedFileAccess.ReadWrite);
                _switchMapping = mmf.CreateViewAccessor(_offsetSwitches, TOTAL_SWITCH * 2, MemoryMappedFileAccess.Read);
                _switchWriteMapping = mmf.CreateViewAccessor(_offsetSwitches, TOTAL_SWITCH * 2, MemoryMappedFileAccess.Write);
                //GD.Print("offset for switches: ", _offsetSwitches);
            }
            else
            {
                Logger.Info("mem_map: read/write states disabled");
            }
        }

        /// <summary>
        /// Writes coils, lamps and leds to the mapping
        /// </summary>
        public virtual void WriteStates()
        {
            //var start = OS.GetTicksMsec();        
            //GD.Print("write states");

            //get game machine states
            var coilBytes = Machine.Coils.GetStatesArray(TOTAL_COIL);
            var lampsBytes = Machine.Lamps.GetStatesArray(TOTAL_LAMP);
            var ledArray = Machine.Leds.GetLedStatesArray(TOTAL_LED);

            //write states. Add offset of 1. Game State on Zero
            viewAccessor.WriteArray(1, coilBytes, 0, coilBytes.Length);
            viewAccessor.WriteArray(_offsetLamps, lampsBytes, 0, lampsBytes.Length);
            viewAccessor.WriteArray(_offsetLeds, ledArray, 0, ledArray.Length);

            //Print("states written in:", OS.GetTicksMsec() - start);
        }

        /// <summary>
        /// Pushes a switch into the memory, on and off, off and on <para/>
        /// This used for extra control, multiple windows. While a game is running this can be used to send switches to the game
        /// </summary>
        /// <param name="swNum"></param>
        /// <param name="swValue"></param>
        internal void WriteSwitchPulse(int swNum, byte swValue)
        {
            byte[] arr = new byte[TOTAL_SWITCH / 2];
            arr[swNum] = (byte)swNum; arr[swNum + 1] = swValue;
            arr[swNum += 2] = (byte)swNum; arr[swNum += 3] = (byte)(swValue > 0 ? 1 : 0);
            //write the switches to memory map.
            _switchWriteMapping.WriteArray(0, arr, 0, arr.Length);
        }

        internal void WriteSwitch(int swNum, byte swValue)
        {
            byte[] arr = new byte[TOTAL_SWITCH / 2];
            arr[swNum - 1] = (byte)(swNum - 1); arr[swNum] = swValue;
            //write the switches to memory map.
            _switchWriteMapping.WriteArray(0, arr, 0, arr.Length);
        }

        public enum GameSyncState
        {
            /// <summary>
            /// No state
            /// </summary>
            None,
            /// <summary>
            /// Game started let . GameRunning
            /// </summary>
            started,
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
            resume,
            /// <summary>
            /// resume godot
            /// </summary>
            reset
        }
    }
}