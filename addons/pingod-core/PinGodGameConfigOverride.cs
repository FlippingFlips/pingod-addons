using Godot;
using PinGod.Base;
/// <summary> Developer options, logging options. Saved to `proc.cfg`</summary>
public class PinGodGameConfigOverride
{
    public static readonly string _ConfigPath =
        $"res://{ProjectSettings.GetSetting("application/config/name")}.cfg";

    /// <summary>Level of output log. written logs found in userdatadir/logs/godot.log</summary>
	public LogLevel LogLevel { get; set; } = LogLevel.Verbose;

    /// <summary>Console and loggin window</summary>
	public static bool ConsoleEnabled { get; set; } = false;

    public static bool EmbedSubWindows { get; set; } = true;

    /// <summary>DisplayExtenstions tools panel buttons to open developer windows?</summary>
    public bool ToolsPaneEnabled { get; set; } = false;

    /// <summary>WINDOWS: for sharing events with a memory map with simulators and the like</summary>
	public bool MemoryMapEnabled { get; set; } = false;

    /// <summary> writes can be larger, because for proc states are written in its game loop </summary>
    public int MemoryMapWriteDelay { get; set; } = 10000;

    /// <summary> read from memory mapping switch states delay</summary>
    public int MemoryMapReadDelay { get; set; } = 10;

    private static ConfigFile configFile = null;

    public virtual ConfigFile Load()
    {
        configFile = new ConfigFile();
        Error err = configFile.Load(_ConfigPath);
        if (err != Error.Ok)
        {
            configFile.SetValue("DEV", nameof(LogLevel), (int)LogLevel);
            configFile.SetValue("DEV", nameof(EmbedSubWindows), EmbedSubWindows);
            configFile.SetValue("DEV", nameof(ConsoleEnabled), ConsoleEnabled);            
            configFile.SetValue("DEV", nameof(ToolsPaneEnabled), ToolsPaneEnabled);
            configFile.SetValue("MEMORYMAP", nameof(MemoryMapEnabled), MemoryMapEnabled);
            configFile.SetValue("MEMORYMAP", nameof(MemoryMapWriteDelay), MemoryMapWriteDelay);
            configFile.SetValue("MEMORYMAP", nameof(MemoryMapReadDelay), MemoryMapReadDelay);
        }
        else
        {
            LogLevel = (LogLevel)((int)configFile.GetValue("DEV", nameof(LogLevel)));
            EmbedSubWindows = (bool)configFile.GetValue("DEV", nameof(EmbedSubWindows));
            ToolsPaneEnabled = (bool)configFile.GetValue("DEV", nameof(ToolsPaneEnabled));
            ConsoleEnabled = (bool)configFile.GetValue("DEV", nameof(ConsoleEnabled));

            MemoryMapEnabled = (bool)configFile.GetValue("MEMORYMAP", nameof(MemoryMapEnabled));
            MemoryMapWriteDelay = (int)configFile.GetValue("MEMORYMAP", nameof(MemoryMapWriteDelay));
            MemoryMapReadDelay = (int)configFile.GetValue("MEMORYMAP", nameof(MemoryMapReadDelay));
        }
        return configFile;
    }

    public virtual void Save() { configFile?.Save(_ConfigPath); }
}
