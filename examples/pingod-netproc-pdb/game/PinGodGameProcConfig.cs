using NetProc.Domain.PinProc;

/// <summary>
/// Developer options, logging options. Saved to `proc.cfg`
/// </summary>
public class PinGodGameProcConfig
{
	public bool Simulated { get; set; } = true;
	public byte Delay { get; set; } = 10;
	public bool IgnoreDbDisplay { get; set; }
	public bool DeleteDbOnInit { get; set; }
	public LogLevel LogLevel { get; set; } = LogLevel.Verbose;
	public bool SwitchWindowEnabled { get; set; } = false;

	public bool MemoryMapEnabled { get; set; } = false;

    /// <summary> writes can be larger because proc states written in its game loop </summary>
    public int MemoryMapWriteDelay { get; set; } = 10000;

    /// <summary> read from memory mapping states delay</summary>
    public int MemoryMapReadDelay { get; set; } = 10;

}
