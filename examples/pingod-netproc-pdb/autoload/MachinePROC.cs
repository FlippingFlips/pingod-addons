using Godot;
using Godot.Collections;
using NetProc.Domain;
using NetProc.Domain.PinProc;
using PinGod.Base;
using PinGod.Core;
using PinGod.Core.Service;
using System.Diagnostics;

/// <summary>
/// This script is loaded with the `res://autoload/Machine.tscn` which inherits a PinGod-MachineNode. <para/>
/// Singleton can be retrieved from the /root/ with GetNode&lt;MachinePROC&gt;("/root/Machine"). <para/>
/// EnterTree initializes a database, gets machine config
/// </summary>
public partial class MachinePROC : MachineNode
{           
	private PinGodGameProc _pinGodGameProc;

	#region Godot overrides
	/// <summary> and machine configuration. Machine config is held public here
	/// Creates database
	/// </summary>
	public override void _EnterTree()
	{
		base._EnterTree();
	}

	/// <summary>
	/// Saves the database and Dispose of the connection
	/// </summary>
	public override void _ExitTree()
	{
		Logger.Info(nameof(MachinePROC), ":", nameof(_ExitTree));
		base._ExitTree();
	}

	/// <summary>
	/// Gets a PinGodGame
	/// </summary>
	public override void _Ready()
	{
		base._Ready();
		_pinGodGameProc = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
	}

	/// <summary>
	/// Process playback events. Turns off if playback is switched off.
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(double delta)
	{
		if (!_pinGodGameProc.GameReady) return;

		if (_recordPlayback != RecordPlaybackOption.Playback)
		{
			SetProcess(false);
			Logger.Info(nameof(MachineNode), ": Playback _Process loop stopped. No recordings are being played back.");
			return;
		}
		else
		{
			var cnt = _recordFile.GetQueueCount();
			if (cnt <= 0)
			{
				Logger.Info(nameof(MachineNode), ": playback events ended, RecordPlayback is off.");
				_recordPlayback = RecordPlaybackOption.Off;
				_recordFile.SaveRecording();
				if (_recordingStatusLabel != null) _recordingStatusLabel.Text = "Machine:Playback ended";
				return;
			}

			var evt = _recordFile.ProcessQueue(_machineLoadTime);
			if (evt != null)
			{
				var sw = _pinGodGameProc.PinGodProcGame.Switches[evt.EvtName];
				OnSwitchCommand(sw.Name, sw.Number, evt.State);
			}
		}
	}

	#endregion

	/// <summary>
	/// Clear any items from the machines collections 
	/// </summary>
	public void ClearMachineItems()
	{
		_coils.Clear();
		_lamps.Clear();
		_leds.Clear();
		_switches.Clear();
	}

	/// <summary>
	/// Name not used?
	/// </summary>
	/// <param name="name"></param>
	/// <param name="num"></param>
	/// <param name="value"></param>
	public override void OnSwitchCommand(string name, int num, byte value)
	{
		Logger.Info("MACHINE_PROC: Switch:" + num);
		//base.OnSwitchCommand(name, index, value);
		if (_pinGodGameProc != null && _pinGodGameProc.PinGodProcConfig.Simulated)
		{
			SetSwitchFakeProc(_pinGodGameProc.PinGodProcGame, (ushort)num, value > 0 ? true : false);
		}        
	}

	/// <summary>
	/// Calls base set switch but will set fake p-roc switch first
	/// </summary>
	/// <param name="switch"></param>
	/// <param name="value"></param>
	/// <param name="fromAction"></param>
	public override void SetSwitch(PinGod.Core.Switch @switch, byte value, bool fromAction = true)
	{
		if (_pinGodGameProc != null)
		{
			//var sw = _switches[@switch.Name];
			SetSwitchFakeProc(_pinGodGameProc.PinGodProcGame, @switch.Name, value > 0 ? true : false);
		}
		base.SetSwitch(@switch, value, fromAction);
	}

	public override void SetSwitch(string name, byte value, bool fromAction = true)
	{
		if(_pinGodGameProc != null)
		{
			SetSwitchFakeProc(_pinGodGameProc.PinGodProcGame, name, value > 0 ? true : false);
		}
	}

	/// <summary>
	/// override godot actions to the machine
	/// </summary>
	/// <param name="swName"></param>
	/// <param name="inputEvent"></param>
	/// <returns></returns>
	public override bool SwitchActionOn(string swName, InputEvent inputEvent) => false;
	public override bool SwitchActionOff(string swName, InputEvent inputEvent) => false;

	protected override void AddCustomMachineItems(Dictionary<string, byte> coils, Dictionary<string, byte> switches, Dictionary<string, byte> lamps, Dictionary<string, byte> leds)
	{
		//base.AddCustomMachineItems(coils, switches, lamps, leds);
		Logger.Info(nameof(MachinePROC), ": P-ROC overriding ", nameof(AddCustomMachineItems));
	}

	internal void AddCoil(string name, byte number) => _coils.Add(name, number);
	internal void AddLamp(string name, byte number) => _lamps.Add(name, number);
	internal void AddLed(string name, byte number) => _leds.Add(name, number);
	internal void AddSwitch(string name, byte number) => _switches.Add(name, number);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="gc"></param>
	/// <param name="name"></param>
	/// <param name="enabled"></param>
	internal void SetSwitchFakeProc(IGameController gc, string name, bool enabled)
	{
		if(_pinGodGameProc != null)
		{
			if (_pinGodGameProc.PinGodProcConfig.Simulated)
			{
				var proc = gc?.PROC as IFakeProcDevice;
				var sw = gc.Switches[name];
				var evtT = enabled ? EventType.SwitchClosedDebounced : EventType.SwitchOpenDebounced;
				proc.AddSwitchEvent(sw.Number, evtT);

				RecordSwitch(name, sw);
			}
		}		
	}

	/// <summary>
	/// Records a switch if the game is recording
	/// </summary>
	/// <param name="name"></param>
	/// <param name="sw"></param>
	private void RecordSwitch(string name, NetProc.Domain.Switch sw)
	{		
		if (_recordPlayback == RecordPlaybackOption.Record)
		{						
			byte state = sw.StateString() == "closed" ? (byte)0 : (byte)1;

			_recordFile.RecordSwitchEvent(name, state, _machineLoadTime);

			Logger.Verbose($"recorded switch: {name} | {state}");
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="gc"></param>
	/// <param name="name"></param>
	/// <param name="enabled"></param>
	internal void SetSwitchFakeProc(IGameController gc, ushort number, bool enabled)
	{
		var proc = gc?.PROC as IFakeProcDevice;
		if (proc != null)
		{
			var sw = gc.Switches[number];
			var evtT = enabled ? EventType.SwitchClosedDebounced : EventType.SwitchOpenDebounced;
			proc.AddSwitchEvent(sw.Number, evtT);

			RecordSwitch(sw.Name, sw);
		}
	}
}
