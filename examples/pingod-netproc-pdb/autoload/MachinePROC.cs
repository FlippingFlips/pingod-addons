using NetProc.Data;
using NetProc.Domain;
using NetProc.Domain.PinProc;
using PinGod.Core;
using PinGod.Core.Service;

/// <summary>
/// This script is loaded with the `res://autoload/Machine.tscn` which inherits a PinGod-MachineNode. <para/>
/// Singleton can be retrieved from the /root/ with GetNode&lt;MachinePROC&gt;("/root/Machine"). <para/>
/// EnterTree initializes a database, gets machine config
/// </summary>
public partial class MachinePROC : MachineNode
{
    public MachineConfiguration _machineConfig;
    private PinGodGameProc _pinGodProc;

    const bool DELETE_DB_ON_INIT = true;
    const bool IS_MACHINE_PDB = true;

    /// <summary> and machine configuration. Machine config is held public here
    /// Creates database
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        try
        {
            //DATABASE - EF CORE SQLITE
            Logger.Info(nameof(MachinePROC), ": init database");
            using var ctx = new NetProcDbContext();
            ctx.InitializeDatabase(true, DELETE_DB_ON_INIT);

            //MACHINE CONFIG FROM DATABASE TABLES
            Logger.Info(nameof(MachinePROC), ": database init complete, creating"+nameof(MachineConfiguration));
            _machineConfig = ctx.GetMachineConfiguration();
            Logger.Info(nameof(MachinePROC), ": machine config created\n",
                $"      Machine Type: {_machineConfig.PRGame.MachineType}, Balls: {_machineConfig.PRGame.NumBalls}\n Creating ProcGame...");
        }
        catch (System.Exception ex)
        {
            Logger.Error(nameof(MachinePROC), $"{ex.Message} {ex.InnerException?.Message}");
        }
    }

    public override void _ExitTree()
    {
        Logger.Info(nameof(MachinePROC), ":", nameof(_ExitTree));

        base._ExitTree();
    }

    public override void _Ready()
    {
        base._Ready();
        _pinGodProc = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
    }

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
        if(proc != null)
        {
            var sw = gc.Switches[name];
            var evtT = enabled ? EventType.SwitchClosedDebounced : EventType.SwitchOpenDebounced;
            proc.AddSwitchEvent(sw.Number, evtT);
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
        if (_pinGodProc != null)
        {
            //var sw = _switches[@switch.Name];
            SetSwitchFakeProc(_pinGodProc.PinGodProcGame, @switch.Name, value > 0 ? true : false);            
        }        
        base.SetSwitch(@switch, value, fromAction);
    }
}
