using Godot;
using PinGod.Core;
using PinGod.Core.Game;

namespace PinGod.EditorPlugins
{
    /// <summary>
    /// Basic credit Label to listen for Coin events to update the scene <para/>
    /// CreditsLayer.tscn
    /// </summary>
    public partial class CreditsLabel : Label
    {
        int _credits = 0;
        private IPinGodGame pingod;
        public override void _EnterTree()
        {
            base._EnterTree();

        }

        public override void _ExitTree()
        {
            base._ExitTree();
            if (pingod != null)
            {
                (pingod as PinGodBase).CreditAdded -= UpdateCredits;
                (pingod as PinGodBase).PlayerAdded -= OnPlayerAdded;
            }
        }

        /// <summary>
        /// Connects to CreditAdded and PlayerAdded signals to <see cref="UpdateCredits"/>
        /// </summary>
        public override void _Ready()
        {
            //update when the credit changes, when added and players added
            if (HasNode("/root/PinGodGame"))
            {
                pingod = GetNode("/root/PinGodGame") as IPinGodGame;
                if (pingod != null)
                {
                    (pingod as PinGodBase).CreditAdded += UpdateCredits;
                    (pingod as PinGodBase).PlayerAdded += OnPlayerAdded;
                }
                else { Logger.WarningRich(nameof(CreditsLabel), ":[color=yellow]", "IPinGodGame wasn't found in root, no coin credit switch handlers were added[/color]"); }
            }

            UpdateCredits(pingod?.Audits.Credits ?? 0);
        }

        /// <summary>
        /// Updates text with credits
        /// </summary>
        /// <param name="credits">use credits 0 just to update score</param>
        public void UpdateCredits(int credits = 0)
        {
            if (pingod != null)
            {
                if (credits > 0)
                    _credits = pingod?.Audits?.Credits ?? 0;
            }
            this.Text = $"{_credits} {Tr("CREDITS")}";
        }

        private void OnPlayerAdded()
        {
            UpdateCredits(0);
        }
    }

}
