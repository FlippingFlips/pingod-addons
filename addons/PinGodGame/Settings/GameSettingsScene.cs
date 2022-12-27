using Godot;

/// <summary>
/// Game settings scene menu, with basic props, balls per game, ball save time
/// </summary>
public class GameSettingsScene : MarginContainer
{
    private PinGodGame pinGod;

    /// <summary>
    /// Connects value_changed to menu spin box events which uses the <see cref="PinGodGame.GameSettings"/> to save to
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        pinGod = GetNode<PinGodGame>("/root/PinGodGame");

        var _ballsPerGame = GetNode<SpinBox>("VBoxContainer/BallsPerGameSpinBox");
        _ballsPerGame.Value = pinGod.GameSettings.BallsPerGame;
        _ballsPerGame.Prefix = Tr("SETT_BALLS");
        _ballsPerGame.Connect("value_changed", this, nameof(_on_BallsPerGameSpinBox_changed));

        var _ballSaveTime = GetNode<SpinBox>("VBoxContainer/BallSaveTimeSpinBox");
        _ballSaveTime.Value = pinGod.GameSettings.BallSaveTime;
        _ballSaveTime.Prefix = Tr("SETT_BALL_SAVE");
        _ballSaveTime.Connect("value_changed", this, nameof(_on_BallSaveTimeSpinBox_changed));

        var _extraBalls = GetNode<SpinBox>("VBoxContainer/ExtraBallsSpinBox");
        _extraBalls.Value = pinGod.GameSettings.MaxExtraBalls;
        _extraBalls.Prefix = Tr("SETT_XB_MAX");
        _extraBalls.Connect("value_changed", this, nameof(_on_ExtraBallsSpinBox_changed));
    }

    void _on_BallsPerGameSpinBox_changed(float val) => pinGod.GameSettings.BallsPerGame = (byte)val;
    void _on_BallSaveTimeSpinBox_changed(float val) => pinGod.GameSettings.BallSaveTime = (byte)val;
    void _on_ExtraBallsSpinBox_changed(float val) => pinGod.GameSettings.MaxExtraBalls = (byte)val;
}
