using NetProc.Domain;
using PinGod.Core;
using PinGod.EditorPlugins;

/// <summary>
/// P-ROC Mode that handles all the door switches and coins in the machine.
/// </summary>
public class MachineSwitchHandlerMode : PinGodProcMode
{
    private string[] _doorSwitches;
    private PinGodGameProc _pinGodProc;    
    private CreditsLabel _creditsLabel;

    public MachineSwitchHandlerMode(IGameController game, IPinGodGame pinGod, int priority = 80, string defaultScene = null, bool loadDefaultScene = true) : 
        base(game, priority, pinGod, defaultScene, loadDefaultScene)
    {
        //get all switches tagged as 'door' and add a AddSwitchHandler to invoke HandleDoorSwitch
        _doorSwitches = Game.Config.GetNamesFromTag("door", MachineItemType.Switch);
        if(_doorSwitches?.Length > 0)
        {
            for (int i = 0; i < _doorSwitches.Length; i++)
                AddSwitchHandler(_doorSwitches[i], SwitchHandleType.active, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
        }
        else { Game.Logger.Log("WARN: no door switches found.", NetProc.Domain.PinProc.LogLevel.Warning); }

        //PingodGame p-roc, use to get hold of the machine so we can add credits.
        _pinGodProc = pinGod as PinGodGameProc;
        if(_pinGodProc != null)
        {            
            //get the procscene
            var pScene = _pinGodProc.GetTree().Root.GetNodeOrNull("ProcScene");
            if (pScene != null)
            {
                _creditsLabel = pScene.GetNodeOrNull<CreditsLabel>("Control/Credits");
            }
        }
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

                }
                else { Game.Logger.Log("coin door not open", NetProc.Domain.PinProc.LogLevel.Debug); }
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
    private void UpdateCredits(int amt)
    {
        _pinGodProc.AddCredits((byte)amt);
        //_creditsLabel?.UpdateCredits(_pinGodProc.Credits);
    }
}
