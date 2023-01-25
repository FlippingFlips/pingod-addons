using MoonStation.game;
using PinGod.Core;
using PinGod.Core.Game;
using PinGod.Game;

/// <summary>
/// A custom version of PinGodGame
/// </summary>
public partial class MsPinGodGame : PinGodGame
{
    public int Multiplier { get; set; }

    /// <summary>
    /// Processing is disabled when _resources?.IsLoading() is complete. For first game run. <para/>
    /// The <see cref="OnResourcesLoaded"/> will load the attract mode when completed
    /// </summary>
    /// <param name="_delta"></param>
    public override void _Process(double _delta)
    {
        base._Process(_delta);
        if (_resources != null)
        {
            bool result = _resources?.IsLoading() ?? true;
            if (!result)
            {
                //resources loaded
                SetProcess(false);
                OnResourcesLoaded();
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Adjustments = MsGameSettings.Load<MsGameSettings>();
        var set = Adjustments as MsGameSettings;
        if (set != null)
        {
            var music = set.Music;
            LogInfo("selected music ", music);
            SetMusicOn(music);
        }
    }

    /// <summary>
    /// Override PinGodGames AddPoints to add multiplier to score and also add bonus here. <para/>
    /// </summary>
    /// <param name="points"></param>
    /// <param name="emitUpdateSignal"></param>
    public override long AddPoints(long points, bool emitUpdateSignal = true)
    {
        var totalPoints = points * Multiplier;
        base.AddPoints(points * Multiplier, emitUpdateSignal);
        AddBonus(totalPoints / 5);
        return totalPoints;
    }

    /// <summary>
    /// override to create our own player type for this game
    /// </summary>
    /// <param name="name"></param>
    public override void CreatePlayer(string name)
    {
        Players.Add(new MsPlayer() { Name = name, Points = 0 });
    }

    /// <summary>
    /// Loads the custom <see cref="MsGameData"/> class into our settings
    /// </summary>
    public override void LoadDataFile()
    {
        Audits = PinGod.Core.Service.Audits.Load<MsGameData>();
    }

    /// <summary>
    /// When the resource node is fully loaded we gets the MainScene and load the attract into it
    /// </summary>
    private void OnResourcesLoaded()
    {
        var ms = GetNodeOrNull<MainScene>("/root/MainScene");
        if (ms != null)
        {
            Logger.WarningRich(nameof(MsPinGodGame), ":Resources loaded");
            ms.AddAttract();
        }
        else { Logger.WarningRich(nameof(MsPinGodGame), "[color=yellow] no MainScene found.[/color]"); }
    }

    public override void SaveGameData()
    {
        PinGod.Core.Service.Audits.Save(Audits as MsGameData);
    }

    public override void SaveGameSettings()
    {
        LogInfo("saving MsGameSettings settings");
        Adjustments.Save(Adjustments as MsGameSettings);
    }

    /// <summary>
    /// Sets music to "off"
    /// </summary>
    public void SetMusicOff()
    {
        var settings = Adjustments as MsGameSettings;
        settings.Music = "off";
        AudioManager.Bgm = "off";
    }

    /// <summary>
    /// Sets the games music name and the <see cref="AudioManager.Bgm"/>
    /// </summary>
    /// <param name="menu"></param>
    public void SetMusicOn(string menu)
    {        
        if(!string.IsNullOrWhiteSpace(menu))
        {
            var settings = Adjustments as MsGameSettings;
            settings.Music = menu;
            if (AudioManager != null)
            {
                AudioManager.Bgm = menu;
                LogDebug("selected music", AudioManager.Bgm);
            }
        }        
    }

    /// <summary>
    /// setup music if on
    /// </summary>
    public override void Setup()
    {
        //base setup
        base.Setup();
    }    
}