using Godot;
using Godot.Collections;
using System.Linq;

/// <summary>
/// Machine Godot Node. Holds all machine items before runtime. Adds to the static collections in <see cref="Machine"/>
/// </summary>
public partial class MachineConfig : Node
{
    /// <summary>
    /// Coil names to pulse when ball searching
    /// </summary>
    [Export] public string[] _ball_search_coils;

    /// <summary>
    /// 
    /// </summary>
    [Export] public bool _ball_search_enabled = true;

    /// <summary>
    /// Switches that stop the ball searching
    /// </summary>
    [Export] public string[] _ball_search_stop_switches;

    /// <summary>
    /// Must reflect VP controller counts
    /// </summary>
    [Export] public byte _memCoilCount = 32;
	/// <summary>
	/// Must reflect VP controller counts
	/// </summary>
	[Export] public byte _memLampCount = 64;
	/// <summary>
	/// Must reflect VP controller counts
	/// </summary>
	[Export] public byte _memLedCount = 64;
	/// <summary>
	/// Must reflect VP controller counts
	/// </summary>
	[Export] public byte _memSwitchCount = 64;

    static bool _instanceLoaded = false;
    /// <summary>
    /// How long to wait for ball searching and reset
    /// </summary>
    [Export] private int _ball_search_wait_time_secs = 10;

    [Export] Dictionary<string, byte> _coils = new Dictionary<string, byte>();
    [Export] Dictionary<string, byte> _lamps = new Dictionary<string, byte>();
    [Export] Dictionary<string, byte> _leds = new Dictionary<string, byte>();
    [Export] Dictionary<string, byte> _switches = new Dictionary<string, byte>();
    /// <summary>
    /// Emitted when a switch comes into the game. From <see cref="PinGodMemoryMapNode.ReadStates"/> then <see cref="PinGodGame.SetSwitch(int, byte)"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    [Signal] public delegate void SwitchCommandEventHandler(string name, byte index, byte value);
    /// <summary>
    /// 
    /// </summary>
    public BallSearchOptions BallSearchOptions { get; private set; }
    /// <summary>
    /// <see cref="AddCustomMachineItems"/>
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
			if (_instanceLoaded)
			{
                Logger.Warning(nameof(MachineConfig), $":_EnterTree multiple {nameof(MachineConfig)} found, removing..");
				this.QueueFree();
				return;
            }				
			//ball search options
			BallSearchOptions = new BallSearchOptions(_ball_search_coils, _ball_search_stop_switches, _ball_search_wait_time_secs, _ball_search_enabled);

			AddCustomMachineItems(_coils, _switches, _lamps, _leds);

			_instanceLoaded = true;

		}		
	}

    /// <summary>
    /// Use in Godot _Input events. Checks a switches input event by friendly name from switch collection <para/>
    /// "coin", @event
    /// </summary>
    /// <param name="swName"></param>
    /// <param name="inputEvent"></param>
    /// <returns></returns>
    public virtual bool SwitchActionOn(string swName, InputEvent inputEvent)
    {        
        var sw = Machine.Switches[swName];
        var result = sw.IsActionOn(inputEvent);
        if (result)
        {
            SetSwitch(sw, 1);
        }
        return result;
    }

    /// <summary>
    /// Checks a switches input event by friendly name that is in the <see cref="Switches"/> <para/>
    /// "coin", @event
    /// </summary>
    /// <param name="swName"></param>
    /// <param name="inputEvent"></param>
    /// <returns></returns>
    public virtual bool SwitchActionOff(string swName, InputEvent inputEvent)
    {
        var sw = Machine.Switches[swName];
        var result = sw.IsActionOff(inputEvent);
        if (result)
        {
            SetSwitch(sw, 0);
        }
        return result;
    }

    /// <summary>
    /// Set IsEnabled on the Switch and emits <see cref="PinGodBase.SwitchCommand"/> with number and byte value <para/>
    /// Switch will be set to 0 or 1 but the signal value can be 0-255 <para/>
    /// Anything listening to the <see cref="PinGodBase.SwitchCommand"/> can get the Switch from <see cref="Machine.Switches"/> without checking keys, it will be valid sent from here
    /// </summary>
    /// <param name="swNum"></param>
    /// <param name="value"></param>
    public void SetSwitch(int swNum, byte value)
    {
        var sw = Machine.Switches.Values.FirstOrDefault(x => x.Num == swNum);
        if (sw != null)
        {
            SetSwitch(sw, value, false);
        }
    }

    /// <summary>
    /// Sets switch IsEnabled when not coming from a Godot action. Sets switch and Emits <see cref="SwitchCommandEventHandler"/>.
    /// </summary>
    /// <param name="switch"></param>
    /// <param name="value"></param>
    /// <param name="fromAction">if false it doesn't set switch, actions should do this when checked here in SwitchOn</param>
    public void SetSwitch(Switch @switch, byte value, bool fromAction = true)
    {
        if (!fromAction)
            @switch.SetSwitch(value > 0);

        //ProcessSwitch(@switch);
        EmitSignal(nameof(SwitchCommand), @switch.Name, @switch.Num, value);
    }

    /// <summary>
    /// Adds custom machine items. Actions are created for switches if they do not exist
    /// </summary>
    /// <param name="coils"></param>
    /// <param name="switches"></param>
    /// <param name="lamps"></param>
    /// <param name="leds"></param>
    protected void AddCustomMachineItems(Dictionary<string, byte> coils, Dictionary<string, byte> switches, Dictionary<string, byte> lamps, Dictionary<string, byte> leds)
	{
		foreach (var coil in coils.Keys)
		{
			Machine.Coils.Add(coil, new PinStateObject(coils[coil]));
		}
		var itemAddResult = string.Join(", ", coils.Keys);
		Logger.Debug(nameof(MachineConfig), $":added coils {itemAddResult}");
		coils.Clear();

		foreach (var sw in switches.Keys)
		{
			//create an action for the switch if it doesn't exist.
			var swVal = switches[sw];
			if (!Godot.InputMap.HasAction("sw" + swVal))
			{
				Godot.InputMap.AddAction("sw" + swVal);
			}

			if (BallSearchOptions.StopSearchSwitches?.Any(x => x == sw) ?? false)
			{
				Machine.Switches.Add(sw, new Switch(sw, swVal, BallSearchSignalOption.Off));
			}
			else
			{
				Machine.Switches.Add(sw, new Switch(sw, swVal, BallSearchSignalOption.Reset));
			}
		}

		itemAddResult = string.Join(", ", switches.Keys);
		//LogDebug($"pingod: added switches {itemAddResult}");
		switches.Clear();

		foreach (var lamp in lamps.Keys)
		{
			Machine.Lamps.Add(lamp, new PinStateObject(lamps[lamp]));
		}
		//itemAddResult = string.Join(", ", lamps.Keys);
		//LogDebug($"pingod: added lamps {itemAddResult}");
		lamps.Clear();

		foreach (var led in leds.Keys)
		{
			Machine.Leds.Add(led, new PinStateObject(leds[led]));
		}
		//LogDebug($"pingod: added leds {string.Join(", ", leds.Keys)}");
		leds.Clear();

        Logger.Debug(nameof(MachineConfig), $":Custom items loaded...");
		Logger.Debug(nameof(MachineConfig), $":switches={Machine.Switches.Count}:coils={Machine.Coils.Count}:lamps={Machine.Lamps.Count},:leds={Machine.Leds.Count}");
    }
}