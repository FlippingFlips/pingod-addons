using Godot;
using MoonStation.game;
using PinGod.Core;

public partial class SongOptionButton : OptionButton
{
    private MsPinGodGame pinGod;
    private MsGameSettings mStationSettings;

    public override void _EnterTree()
    {
        base._EnterTree();

        this.AddItem("off", 0);
        this.AddItem("techno", 1);
        this.AddItem("dnb", 2);
    }

    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            pinGod = GetNodeOrNull<IPinGodGame>("/root/PinGodGame") as MsPinGodGame;
            if(pinGod != null)
            {
                mStationSettings = pinGod.Adjustments as MsGameSettings;
                if (mStationSettings != null)
                {
                    if (mStationSettings.Music == "off")
                    {
                        Selected = 0;
                        pinGod.SetMusicOff();
                    }
                    else
                    {
                        if (mStationSettings.Music == "dnb")
                            Selected = 2;
                        else
                            Selected = 1;

                        pinGod.SetMusicOn(mStationSettings.Music);
                    }
                }
                else { Logger.Warning(nameof(SongOptionButton), ": couldn't find ms music settings"); }
            }            
        }
    }


    void _on_OptionButton_item_selected(int index)
    {
        if (!Engine.IsEditorHint())
        {
            mStationSettings = pinGod.Adjustments as MsGameSettings;
            string music = GetItemText(index);
            mStationSettings.Music = music;
            if (index > 0)
            {                
                pinGod.LogInfo($"selected {index} " + music);
                pinGod?.SetMusicOn(music);
                mStationSettings.MusicEnabled = true;
            }
            else
            {
                pinGod?.SetMusicOff();
                mStationSettings.MusicEnabled = false;
            }                
        }
    }
}
