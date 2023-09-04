using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using NetProc.Domain;
using PinGod.Core;
using PinGod.EditorPlugins;
using PinGod.Game;
using System;
using System.Linq;

/// <summary>
/// P-ROC Mode that handles all the door switches and coins in the machine.
/// </summary>
public class MachineSwitchHandlerMode : PinGodProcMode
{
    private string[] _doorSwitches;
    private PinGodGameProc _pinGodProc;
    private Godot.Label _creditsLabel;

    /// <summary>
    /// Mode to run if coin door opened and enter pushed for service
    /// </summary>
    private ServiceMode _serviceMode;

    public MachineSwitchHandlerMode(IGameController game, IPinGodGame pinGod, string name = nameof(MachineSwitchHandlerMode), int priority = 80, string defaultScene = null, bool loadDefaultScene = true) :
        base(game, name, priority, pinGod, defaultScene, loadDefaultScene)
    {
        //get all switches tagged as 'door' and add a AddSwitchHandler to invoke HandleDoorSwitch
        _doorSwitches = Game.Config.GetNamesFromTag("door", MachineItemType.Switch);
        if (_doorSwitches?.Length > 0)
        {
            for (int i = 0; i < _doorSwitches.Length; i++)
            {
                if (_doorSwitches[i] == "coinDoor")
                    AddSwitchHandler(_doorSwitches[i], SwitchHandleType.open, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
                else
                    AddSwitchHandler(_doorSwitches[i], SwitchHandleType.closed, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
            }                
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
                if (Game.Switches["coinDoor"].IsOpen() && _serviceMode == null)
                {
                    Game.Logger.Log("todo: volume down", NetProc.Domain.PinProc.LogLevel.Info);
                }                    
                break;
            case "up":
                //todo: check if the service isn't running as well, this is our volume
                if (Game.Switches["coinDoor"].IsOpen() && _serviceMode == null)
                {
                    //TODO: volume up
                    //bool result = _pinGodProc.PinGodProcGame.IsModeRunning("ServiceMode");                    
                    Game.Logger.Log("todo: volume up ", NetProc.Domain.PinProc.LogLevel.Info);
                }
                break;
            case "enter":
                if (Game.Switches["coinDoor"].IsOpen() && !Game.Modes.Modes.Any(x => x.GetType() == typeof(ServiceMode)))
                {
                    //adds the service mode and remove attract
                    _pinGodProc.CallDeferred("RemoveMode", "attract");
                    _pinGodProc.CallDeferred("AddMode", "service");

                    //Game.Modes.Remove(_pinGodProc.PinGodProcGame.);
                }
                break;           
            case "coinDoor":                
                if (sw.IsOpen())
                {
                    //todo: hardware: cut the power on the ground switch
                    Game.Logger.Log("coinDoor open");
                }
                else
                {
                    Game.Logger.Log("coinDoor closed");
                }
                break;
            case "coin1":
                UpdateCredits(1);
                break;
            case "coin2":
                UpdateCredits(2);
                break;
            case "coin3":
                UpdateCredits(3);
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
        _pinGodProc?.CallDeferred("AddCredits", ((byte)amt));

        _pinGodProc.PinGodProcGame.IncrementAudit("CREDITS", amt);

        //Update the attract layer credits, no game is in play
        //if (!_pinGodProc.GameInPlay)
        //{
        //    if (_creditsLabel == null)
        //    {
        //        if (_pinGodProc != null)
        //        {
        //            Credits
        //            //The modes are added to a CanvasLayer (Modes), then the mode is a new CanvasLayer named by the P-ROC mode.
        //            //Then access the controls name. In this case Attract is a control in the scene and AttactMode was added by P-ROC
        //            var attract = _pinGodProc.GetNodeOrNull("/root/ProcScene/Modes/AttractMode/Attract");
        //            if (attract != null)
        //            {
        //                _creditsLabel = attract.GetNodeOrNull<Godot.Label>("Credits");
        //            }
        //        }
        //        else return;
        //    }

        //    _creditsLabel?.Set("text", $"CREDITS: {_pinGodProc.Credits}"); //TODO: use the Tr function from Godot to translate
        //}
    }
}
