using PinGod.Base;

/// <summary>
/// Logging wrapper around Godot's Print methods and PushErrors
/// </summary>
public interface IPinGodLogger
{
    /// <summary>
    /// 
    /// </summary>
    LogLevel LogLevel { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="what"></param>
    void Debug(params object[] what);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    void Error(string message = null, params object[] what);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="what"></param>
    void Info(params object[] what);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="what"></param>
    void Warning(string message = null, params object[] what);
}