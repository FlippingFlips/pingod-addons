using PinGod.Base;

public partial class DemoGame : Game
{
    /// <summary> Add 100 extra points to bonus </summary>
    /// <param name="points"></param>
    /// <param name="bonus"></param>
    public override void AddPoints(int points, int bonus = 50) => 
        base.AddPoints(points, bonus + 1000);

    /// <summary> Just logs </summary>
    public void OnBallSaveDisabled() => Logger
        .Log(LogLevel.Info, Logger.BBColor.green, nameof(DemoGame), ":" + nameof(OnBallSaveDisabled));

    /// <summary> Just logs </summary>
	public void OnBallSaveEnabled() => Logger
        .Log(LogLevel.Info, Logger.BBColor.green, nameof(DemoGame), ":" + nameof(OnBallSaveEnabled));
}
