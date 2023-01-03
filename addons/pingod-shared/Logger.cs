using System.Security.Permissions;
using static Godot.GD;
/// <summary>
/// Static logger class. Prints Godot errors and push warning / errors to godot debugger
/// </summary>
public static class Logger
{
    /// <summary>    
    public static PinGodLogLevel LogLevel { get; set; } = 0;
    /// <summary>
    /// Use for switches, or other verbose logging
    /// </summary>
    /// <param name="what"></param>
    public static void Verbose(params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Verbose)
        {
            Print(what);
            //Log(PinGodLogLevel.Verbose, what);            
        }
    }
    /// <summary>
    /// Appends the log level to arguments
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="what"></param>
    public static void Log(PinGodLogLevel logLevel, params object[] what) => 
        Print(logLevel, what);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="what"></param>
    public static void Debug(params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Debug)
        {
            Print(what);
            //Log(PinGodLogLevel.Debug, what);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    public static void Error(string message = null, params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Error)
        {
            if (what?.Length > 0) PrintErr(PinGodLogLevel.Error, message, what);
            else PrintErr(PinGodLogLevel.Error,message, what);
            //PushError(message);
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
            //Log(PinGodLogLevel.Info, what);            
        }
    }
    /// <summary>
    /// Logs warnings and also pushes warnings to Godot
    /// </summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    public static void Warning(params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Warning && what?.Length>0)
        {
            
            Print(what);

            //won't use the object here, need to wrap it
            //PrintRich("[code][b]", what, "[/b][/code]");

            //todo: move somewhere else. godot push call stack but without these objects
            //if (what[0] != null)
            //    PushWarning(what[0].ToString());//push warning not good in Godot. Cannot see the message you push but you can see call stack
        }
    }

    /// <summary>
    /// bbcodes with log https://www.bbcode.org/reference.php <para/>
    /// [url]{url}[/url]
    /// </summary>
    /// <param name="what"></param>
    public static void WarningRich(params object[] what)
    {
        if (LogLevel <= PinGodLogLevel.Warning && what?.Length > 0)
        {

            PrintRich(what);

            //PrintStack();

            //todo: move somewhere else. godot push call stack but without these objects
            //if (what[0] != null)
            //    PushWarning(what[0].ToString());//push warning not good in Godot. Cannot see the message you push but you can see call stack
        }
    }
}