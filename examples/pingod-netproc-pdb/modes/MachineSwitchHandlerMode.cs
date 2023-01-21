using NetProc.Domain;
using PinGod.Core;
using PinGod.EditorPlugins;
using System;

/// <summary>
/// P-ROC Mode that handles all the door switches and coins in the machine.
/// </summary>
public class MachineSwitchHandlerMode : PinGodProcMode
{
    private string[] _doorSwitches;
    private PinGodGameProc _pinGodProc;    
    private Godot.Label _creditsLabel;

    public MachineSwitchHandlerMode(IGameController game, IPinGodGame pinGod, string name = nameof(MachineSwitchHandlerMode), int priority = 80, string defaultScene = null, bool loadDefaultScene = true) : 
        base(game, name, priority, pinGod, defaultScene, loadDefaultScene)
    {
        //get all switches tagged as 'door' and add a AddSwitchHandler to invoke HandleDoorSwitch
        _doorSwitches = Game.Config.GetNamesFromTag("door", MachineItemType.Switch);
        if(_doorSwitches?.Length > 0)
        {
            for (int i = 0; i < _doorSwitches.Length; i++)
                AddSwitchHandler(_doorSwitches[i], SwitchHandleType.closed, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
        }
        else { Game.Logger.Log("WARN: no door switches found.", NetProc.Domain.PinProc.LogLevel.Warning); }

        //PingodGame p-roc, use to get hold of the machine so we can add credits.
        _pinGodProc = pinGod as PinGodGameProc;        
    }

    public override void ModeStarted()
    {
        base.ModeStarted();
        Delay(nameof(UpdateCredits), NetProc.Domain.PinProc.EventType.None, 0.3, new AnonDelayedHandler(UpdateCredits));
    }

    internal void UpdateCredits()
    {
        UpdateCredits(0);
    }

    bool HandleDoorSwitch(NetProc.Domain.Switch sw)
    {        
        switch (sw.Name)
        {
            case "down":
            case "enter":
            case "exit":
            case "up":
                if (Game.Switches["coinDoor"].IsClosed())
                {
                    //todo: should run the service menu?
                }
                else 
                { 
                    Game.Logger.Log("coin door isn't open.", NetProc.Domain.PinProc.LogLevel.Debug); 
                }
            break;
            case "coinDoor":
            case "coin1":
                UpdateCredits(1);
                break;
            case "coin2":
                UpdateCredits(2);
                break;
            case "coin3":
                UpdateCredits(3);
                //Game.Logger.Log("door switch:" + sw.Name);
                break;
            default:
                break;
        }
        return SWITCH_CONTINUE;
    }

    /// <summary>
    /// Increment the local <see cref="Credits"/>, update the database lookup, then update the view <see cref="CreditsLabel"/>
    /// </summary>
    /// <param name="amt"></param>
    private void UpdateCredits(int amt = 0)
    {
        _pinGodProc?.AddCredits((byte)amt);

        //Update the attract layer credits, no game is in play
        if (!_pinGodProc.GameInPlay)
        {
            if (_creditsLabel == null)
            {
                if (_pinGodProc != null)
                {
                    //The modes are added to a CanvasLayer (Modes), then the mode is a new CanvasLayer named by the P-ROC mode.
                    //Then access the controls name. In this case Attract is a control in the scene and AttactMode was added by P-ROC
                    var attract = _pinGodProc.GetNodeOrNull("/root/ProcScene/Modes/AttractMode/Attract");
                    if (attract != null)
                    {
                        _creditsLabel = attract.GetNodeOrNull<Godot.Label>("Credits");
                    }
                }
                else return;
            }

            _creditsLabel?.Set("text", $"CREDITS: {_pinGodProc.Credits}"); //TODO: use the Tr function from Godot to translate
        }        
    }
}
