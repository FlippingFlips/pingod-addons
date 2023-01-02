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
    [Godot.Signal] public delegate void SwitchCommandEventHandler(string name, byte index, byte value);

    /// <summary>
    /// Convert the given totals with added state to 
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
            }
            else
            {
                Logger.Warning(nameof(PinGodMemoryMapNode), "$:WARN:MemoryMapScript already added");
                this.QueueFree();
                return;
            }
            

            //TODO: print memory offsets

            //set totals. 2 for on/off, 3 with added color
            //TOTAL_COIL = CoilTotal * 2;
            //TOTAL_LAMP = LampTotal * 2;
            //TOTAL_LED = LedTotal * 3;
            //TOTAL_SWITCH = SwitchTotal * 2;
            ////get offset positions to access in memory, other map in controller does the same
            //_offsetLamps = TOTAL_COIL;
            //_offsetLeds = _offsetLamps + TOTAL_LAMP;
            //_offsetSwitches = _offsetLeds + (TOTAL_LED * sizeof(int));

            //Logger.Debug(nameof(MemoryMap), $":coils:{CoilTotal},lamps:{LampTotal},leds:{LedTotal},switches:{SwitchTotal}");
            //Logger.Debug(nameof(MemoryMap), $":offsets:coils:0,lamps:{_offsetLamps},leds:{_offsetLeds},switches:{_offsetSwitches}");

            //SetUp();
            GD.Print(nameof(PinGodMemoryMapNode), "memory map script enter");
            mMap.MemorySwitchEventHandler += MMap_MemorySwitchEventHandler;
            Start();
        }
        else
        {
            GD.Print(nameof(PinGodMemoryMapNode), ":script in editor, doing nothing");
        }
    }

    private void MMap_MemorySwitchEventHandler(object sender, System.EventArgs e)
    {
        
    }

    public void Start()
    {
        if(mMap != null)
        {
            mMap.Start();
        }
    }

    public void Stop()
    {
        if (mMap != null)
        {
            mMap.Stop();
        }
    }
}
