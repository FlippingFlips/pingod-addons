using PinGod.Base;
using PinGod.Modes;
using System.Linq;

public partial class AttractProc : Attract
{
	private PinGodGameProc _pinGodProcGame;

	public override void _EnterTree()
	{
		base._EnterTree();
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
	}

	/// <summary>
	/// <inheritdoc/> <para/>
	/// This version gets top scores from database
	/// </summary>
	public override void GetHighScores()
	{
		//base.GetHighScores();

		_highScoresList?.Clear();
		var scores = _pinGodProcGame?.PinGodProcGame?.GetTopScores();
		if (scores?.Count() > 0) 
			_highScoresList = scores.Select(s => new HighScore { Name = s.Player.Name, Points = s.Points, Created = s.GamePlayed.Ended }).ToList();
	}
}
