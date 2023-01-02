using Godot;
using System;

/// <summary>
/// Basic credit Label to listen for Coin events to update the scene <para/>
/// CreditsLayer.tscn
/// </summary>
public partial class CreditsLayer : Label
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
            pingod.CreditAdded += OnCreditsUpdated;
            pingod.PlayerAdded += OnPlayerAdded;
        }
    }

    /// <summary>
    /// Connects to CreditAdded and PlayerAdded signals to <see cref="OnCreditsUpdated"/>
    /// </summary>
    public override void _Ready()
    {
        OnCreditsUpdated(0);
    }

    /// <summary>
    /// Updates text with credits
    /// </summary>
    /// <param name="credits">use credits 0 just to update score</param>
    public virtual void OnCreditsUpdated(int credits = 0)
    {
        if (credits > 0)
            _credits = credits;
        this.Text = $"{_credits} {Tr("CREDITS")}";
    }

    private void OnPlayerAdded()
    {
        OnCreditsUpdated(0);
    }
}
