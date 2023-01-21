using Godot;
using Godot.Collections;
using NetProc.Domain;
using NetProc.Domain.PinProc;
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

    public override void OnSwitchCommand(string name, int index, byte value)
    {
        Logger.Verbose("MACHINE_PROC: Switch:" + index);
        //base.OnSwitchCommand(name, index, value);
        if (_pinGodGameProc != null)
        { 
            SetSwitchFakeProc(_pinGodGameProc.PinGodProcGame, (ushort)index, value > 0 ? true : false);
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
        var proc = gc?.PROC as IFakeProcDevice;
        if (proc != null)
        {
            var sw = gc.Switches[name];
            var evtT = enabled ? EventType.SwitchClosedDebounced : EventType.SwitchOpenDebounced;
            proc.AddSwitchEvent(sw.Number, evtT);
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
        }
    }
}
