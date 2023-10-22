public interface IGame
{
    void AddMultiballSceneToTree();
    void AddPoints(int points, int bonus = 25);
    void EndMultiball();
    void OnBallDrained();
    void OnBallEnded(bool lastBall);
    void OnBallSaved();
    void OnBonusEnded();
    void OnScoreEntryEnded();
    void StartNewBall();
    void _EnterTree();
    void _ExitTree();
    void _Ready();
}