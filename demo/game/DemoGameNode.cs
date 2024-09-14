using PinGod.Base;

/// <summary>This game class is a Godot Node with scenes and a modes layer</summary>
public partial class DemoGameNode : Game
{
    /// <summary> Add 100 extra points to bonus </summary>
    /// <param name="points"></param>
    /// <param name="bonus"></param>
    public override void AddPoints(int points, int bonus = 50) => 
        base.AddPoints(points, bonus + 1000);

    /// <summary> Just logs </summary>
    public void OnBallSaveDisabled() => Logger
        .Log(LogLevel.Info, Logger.BBColor.green, nameof(DemoGameNode), ":" + nameof(OnBallSaveDisabled));

    /// <summary> Just logs </summary>
	public void OnBallSaveEnabled() => Logger
        .Log(LogLevel.Info, Logger.BBColor.green, nameof(DemoGameNode), ":" + nameof(OnBallSaveEnabled));
}
