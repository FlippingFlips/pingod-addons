using Godot;

namespace PinGod.Core.Game
{
    /// <summary>
    /// Contains generic base signals to emit from the game.
    /// </summary>
    public abstract partial class PinGodBase : Node
    {
        [Signal] public delegate void BallDrainedEventHandler();
        [Signal] public delegate void BallEndedEventHandler(bool lastBall);
        [Signal] public delegate void BallSavedEventHandler();
        [Signal] public delegate void BallSaveEndedEventHandler();
        [Signal] public delegate void BonusEndedEventHandler();
        [Signal] public delegate void CreditAddedEventHandler(int credits);
        [Signal] public delegate void GameEndedEventHandler();
        [Signal] public delegate void GameStartedEventHandler();
        [Signal] public delegate void ModeTimedOutEventHandler(string title);
        [Signal] public delegate void MultiBallEndedEventHandler();
        [Signal] public delegate void MultiballStartedEventHandler();
        [Signal] public delegate void PlayerAddedEventHandler();
        [Signal] public delegate void ScoresUpdatedEventHandler();
        [Signal] public delegate void ServiceMenuEnterEventHandler();
        [Signal] public delegate void ServiceMenuExitEventHandler();
        [Signal] public delegate void VpCommandEventHandler(byte index);
    }
}