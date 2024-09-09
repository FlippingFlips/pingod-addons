public abstract partial class PinGodGameModeControl : PinGodGameControl
{
    /// <summary> Mode group signals</summary>
    protected virtual void OnBallDrained() { }

    /// <summary>Mode group signals</summary>
    protected virtual void OnBallSaved() { }

    /// <summary>Mode group signals</summary>
    protected virtual void OnBallStarted() { }

    /// <summary>Mode group signals</summary>
    protected virtual void UpdateLamps() { }

    public override void _EnterTree() => base._EnterTree();
}
