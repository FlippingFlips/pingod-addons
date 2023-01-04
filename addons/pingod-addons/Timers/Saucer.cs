using Godot;

/// <summary>
/// Tool: Kicker / Saucer node based on timer and to process switch actions. <para/>
/// TODO: this is pretty much the same as a bumper. Need re-factor and make a better base, new name
/// </summary>
[Tool]
public partial class Saucer : Timer
{
    /// <summary>
    /// Coil name
    /// </summary>
    [Export] public string _coil = null;

    /// <summary>
    /// Switch name to listen for active/inactive
    /// </summary>
    [Export] string _switch = null;
    
    private PinGodMachine _pingodMachine;

    /// <summary>
    /// Emitted when switch is on
    /// </summary>
    [Signal] public delegate void SwitchActiveEventHandler();

    /// <summary>
    /// Emitted when switch is off
    /// </summary>
    [Signal] public delegate void SwitchInActiveEventHandler();

    #region Public Methods
    /// <summary>
    /// Connects the <see cref="OnSwitchCommandHandler"/>
    /// </summary>
    public override void _EnterTree()
	{
        Logger.Debug(nameof(Saucer),":",nameof(_EnterTree));
		if (!Engine.IsEditorHint())
		{            
            //emit signal when the switch is active
            if (HasNode("/root/Machine"))
            {
                _pingodMachine = GetNode<PinGodMachine>("/root/Machine");
                _pingodMachine.SwitchCommand += OnSwitchCommandHandler;
                Logger.Debug(nameof(Saucer), ":", nameof(_EnterTree), ":Machine found, handling switches");
            }
            else { Logger.Debug(nameof(Saucer), ":", nameof(_EnterTree), ":Machine not found, not handling switches"); }
        }
	}

    private void DisableEventHandlers()
    {
        if (_pingodMachine!=null)
        {
            _pingodMachine.SwitchCommand -= OnSwitchCommandHandler;
        }
    }

    private void OnSwitchCommandHandler(string name, byte index, byte value)
    {
        if (_switch == null) 
        { 
            Logger.Warning(nameof(Saucer), ":no switch has been set, disabling."); 
            DisableEventHandlers(); 
            return; 
        }
        if(name == _switch)
        {
            if(value > 0)
                EmitSignal(nameof(SwitchActive));
            else
                EmitSignal(nameof(SwitchInActive));
        }
    }

    /// <summary>
    /// Stops the timer
    /// </summary>
    public override void _ExitTree()
    {
        Logger.Debug(nameof(Saucer), ":", nameof(_ExitTree));
        DisableEventHandlers();
        Stop();
    }

    /// <summary>
    /// Turns off SetProcessInput to process no switches if they haven't been set
    /// </summary>
    public override void _Ready()
    {
		if (!Engine.IsEditorHint())
		{
			// Code to execute when in game.
			if (string.IsNullOrWhiteSpace(_switch))
			{
				Logger.Error("no _sw_action set", this.Name);
				SetProcessInput(false);
                return;
			}
		}

        Logger.Debug(nameof(Saucer), ":", nameof(_Ready));
    }

    /// <summary>
    /// Uses <see cref="PinGodMachine.CoilPulse(string, int)(string)"/>.
    /// </summary>
    public void Kick(int pulse = 255) => _pingodMachine?.CoilPulse(_coil, pulse);

    #endregion
}
