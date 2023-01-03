using Godot;
using System;

/// <summary>
/// Basic credit Label to listen for Coin events to update the scene <para/>
/// CreditsLayer.tscn
/// </summary>
public partial class CreditsLabel : Label
{
    int _credits = 0;
    private PinGodGame pingod;
    public override void _EnterTree()
    {
        base._EnterTree();        
        //update when the credit changes, when added and players added
        if (HasNode("/root/PinGodGame"))
        {
            pingod = GetNode("/root/PinGodGame") as PinGodGame;
            if (pingod != null)
            {
                pingod.CreditAdded += UpdateCredits;
                pingod.PlayerAdded += OnPlayerAdded;
            }
            else { Logger.Debug(nameof(CreditsLabel),": pingod game not found"); }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (pingod != null)
        {
            pingod.CreditAdded-=UpdateCredits;
            pingod.PlayerAdded -= OnPlayerAdded;
        }
    }

    /// <summary>
    /// Connects to CreditAdded and PlayerAdded signals to <see cref="UpdateCredits"/>
    /// </summary>
    public override void _Ready()
    {
        UpdateCredits(pingod?.GameData.Credits ?? 0);
    }

    /// <summary>
    /// Updates text with credits
    /// </summary>
    /// <param name="credits">use credits 0 just to update score</param>
    public void UpdateCredits(int credits = 0)
    {
        if(pingod != null)
        {
            if (credits > 0)
                _credits = pingod?.GameData?.Credits ?? 0;
        }        
        this.Text = $"{_credits} {Tr("CREDITS")}";
    }

    private void OnPlayerAdded()
    {
        UpdateCredits(0);
    }
}
