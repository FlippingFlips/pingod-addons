using Godot;
using PinGod.Base;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PinGod.Core
{

    /// <summary>
    /// Default game settings for the machine / game. <para/>
    /// %AppData%\Godot\app_userdata
    /// </summary>
    public partial class Adjustments
    {
        /// <summary>
        /// File saved in the game folder in users roaming GODOT 
        /// </summary>
        [JsonIgnore]
        const string GAME_SETTINGS_FILE = "user://adjustments.save";

        /// <summary>
        /// default ball save time
        /// </summary>
        public byte BallSaveTime { get; set; } = 12;
        /// <summary>
        /// default balls per game = 3
        /// </summary>
        public byte BallsPerGame { get; set; } = 3;

        /// <summary>
        /// Max extra balls used in game, defaults to 2
        /// </summary>
        public byte MaxExtraBalls { get; set; } = 4;
        /// <summary>
        /// Max extra balls for
        /// </summary>
        public byte MaxExtraBallsBallInPlay { get; set; } = 0;
        /// <summary>
        /// maximum high scores to keep, defaults to 5
        /// </summary>
        public byte MaxHiScoresCount { get; set; } = 5;


        public byte TiltWarnings { get; set; } = 3;

        /// <summary>
        /// The current language for this machine
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte ReplayPercent { get; set; } = 10;

        /// <summary>
        /// 
        /// </summary>
        public byte ReplayStart { get; set; } = 10;

        /// <summary>
        /// 
        /// </summary>
        public byte ReplayLevels { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public byte ReplayBoost { get; set; } = 50;

        /// <summary>
        /// default ball save time
        /// </summary>
        public byte MatchPercent { get; set; } = 10;

        /// <summary>
        /// Access to change display setting
        /// </summary>
        public DisplaySettings Display { get; set; } = new DisplaySettings();

        /// <summary>
        /// Logging level
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Error;

        /// <summary>
        /// Read from memory map?
        /// </summary>
        public bool MachineStatesRead { get; set; } = true;

        /// <summary>
        /// Write states to memory map?
        /// </summary>
        public bool MachineStatesWrite { get; set; } = true;

        /// <summary>
        /// Delay to write to memory. 10 default, 1 high CPU, 500 too much...
        /// </summary>
        public int MachineStatesWriteDelay { get; set; } = 10;

        /// <summary>
        /// A command switch number for the memory map so user can send a byte over a switch number <para/>
        /// Set this id greater than > 0. 0 is default for game state
        /// </summary>
        public int VpCommandSwitchId { get; set; } = -1;

        /// <summary>
        /// Decibel volume. minus values. -80 lowest
        /// </summary>
        public float MasterVolume { get; set; } = 0;

        /// <summary>
        /// Enable / Disable music
        /// </summary>

        public bool MusicEnabled { get; set; } = true;

        /// <summary>
        /// Enable / Disable SFX
        /// </summary>
        public bool SfxEnabled { get; set; } = true;

        /// <summary>
        /// Enable / Disable Voice
        /// </summary>
        public bool VoiceEnabled { get; set; } = true;

        [JsonNumberHandling(handling: JsonNumberHandling.AllowNamedFloatingPointLiterals)]
        /// <summary>
        /// Decibel volume. minus values. -80 lowest
        /// </summary>
        public float MusicVolume { get; set; } = -6.0f;

        /// <summary>
        /// Decibel volume. minus values. -80 lowest
        /// </summary>
        public float SfxVolume { get; set; } = -6.0f;

        /// <summary>
        /// Decibel volume. minus values. -80 lowest
        /// </summary>
        public float VoiceVolume { get; set; } = -6.0f;

        /// <summary>
        /// De-serializes settings from json if Type is <see cref="Adjustments"/>
        /// </summary>
        public static T DeserializeSettings<T>(string gameSettingsJson) where T : Adjustments => JsonSerializer.Deserialize<T>(gameSettingsJson);

        /// <summary>
        /// Loads game settings file from the user directory. Creates a new save file if there isn't one available
        /// </summary>
        public static T Load<T>() where T : Adjustments
        {
            T gS = Activator.CreateInstance<T>();
            using var settingsSave = FileAccess.Open(GAME_SETTINGS_FILE, FileAccess.ModeFlags.Read);
            if (FileAccess.GetOpenError() != Error.FileNotFound)
            {
                gS = DeserializeSettings<T>(settingsSave.GetLine());
                Logger.Info(nameof(Adjustments), ":loaded from file");
            }
            else { gS = null; }

            return gS;
        }

        /// <summary>
        /// Saves generic <see cref="Adjustments"/>
        /// </summary>
        //public static void Save<T>(T settings) where T : Adjustments => Save((T)settings);

        /// <summary>
        /// Saves <see cref="Adjustments"/>
        /// </summary>
        public static void Save<T>(T settings) where T : Adjustments
        {
            if (settings != null)
            {
                using var saveGame = FileAccess.Open(GAME_SETTINGS_FILE, FileAccess.ModeFlags.Write);
                saveGame.StoreLine(JsonSerializer.Serialize(settings, new JsonSerializerOptions() { IgnoreReadOnlyFields = true }));
            }
        }

        /// <summary>
        /// Loads game settings file from the user directory. Creates a new save file if there isn't one available
        /// </summary>
        public static Adjustments Load()
        {
            using var settingsSave = FileAccess.Open(GAME_SETTINGS_FILE, FileAccess.ModeFlags.Read);
            Adjustments gS = new Adjustments();
            if (FileAccess.GetOpenError() != Error.FileNotFound)
            {
                gS = JsonSerializer.Deserialize<Adjustments>(settingsSave.GetLine());
            }
            else
            {
                Save(gS);
            }

            if (gS == null)
                gS = new Adjustments();
            return gS;
        }
    }
}