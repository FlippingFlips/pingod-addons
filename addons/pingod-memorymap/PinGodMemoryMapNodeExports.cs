using Godot;

/// <summary>
/// Exports for godot editor
/// </summary>
public partial class PinGodMemoryMapNode
{
    #region Exports
    [Export]
    [ExportGroup("Enable")]
    bool IsEnabled = false;
    /// <summary>
    /// 10 = low cpu
    /// </summary>
    [Export(PropertyHint.Range, "-1,1000")]
    [ExportGroup("Enable")]
    int WriteDelay = 10;
    [Export(PropertyHint.Range, "-1,1000")]
    [ExportGroup("Enable")]
    int ReadDelay = 10;
    [Export]
    [ExportGroup("Map")]
    string MapName = "pingod_vp";
    [Export(PropertyHint.Range,"32,128")]
    [ExportGroup("Map")]
    byte CoilTotal = 32;
    [Export(PropertyHint.Range, "64,256")]
    [ExportGroup("Map")]
    byte LampTotal = 64;
    [Export(PropertyHint.Range, "64,256")]
    [ExportGroup("Map")]
    byte LedTotal = 64;
    [Export(PropertyHint.Range, "64,256")]
    [ExportGroup("Map")]
    byte SwitchTotal = 64;
    [Export]
    [ExportGroup("Mutex")]
    string MutexName = "pingod_vp_mutex";
    #endregion
}
