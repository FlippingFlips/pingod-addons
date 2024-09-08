using PinGod.Base;
using System;
using static Godot.GD;

/// <summary>
/// Static logger class. Prints Godot errors and push warning / errors to godot debugger
/// </summary>
public static class Logger
{
    public enum BBColor
    {
        white,
        red,
        yellow,
        blue,
        green
    }

    public static event EventHandler<string> LoggedMessage;

    /// <summary>    
    public static LogLevel LogLevel { get; set; } = 0;

    public static string LogPrefix { get; set; } = "[PGOD]";

    public static bool TimeStamp { get; set; } = true;

    public static void Debug(params object[] what)
    {
        if (LogLevel <= LogLevel.Debug)
        {
            Log(LogLevel.Debug, BBColor.white, what);
        }
    }

    /// <summary></summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    public static void Error(string message = null, params object[] what)
    {
        if (LogLevel <= LogLevel.Error)
        {
            if (what?.Length > 0) PrintErr(LogLevel.Error, message, what);
            else PrintErr(LogLevel.Error, message, what);
            //PushError(message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="what"></param>
    public static void Info(params object[] what)
    {
        if (LogLevel <= LogLevel.Info)
        {
            //Print(what);
            Log(LogLevel.Info, BBColor.white, what);
        }
    }

    /// <summary>
    /// Appends the log level and color to arguments
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="what"></param>
    public static void Log(LogLevel logLevel, BBColor color, params object[] what)
    {
        if (logLevel < LogLevel) return;

        var arr = new object[what.Length + 1];
        var msg = string.Empty;
        var endTag = string.Empty;
        if (color > 0)
        {
            msg += $"[color={color}]";
            endTag += "[/color]";
        }
        msg += $"{LogPrefix}[{logLevel}][{DateTime.Now.TimeOfDay}]:";
        if (color > 0)
            msg += endTag;

        arr[0] = msg;
        what.CopyTo(arr, 1);
        PrintRich(arr);

        LoggedMessage?.Invoke(null, string.Join(" ", arr));
    }

    /// <summary>
    /// Use for switches, or other verbose logging
    /// </summary>
    /// <param name="what"></param>
    public static void Verbose(params object[] what)
    {
        if (LogLevel <= LogLevel.Verbose)
        {
            //Print(what);
            Log(LogLevel.Verbose, BBColor.white, what);
        }
    }
    /// <summary>
    /// Logs warnings and also pushes warnings to Godot
    /// </summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    public static void Warning(params object[] what)
    {
        if (LogLevel <= LogLevel.Warning && what?.Length > 0)
        {
            Log(LogLevel.Warning, BBColor.yellow, what);
        }
    }

    /// <summary>
    /// bbcodes with log https://www.bbcode.org/reference.php <para/>
    /// [url]{url}[/url]
    /// </summary>
    /// <param name="what"></param>
    public static void WarningRich(params object[] what)
    {
        if (LogLevel <= LogLevel.Warning && what?.Length > 0)
        {
            var msg = AppendPrefixToParams(LogLevel.Warning, what);
            PrintRich(msg);
            LoggedMessage?.Invoke(null, string.Join(" ", msg));
        }
    }

    private static object[] AppendPrefixToParams(LogLevel level, params object[] what)
    {
        var newParams = new object[what.Length + 1];
        newParams[0] = $"{LogPrefix}[{level}][{DateTime.Now.TimeOfDay}]:";
        what.CopyTo(newParams, 1);
        return newParams;
    }
}