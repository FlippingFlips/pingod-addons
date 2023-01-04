using Godot;

/// <summary>
/// Contains generic base signals to emit from the game.
/// </summary>
public abstract partial class PinGodBase : Node
{
    #region Signals
    /// <summary>
    /// Emitted signal
    /// </summary>
    [Signal] public delegate void BallDrainedEventHandler();
    /// <summary>
    /// Emitted signal
    /// </summary>
    /// <param name="lastBall">Is this last ball?</param>
	[Signal] public delegate void BallEndedEventHandler(bool lastBall);
    /// <summary>
    /// Emitted signal when a ball is saved
    /// </summary>
	[Signal] public delegate void BallSavedEventHandler();
    /// <summary>
    /// Emitted signal when ball save is ended
    /// </summary>
	[Signal] public delegate void BallSaveEndedEventHandler();
    /// <summary>
    /// Emitted signal when bonus is ended
    /// </summary>
    [Signal] public delegate void BonusEndedEventHandler();
    /// <summary>
    /// Emitted signal when a credit is added to game
    /// </summary>
    /// <param name="credits">total credits in game</param>
    [Signal] public delegate void CreditAddedEventHandler(int credits);
    /// <summary>
    /// Emitted signal when game ends
    /// </summary>
    [Signal] public delegate void GameEndedEventHandler();    
    /// <summary>
    /// Emitted signal when game starts
    /// </summary>
    [Signal] public delegate void GameStartedEventHandler();
    /// <summary>
    /// Emitted signal when a mode times out
    /// </summary>
    [Signal] public delegate void ModeTimedOutEventHandler(string title);
    /// <summary>
    /// Emitted signal when multi-ball ends
    /// </summary>
    [Signal] public delegate void MultiBallEndedEventHandler();
    /// <summary>
    /// Emitted signal when multi-ball starts
    /// </summary>
    [Signal] public delegate void MultiballStartedEventHandler();
    /// <summary>
    /// Emitted signal when a player is added to the game
    /// </summary>
    [Signal] public delegate void PlayerAddedEventHandler();
    /// <summary>
    /// Emitted signal when player has finished entering their scores
    /// </summary>
    [Signal] public delegate void ScoreEntryEndedEventHandler();
    /// <summary>
    /// Emitted signal each time score is updated
    /// </summary>
    [Signal] public delegate void ScoresUpdatedEventHandler();
    /// <summary>
    /// Emitted signal when player enters the service menu
    /// </summary>
    [Signal] public delegate void ServiceMenuEnterEventHandler();
    /// <summary>
    /// Emitted signal when player exits the service menu
    /// </summary>
	[Signal] public delegate void ServiceMenuExitEventHandler();

    /// <summary>
    /// Signal sent from memory map if <see cref="Adjustments.VpCommandSwitchId"/> was found while reading switch states
    /// </summary>
    /// <param name="index"></param>
    [Signal] public delegate void VpCommandEventHandler(byte index);
    
    #endregion
}