using Godot;

namespace PinGod.Core.Service
{

    /// <summary>Memory Map Settings</summary>
    public partial class MemoryMapNode
    {
        #region Exports        
        [Export]
        [ExportGroup("Map")]
        protected string MapName = "pingod_vp";
        [Export(PropertyHint.Range, "32,128")]
        [ExportGroup("Map")]
        protected byte CoilTotal = 32;
        [Export(PropertyHint.Range, "64,256")]
        [ExportGroup("Map")]
        protected byte LampTotal = 64;
        [Export(PropertyHint.Range, "64,256")]
        [ExportGroup("Map")]
        protected byte LedTotal = 64;
        [Export(PropertyHint.Range, "64,256")]
        [ExportGroup("Map")]
        protected byte SwitchTotal = 128;
        [Export]
        [ExportGroup("Mutex")]
        protected string MutexName = "pingod_vp_mutex";
        #endregion
    }
}