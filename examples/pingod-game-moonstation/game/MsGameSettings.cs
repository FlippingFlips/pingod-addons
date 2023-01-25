using PinGod.Core;

namespace MoonStation.game
{
    /// <summary>
    /// Custom settings which will be saved to a file
    /// </summary>
    public partial class MsGameSettings : Adjustments
    {
        public string Music { get; set; } = "techno";
    }
}
