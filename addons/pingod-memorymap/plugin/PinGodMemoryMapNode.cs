using BasicGameGodot.addons.pingod_memorymap;
using Godot;
using static MemoryMap;

/// <summary>
/// The scene script to manage the mapping
/// </summary>
public partial class PinGodMemoryMapNode : Node
{
    static MemoryMap mMap;

    const string ROOT_DIR = "addons/pingod-addons/";

    /// <summary>
    /// Emitted when a switch comes into the game. From <see cref="MemoryMap.ReadStates"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    [Godot.Signal] public delegate void SwitchCommandEventHandler(string name, int index, byte value);

    /// <summary>
    /// MemoryMapNode loaded. Needs to be enabled in options. Removes if isn't enabled and write/read delay isn't high enough.<para/>
    /// Creates a <see cref="MemoryMap"/> for other application to access memory (windows) <para/>
    /// 
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
            if (!this.IsEnabled)
            {
                Logger.Warning("removing PinGo-Memory addon. Node IsEnabled=false");
                this.QueueFree();
                return;
            }

            if (this.WriteDelay < 0 && this.ReadDelay < 0)
            {
                Logger.Warning("removing PinGo-Memory addon. enable the read delay and write delay with values higher than 1");
                this.QueueFree();
                return;
            }

            if (mMap == null)
            {
                //todo vp command switch
                mMap = new MemoryMap(this.MutexName, MapName, WriteDelay, ReadDelay, CoilTotal, LampTotal, LedTotal, SwitchTotal);

                Logger.Debug(nameof(PinGodMemoryMapNode), $@":MappingFile Created. mutex:{MutexName}, map:{MapName}");
                Logger.Debug(nameof(PinGodMemoryMapNode), $@": Read:{WriteDelay},write:{WriteDelay}. showing count-total bytes");
                Logger.Debug(nameof(PinGodMemoryMapNode), $@": coils:{CoilTotal}-{mMap.TOTAL_COIL},sw:{SwitchTotal}-{mMap.TOTAL_SWITCH},lamps:{LampTotal}-{mMap.TOTAL_LAMP},led:{LedTotal}-{mMap.TOTAL_LED}");
                //Logger.Debug(nameof(PinGodMemoryMapNode), $":offsets:coils:0,lamps:{mMap.},leds:{_offsetLeds},switches:{_offsetSwitches}");
            }
            else
            {
                //adding another one...
                Logger.Warning(nameof(PinGodMemoryMapNode), "$:WARN:MemoryMapScript already added");
                this.QueueFree();
                return;
            }

            //got this far so we can start memory mapping            
            Logger.Info(nameof(PinGodMemoryMapNode), ":memory map loaded, starting read/write state tasks.");
            mMap.MemorySwitchEventHandler += MMap_MemorySwitchEventHandler;
            Start();
        }
        else
        {
            GD.Print(nameof(PinGodMemoryMapNode), ":script in editor, doing nothing");
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        Logger.Info(nameof(PinGodMemoryMapNode), ":memory map exiting");
        Stop();
        Logger.Info(nameof(PinGodMemoryMapNode), ":memory map exited");
    }

    private void MMap_MemorySwitchEventHandler(object sender, SwitchEventArgs sw)
    {
        Logger.Debug(nameof(PinGodMemoryMapNode), sender.ToString());
        EmitSignal(nameof(SwitchCommand), new Variant[] { string.Empty, sw.Num, sw.Value});
    }

    void Start()
    {
        if(mMap != null)
        {
            mMap.Start();
        }
    }

    void Stop()
    {
        if (mMap != null)
        {            
            mMap.Stop();
            mMap.Dispose();
        }
    }
}
