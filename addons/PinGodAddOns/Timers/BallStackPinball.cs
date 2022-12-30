using Godot;

/// <summary>
/// Tool: Kicker / Saucer node based on timer and to process switch actions.
/// </summary>
[Tool]
public class BallStackPinball : Timer
{
    /// <summary>
    /// Coil name
    /// </summary>
    [Export] public string _coil = null;

    /// <summary>
    /// Switch name to listen for active/inactive
    /// </summary>
    [Export] string _switch = null;

    private PinGodGame pingod;

    /// <summary>
    /// Emitted when switch is on
    /// </summary>
    [Signal] public delegate void SwitchActive();

    /// <summary>
    /// Emitted when switch is off
    /// </summary>
    [Signal] public delegate void SwitchInActive();

    #region Public Methods
    /// <summary>
    /// Connects the <see cref="OnSwitchCommandHandler"/>
    /// </summary>
    public override void _EnterTree()
	{
		if (!Engine.EditorHint)
		{
			pingod = GetNode("/root/PinGodGame") as PinGodGame;
            //emit signal when the switch is active
            pingod.Connect(nameof(PinGodBase.SwitchCommand), this, nameof(OnSwitchCommandHandler));
        }
	}

    private void OnSwitchCommandHandler(string name, byte index, byte value)
    {
        if (_switch == null) return;
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
        Stop();
    }

    /// <summary>
    /// Turns off SetProcessInput to process no switches if they haven't been set
    /// </summary>
    public override void _Ready()
    {
		if (!Engine.EditorHint)
		{
			// Code to execute when in game.
			if (string.IsNullOrWhiteSpace(_switch))
			{
				Logger.Error("no _sw_action set", this.Name);
				SetProcessInput(false);
			}
		}
	}

    /// <summary>
    /// Uses Pingod <see cref="PinGodGame.SolenoidPulseTimer"/>
    /// </summary>
    public void SolenoidPulse() => pingod.SolenoidPulseTimer(_coil);

    #endregion
}
