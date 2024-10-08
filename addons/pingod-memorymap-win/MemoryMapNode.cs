using Godot;
using PinGod.Base;
using PinGod.Game;
using System.Reflection.Metadata.Ecma335;

namespace PinGod.Core.Service
{
	/// <summary>
	/// The scene script to manage the mapping
	/// </summary>
	public partial class MemoryMapNode : Node
	{
		protected static MemoryMap mMap;
		/// <summary>
		/// Emitted when a switch comes into the game. From <see cref="MemoryMap.ReadStates"/>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		[Godot.Signal] public delegate void MemorySwitchSignalEventHandler(string name, int index, byte value);

		/// <summary> Switch number to use for sending a custom value to </summary>
		[Export] byte _vpSwitchCommand { get; set; }

		public override void _ExitTree()
		{
			base._ExitTree();

			Logger.Info(nameof(MemoryMapNode), ":memory map exiting");
			Stop();
			Logger.Info(nameof(MemoryMapNode), ":memory map exited");
		}

        /// <summary>
        /// MemoryMapNode loaded. Needs to be enabled in options. Removes if isn't enabled and write/read delay isn't high enough.<para/>
        /// Creates a <see cref="MemoryMap"/> for other application to access memory (windows) <para/>
        /// </summary>
        public override void _Ready()
		{
			base._Ready();

            if (!Engine.IsEditorHint())
            {
                if (!PinGodGame.PinGodOverrideConfig.MemoryMapEnabled)
                {
                    Logger.WarningRich(nameof(MemoryMapNode), ":[color-yellow] PinGod-Memory addon disabled. Use IsEnabled in the scene.\n**Duplicate the default MemoryMap.tscn and put in autoload directory, change settings in scene and re-enable the plugin.** [/color]");
                    this.QueueFree();
                    return;
                }

                if (PinGodGame.PinGodOverrideConfig.MemoryMapWriteDelay < 0 && PinGodGame.PinGodOverrideConfig.MemoryMapReadDelay < 0)
                {
                    Logger.WarningRich(nameof(MemoryMapNode), ":[color-yellow]removing PinGod-Memory addon. enable the read delay and write delay with values higher than 1[/color]");
                    this.QueueFree();
                    return;
                }
            }
            else
            {
                Logger.Info(nameof(MemoryMapNode), ":script in editor, doing nothing");
            }

            if (mMap == null)
			{
				//todo vp command switch
				CreateMemoryMap();

				if(mMap != null)
				{
					Logger.Debug(nameof(MemoryMapNode), $@": MappingFile Created. mutex:{MutexName}, map:{MapName}");
					Logger.Debug(nameof(MemoryMapNode), $@": Read:{PinGodGame.PinGodOverrideConfig.MemoryMapReadDelay},write:{PinGodGame.PinGodOverrideConfig.MemoryMapWriteDelay}. showing count-total bytes");
					Logger.Debug(nameof(MemoryMapNode), $@": coils:{CoilTotal}-{mMap.TOTAL_COIL},sw:{SwitchTotal}-{mMap.TOTAL_SWITCH},lamps:{LampTotal}-{mMap.TOTAL_LAMP},led:{LedTotal}-{mMap.TOTAL_LED}");
					//Logger.Debug(nameof(PinGodMemoryMapNode), $":offsets:coils:0,lamps:{mMap.},leds:{_offsetLeds},switches:{_offsetSwitches}");
				}
				else
				{
					Logger.Warning(nameof(MemoryMapNode), "$:WARN:No Memory Map created");
					this.QueueFree();
				}
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

            if (HasNode("/root/Resources"))
            {
                var _resources = GetNode("/root/Resources") as Resources;
                _resources.ResourcesLoaded += _resources_ResourcesLoaded; ;
            }
			else
			{
                Start();
            }
		}

        private void _resources_ResourcesLoaded(int amt)
        {
			Start();
        }

        /// <summary>
        /// Creates the <see cref="mMap"/>, <see cref="MemoryMap"/> <para/>
        /// Override to create your own here.
        /// </summary>
        public virtual void CreateMemoryMap()
		{
			mMap = new MemoryMap(this.MutexName, MapName,
                PinGodGame.PinGodOverrideConfig.MemoryMapWriteDelay,
                PinGodGame.PinGodOverrideConfig.MemoryMapReadDelay,
				CoilTotal, LampTotal, LedTotal, SwitchTotal, _vpSwitchCommand);

			//add a ref to the mapper pointing to this node
			mMap._mMapNodeRef = this;
        }

		public int CoilCount() => mMap.TOTAL_COIL;
		internal int LampCount() => mMap.TOTAL_LAMP;
		internal int LedCount() => mMap.TOTAL_LED;
		internal int SwitchCount() => mMap.TOTAL_SWITCH;

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

            //Godot 3
            //EmitSignal(nameof(MemorySwitchSignal), new Variant[] { string.Empty, sw.Num, sw.Value });			            
            //Godot 4
            CallDeferred("emit_signal", new Variant[] { nameof(MemorySwitchSignal), string.Empty, sw.Num, sw.Value });

        }

		/// <summary>
		/// Starts the memory map tasks
		/// </summary>
		public virtual void Start() => mMap?.Start();

		public virtual void ProcessGameState(int syncState)
		{
            var ev = new InputEventAction() { Action = "", Pressed = true };
            switch ((GameSyncState)syncState)
            {
                case GameSyncState.quit:
                    ev.Action = "quit";
                    var game = GetNodeOrNull("/root/WindowActions") as WindowActionsNode;
					game?.Quit();
                    break;
                case GameSyncState.pause: //pause / resume on a toggle, not held down
                case GameSyncState.resume:
                case GameSyncState.reset:
                    ev.Action = syncState.ToString();
                    ev.Pressed = true;
                    break;
                case GameSyncState.None:
                case GameSyncState.started:
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(ev.Action))
            {
                //Input.ParseInputEvent(ev);


				Logger.Verbose(nameof(MemoryMapNode), $": processed game state {ev.Action}");
            }
        }

		/// <summary>
		/// stops tasks and disposes of memory mapping
		/// </summary>
		void Stop()
		{
			mMap?.Stop();
			mMap?.Dispose();
		}
	}
}
