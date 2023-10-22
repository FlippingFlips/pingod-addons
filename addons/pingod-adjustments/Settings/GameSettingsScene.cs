using Godot;
using PinGod;
using PinGod.Core;

/// <summary>
/// Game settings scene menu, with basic props, balls per game, ball save time
/// </summary>
public partial class GameSettingsScene : MarginContainer
{
    private IPinGodGame pinGod;

    /// <summary>
    /// Connects value_changed to menu spin box events which uses the <see cref="IPinGodGame.Adjustments"/> to save to
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        if (HasNode(Paths.ROOT_PINGODGAME))
        {
            pinGod = GetNode<IPinGodGame>(Paths.ROOT_PINGODGAME);
        }        

        var _ballsPerGame = GetNode<SpinBox>("VBoxContainer/BallsPerGameSpinBox");
        _ballsPerGame.Value = pinGod?.Adjustments?.BallsPerGame ?? 3;
        _ballsPerGame.Prefix = Tr("SETT_BALLS");
        _ballsPerGame.Connect("value_changed", new Callable(this, nameof(_on_BallsPerGameSpinBox_changed)));

        var _ballSaveTime = GetNode<SpinBox>("VBoxContainer/BallSaveTimeSpinBox");
        _ballSaveTime.Value = pinGod?.Adjustments?.BallSaveTime ?? 8;
        _ballSaveTime.Prefix = Tr("SETT_BALL_SAVE");
        _ballSaveTime.Connect("value_changed", new Callable(this, nameof(_on_BallSaveTimeSpinBox_changed)));

        var _extraBalls = GetNode<SpinBox>("VBoxContainer/ExtraBallsSpinBox");
        _extraBalls.Value = pinGod?.Adjustments?.MaxExtraBalls ?? 5;
        _extraBalls.Prefix = Tr("SETT_XB_MAX");
        _extraBalls.Connect("value_changed", new Callable(this, nameof(_on_ExtraBallsSpinBox_changed)));
    }

    void _on_BallsPerGameSpinBox_changed(float val)
    {
        if(pinGod!=null) pinGod.Adjustments.BallsPerGame = (byte)val;
    }
    void _on_BallSaveTimeSpinBox_changed(float val)
    {
        if (pinGod != null) pinGod.Adjustments.BallSaveTime = (byte)val;
    }
    void _on_ExtraBallsSpinBox_changed(float val)
    {
        if (pinGod != null) pinGod.Adjustments.MaxExtraBalls = (byte)val;
    }
}
