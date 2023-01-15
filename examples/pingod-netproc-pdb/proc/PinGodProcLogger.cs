using NetProc.Domain;
using NetProc.Domain.PinProc;
using PinGod.Core;

internal class PinGodProcLogger : ILogger
{
    public void Log(string text) => Logger.Info("P-ROC:", text);

    public void Log(string text, LogLevel logLevel = LogLevel.Info)
    {
        Logger.Info("P-ROC:", logLevel, ":", text);
    }
}
