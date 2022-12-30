using static Godot.GD;
/// <summary>
/// Static logger class. Prints Godot errors and push warning / errors to godot debugger
/// </summary>
public static class Logger
{
    /// <summary>
    /// 
    /// </summary>
    public static PinGodLogLevel LogLevel { get; set; }
    /// <summary>
    /// Use for switches, or other verbose logging
    /// </summary>
    /// <param name="what"></param>
    public static void Verbose(params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Verbose)
        {
            Print(what);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="what"></param>
    public static void Debug(params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Debug)
        {
            Print(what);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    public static void Error(string message = null, params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Warning)
        {
            if (what?.Length > 0) PrintErr(message, what);
            else PrintErr(message, what);
            PushError(message);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="what"></param>
    public static void Info(params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Info)
        {
            Print(what);
        }
    }
    /// <summary>
    /// Logs warnings and also pushes warnings to Godot
    /// </summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    public static void Warning(string message = null, params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Warning)
        {
            if(what?.Length > 0) Print(message, what);
            else Print(message, what);
            PushWarning(message);
        }
    }
}