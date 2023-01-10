using Godot;
using PinGod.Base;

namespace PinGod.Core.Service
{
    /// <summary>
    /// The scene script to manage the mapping
    /// </summary>
    public partial class MemoryMapNode : Node
    {
        static MemoryMap mMap;

        const string ROOT_DIR = "addons/pingod-addons/";

        /// <summary>
        /// Emitted when a switch comes into the game. From <see cref="MemoryMap.ReadStates"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        [Godot.Signal] public delegate void MemorySwitchSignalEventHandler(string name, int index, byte value);

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
                    Logger.Warning(nameof(MemoryMapNode), ": PinGod-Memory addon disabled. Use IsEnabled in the scene.\n**Duplicate the default MemoryMap.tscn and put in autoload directory, change settings in scene and re-enable the plugin.**");
                    this.QueueFree();
                    return;
                }

                if (this.WriteDelay < 0 && this.ReadDelay < 0)
                {
                    Logger.Warning(nameof(MemoryMapNode), "removing PinGod-Memory addon. enable the read delay and write delay with values higher than 1");
                    this.QueueFree();
                    return;
                }

                if (mMap == null)
                {
                    //todo vp command switch
                    mMap = new MemoryMap(this.MutexName, MapName, WriteDelay, ReadDelay, CoilTotal, LampTotal, LedTotal, SwitchTotal);

                    Logger.Debug(nameof(MemoryMapNode), $@":MappingFile Created. mutex:{MutexName}, map:{MapName}");
                    Logger.Debug(nameof(MemoryMapNode), $@": Read:{WriteDelay},write:{WriteDelay}. showing count-total bytes");
                    Logger.Debug(nameof(MemoryMapNode), $@": coils:{CoilTotal}-{mMap.TOTAL_COIL},sw:{SwitchTotal}-{mMap.TOTAL_SWITCH},lamps:{LampTotal}-{mMap.TOTAL_LAMP},led:{LedTotal}-{mMap.TOTAL_LED}");
                    //Logger.Debug(nameof(PinGodMemoryMapNode), $":offsets:coils:0,lamps:{mMap.},leds:{_offsetLeds},switches:{_offsetSwitches}");
                }
                else
                {
                    //adding another one...
                    Logger.Warning(nameof(MemoryMapNode), "$:WARN:MemoryMapScript already added");
                    this.QueueFree();
                    return;
                }

                if (mMap != null)
                {
                    //got this far so we can start memory mapping            
                    Logger.Info(nameof(MemoryMapNode), ":memory map loaded, starting read/write state tasks.");
                    Logger.Info(nameof(MemoryMapNode), nameof(_Ready), ": setup event handling from switches");
                    mMap.MemorySwitchEventHandler += MMap_MemorySwitchEventHandler;
                }
                Start();
            }
            else
            {
                Logger.Info(nameof(MemoryMapNode), ":script in editor, doing nothing");
            }
        }

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            Logger.Info(nameof(MemoryMapNode), ":memory map exiting");
            Stop();
            Logger.Info(nameof(MemoryMapNode), ":memory map exited");
        }

        public void WriteSwitchPulseToMemory(int swNum, byte swValue)
        {
            //set the switch on/off
            var sw = Machine.Switches.GetSwitch(swNum);
            sw.SetSwitch(swValue);

            //write to memory
            mMap.WriteSwitchPulse(swNum, swValue);
        }

        public void WriteSwitchToMemory(int swNum, byte swValue)
        {
            //set the switch on/off
            var sw = Machine.Switches.GetSwitch(swNum);
            sw.SetSwitch(swValue);

            //write to memory
            mMap.WriteSwitch(swNum, swValue);
        }

        private void MMap_MemorySwitchEventHandler(object sender, SwitchEventArgs sw)
        {
            Logger.Verbose(nameof(MemoryMapNode), $": map switch: {sw.Num}={sw.Value}");
            EmitSignal(nameof(MemorySwitchSignal), new Variant[] { string.Empty, sw.Num, sw.Value });
        }

        void Start()
        {
            if (mMap != null)
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
}
