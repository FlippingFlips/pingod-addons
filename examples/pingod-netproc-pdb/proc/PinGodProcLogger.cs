using Godot;
using NetProc.Domain;
using NetProc.Domain.PinProc;
using PinGod.Core;

internal class PinGodProcLogger : ILogger
{
    public LogLevel LogLevel { get; set; }

    public void Log(string text)
    {
        if (CanLog(LogLevel.Info))
            Logger.Info("P-ROC:", text);
    }

    public void Log(string text, LogLevel logLevel = LogLevel.Info)
    {
        if (CanLog(logLevel))
        {
            GD.Print(text);
        }
    }

    public void Log(LogLevel logLevel = LogLevel.Info, params object[] logObjs)
    {
        if (CanLog(logLevel))
        {
            GD.Print(logObjs);
        }
    }

    public void Log(params object[] logObjs)
    {
        GD.Print(logObjs);
    }

    bool CanLog(LogLevel logLevel) => LogLevel <= logLevel;
}
