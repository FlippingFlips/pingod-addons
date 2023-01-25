using Godot;
using PinGod.Core;
using PinGod.Game;
using System.Collections.Generic;
using System.Linq;
using static Godot.GD;
using static MoonStation.GameGlobals;

/// <summary>
/// Both drop target banks increase the multiplier
/// </summary>
public partial class DropTargets : Control
{
    private MsGame game;
    private IPinGodGame pinGod;
    private AudioStreamPlayer voicePlayer;
    private Dictionary<string, AudioStream> voices;

	public override void _EnterTree()
	{
		//player to play voices with. WAV so they don't loop automatically
		voicePlayer = GetNode("AudioStreamPlayer") as AudioStreamPlayer;

		//load all voices to dict
		voices = new Dictionary<string, AudioStream>();
		var vDir = "res://assets/audio/voice";
		var chars = new string[] { "m", "o", "n", "s", "t", "a", "i" };
		for (int i = 0; i < chars.Length; i++)
		{
			voices.Add(chars[i], Load(vDir + $"/{chars[i]}.wav") as AudioStream);
		}
		
        game = GetNodeOrNull("/root/MainScene/Modes/Game") as MsGame;
    }

    public override void _Ready()
    {
        Logger.Debug(nameof(DropTargets), ":drop targets scene _Ready, resetting targets");

        pinGod = GetNodeOrNull<IPinGodGame>("/root/PinGodGame");
        if (pinGod == null)
        {
            Logger.WarningRich(nameof(DropTargets), ":", "[color=yellow]", "PinGodGame not found", "[/color]");
        }

        if (pinGod?.MachineNode != null)
        {
            var pg = pinGod as PinGodGame;
            pg.Connect("BallEnded", new Callable(this, "ResetTargets"));
            pinGod.MachineNode.SwitchCommand += SwitchCommandHandler;
        }

        ResetTargets(false);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (pinGod?.MachineNode != null)
            pinGod.MachineNode.SwitchCommand -= SwitchCommandHandler;
    }

    public void ResetTargets(bool lastBall)
    {
        (pinGod as MsPinGodGame).Multiplier = 1;
        ResetMoon();
        ResetStation();
        game.UpdateLamps();
    }

    /// <summary>
    /// Each time Moon target is hit, run a check if all complete to increase multiplier
    /// </summary>
    /// <returns></returns>
    private bool MoonCheck()
    {
        pinGod.AddPoints(SMALL_SCORE);

        if (!game.MoonTargets.Any(x => x == 0))
        {
            pinGod.LogInfo("Moon drops completed. PF multiplier added");
            (pinGod as MsPinGodGame).Multiplier++;
            game.UpdateLamps();
            ResetMoon();
            return true;
        }
        return false;
    }

    /// <summary>
    /// plays letter of target voice, if voice enabled
    /// </summary>
    /// <param name="letter"></param>
    private void Playsound(string letter)
    {
        if (pinGod?.Adjustments?.VoiceEnabled ?? false)
        {
            voicePlayer.Stream = voices[letter];
            voicePlayer.Play();
        }
    }

    private void ResetMoon()
    {
        game.MoonTargets = new byte[4] { 0, 0, 0, 0 };
        pinGod.SolenoidPulse("drops_l"); // reset moon Drops
    }

    private void ResetStation()
    {
        pinGod.SolenoidPulse("drops_r"); // reset station Drops
        game.StationTargets = new byte[7] { 0, 0, 0, 0, 0, 0, 0 };
    }

    private void SetMoonTarget(int index, string letter)
    {
        pinGod.LogDebug(nameof(DropTargets), ":moon target: " + letter);
        game.MoonTargets[index] = 1;
        Playsound(letter);
        MoonCheck();
    }

    private void SetStationTarget(int index, string letter)
    {
        pinGod.LogDebug("sw: " + letter);
        game.StationTargets[index] = 1;
        Playsound(letter);
        StationCheck();
    }

    /// <summary>
    /// Each time a Station target is hit, run a check if all complete to increase multiplier
    /// </summary>
    /// <returns></returns>
    private bool StationCheck()
    {
        pinGod.AddPoints(SMALL_SCORE);

        if (!game.StationTargets.Any(x => x == 0))
        {
            pinGod.LogInfo("Station drops completed. PF multiplier added");
            (pinGod as MsPinGodGame).Multiplier++;
            game.UpdateLamps();
            ResetStation();
            return true;
        }

        return false;
    }

    /// <summary>
    /// MOn switch on set <see cref="SetMoonTarget"/> and <see cref="SetStationTarget(int, string)"/>
    /// </summary>
    /// <param name="swName"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void SwitchCommandHandler(string swName, byte index, byte value)
    {
        if (value > 0 && pinGod.GameInPlay && !pinGod.IsTilted)
        {
            switch (swName)
            {
                case "Moon":
                    SetMoonTarget(0, "m");
                    break;
                case "mOon":
                    SetMoonTarget(1, "o");
                    break;
                case "moOn":
                    SetMoonTarget(2, "o");
                    break;
                case "mooN":
                    SetMoonTarget(3, "n");
                    break;
                case "Station":
                    SetStationTarget(0, "s");
                    break;
                case "sTation":
                    SetStationTarget(1, "t");
                    break;
                case "stAtion":
                    SetStationTarget(2, "a");
                    break;
                case "staTion":
                    SetStationTarget(3, "t");
                    break;
                case "statIon":
                    SetStationTarget(4, "i");
                    break;
                case "statiOn":
                    SetStationTarget(5, "o");
                    break;
                case "statioN":
                    SetStationTarget(6, "n");
                    break;
                default:
                    break;
            }
        }
    }
}
