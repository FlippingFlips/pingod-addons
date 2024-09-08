using System.Collections.Generic;

namespace PinGodAddOns.addons.pingod_core.Model
{
    /// <summary>
    /// Collections of switches and coils for ball searching
    /// </summary>
    public class MachineProcBallSearch
    {
        /// <summary>
        /// Coils to pulse
        /// </summary>
        public List<string> PulseCoils { get; set; } = new List<string>();

        /// <summary>
        /// Reset ball search switches
        /// </summary>
        public Dictionary<string, string> ResetSwitches { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Stop ball search switches
        /// </summary>
        public Dictionary<string, string> StopSwitches { get; set; } = new Dictionary<string, string>();
    }
}
