using Godot;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Game data for the machine. %AppData%\Godot\app_userdata <para/>
/// You can inherit this class and use the generic save, load so should work for all games.
/// </summary>
public partial class GameData
{
	/// <summary>
	/// File saved in the game folder in users roaming GODOT 
	/// </summary>
	[JsonIgnore]
	const string GAME_DATA_FILE = "user://gamedata.save";

	/// <summary>
	/// Total Balls played
	/// </summary>
	public int BallsPlayed { get; set; }
	/// <summary>
	/// Total Balls started
	/// </summary>
	public int BallsStarted { get; set; }
	/// <summary>
	/// Credits saved in machine
	/// </summary>
	public byte Credits { get; set; }
	/// <summary>
	/// Amount of games finished to the end
	/// </summary>
	public int GamesFinished { get; set; }
	/// <summary>
	/// Total games played
	/// </summary>
	public int GamesPlayed { get; set; }
	/// <summary>
	/// Total games started
	/// </summary>
	public int GamesStarted { get; set; }
	/// <summary>
	/// Collection of saved High Scores
	/// </summary>
	public List<HighScore> HighScores { get; set; } = new List<HighScore>();
	/// <summary>
	/// Total times tilted
	/// </summary>
	public int Tilted { get; set; }
	/// <summary>
	/// Total time played
	/// </summary>
	public ulong TimePlayed { get; set; }

	/// <summary>
	/// De-serializes gamedata from json if Type is <see cref="GameData"/>
	/// </summary>
	public static T DeserializeGameData<T>(string gameSettingsJson) where T : GameData => JsonSerializer.Deserialize<T>(gameSettingsJson);

    /// <summary>
    /// Loads the <see cref="GAME_DATA_FILE"/>
    /// </summary>
    /// <returns></returns>
    public static GameData Load()
	{
        GameData gameData = new GameData();
        using var saveGame = FileAccess.Open(GAME_DATA_FILE, FileAccess.ModeFlags.Read);
		if (FileAccess.GetOpenError() != Error.FileNotFound)
		{
			gameData = JsonSerializer.Deserialize<GameData>(saveGame.GetLine());
		}
		else { Save(gameData); }

		return gameData;
	}

	/// <summary>
	/// Loads generic game data file from the user directory. Creates a new game data file if there isn't one available
	/// </summary>
	public static T Load<T>() where T : GameData
	{
		using var dataSave = FileAccess.Open(GAME_DATA_FILE, FileAccess.ModeFlags.Read);
		T gD = default(T);
		if (FileAccess.GetOpenError() != Error.FileNotFound)
		{
			gD = DeserializeGameData<T>(dataSave.GetLine());
            Logger.Info(nameof(GameSettings), ":gamedata.save loaded");
        }
		else
		{
			Save(gD);
            Logger.Info(nameof(GameSettings), ":gamedata.save created");
        }

		return gD;
	}

	/// <summary>
	/// Saves <see cref="GameData"/>
	/// </summary>
	public static void Save(GameData gameData)
	{
		using var saveGame = FileAccess.Open(GAME_DATA_FILE, FileAccess.ModeFlags.Write);
		saveGame.StoreLine(JsonSerializer.Serialize(gameData));
	}

	/// <summary>
	/// Saves generic <see cref="GameData"/>
	/// </summary>
	public static void Save<T>(T gameData) where T : GameData
	{		
		using var saveGame = FileAccess.Open(GAME_DATA_FILE, FileAccess.ModeFlags.Write);
		saveGame.StoreLine(JsonSerializer.Serialize(gameData));
	}
}
