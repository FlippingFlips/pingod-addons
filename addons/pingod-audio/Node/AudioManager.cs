using Godot;
using PinGod.Core;
using PinGod;
using System.Collections.Generic;

/// <summary>
/// Helper class for audio. <para/>
/// note: if you want the finished signal on audio to work when the file is an ogg, the loop must be unchecked then reimported from the import tab
/// </summary>
public partial class AudioManager : Node, IAudioManager
{
    #region Exports

    /// <summary> Collection of voice assets to load on startup</summary>
    [Export] public Godot.Collections.Dictionary<string, string> VoiceAssets = new Godot.Collections.Dictionary<string, string>();

    /// <summary>Background music key</summary>
    [Export] public string Bgm { get; set; }

    /// <summary>Collection of assets to load on startup</summary>
    [Export] public Godot.Collections.Dictionary<string, string> MusicAssets { get; set; }

    /// <summary>Assets dictionary with some default sounds</summary>
    [Export]
    public Godot.Collections.Dictionary<string, string> SfxAssets { get; private set; } = new Godot.Collections.Dictionary<string, string>() {
        { "credit" , Paths.PINGOD_ASSETS+"audio/sfx/credit.wav"},
        { "tilt" , Paths.PINGOD_ASSETS+"audio/sfx/tilt.wav"},
        { "warning" , Paths.PINGOD_ASSETS+"audio/sfx/tilt_warning.wav"}};

    [Export] public bool MusicEnabled { get; set; }

    [Export] public bool SfxEnabled { get; set; }

    [Export] public bool VoiceEnabled { get; set; }

    #endregion

    public string CurrentMusic { get; set; }

    #region Audio Stream Dictionaries
    public Dictionary<string, AudioStream> Music { get; private set; }
    public Dictionary<string, AudioStream> Sfx { get; private set; }
    public Dictionary<string, AudioStream> Voice { get; private set; }
    #endregion

    #region Audio Stream Players
    public AudioStreamPlayer MusicPlayer { get; private set; }
    public AudioStreamPlayer SfxPlayer { get; private set; }
    public AudioStreamPlayer VoicePlayer { get; private set; } 
    #endregion

    /// <summary>
    /// Initializes the AudioStreamPlayers. Loads sound pack resources and adds any assets found into the dictionaries
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
            //create players and add to tree
            MusicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
            SfxPlayer = GetNode<AudioStreamPlayer>("SfxPlayer");
            VoicePlayer = GetNode<AudioStreamPlayer>("VoicePlayer");

            Music = new Dictionary<string, AudioStream>();
            Sfx = new Dictionary<string, AudioStream>();
            Voice = new Dictionary<string, AudioStream>();

            foreach (var sfx in SfxAssets)
            {
                AddSfx(sfx.Value, sfx.Key);
            }

            foreach (var vox in VoiceAssets)
            {
                AddVoice(vox.Value, vox.Key);
            }

            if (MusicAssets?.Count > 0)
            {
                foreach (var music in MusicAssets)
                {
                    AddMusic(music.Value, music.Key);
                }
            }
        }
    }

    #region IAudioManager Methods
    /// <inheritdoc/>
    public void AddMusic(string resource, string key)
    {
        if (!Music.ContainsKey(key))
        {
            var stream = GD.Load(resource) as AudioStream;
            if (stream != null)
            {
                Music.Add(key, stream);
                Logger.Debug(nameof(AudioManager), nameof(AddMusic), $": {key},path:{resource}");
            }
            else { Logger.Error(nameof(AudioManager), nameof(AddMusic), $": failed:{key},path:{resource}"); }
        }
    }

    /// <inheritdoc/>
    public void AddSfx(string resource, string key)
    {
        if (!Sfx.ContainsKey(key))
        {
            var stream = GD.Load(resource) as AudioStream;
            if (stream != null)
            {
                Sfx.Add(key, stream);
                Logger.Debug(nameof(AudioManager), nameof(AddSfx), $": {key},path:{resource}");
            }
            else { Logger.Error(nameof(AudioManager), nameof(AddSfx), $": failed:{key},path:{resource}"); }
        }
    }

    /// <inheritdoc/>
    public void AddVoice(string resource, string key)
    {
        if (!Voice.ContainsKey(key))
        {
            var stream = GD.Load(resource) as AudioStream;
            if (stream != null)
            {
                Voice.Add(key, stream);
                Logger.Debug(nameof(AudioManager), nameof(AddVoice), $": {key},path:{resource}");
            }
            else { Logger.Error(nameof(AudioManager), nameof(AddVoice), $": failed:{key},path:{resource}"); }
        }
    }

    /// <inheritdoc/>
    public virtual void MusicPlayer_finished() =>
        Logger.Debug($"{MusicPlayer.Stream?.ResourceName} - music player finished");

    /// <inheritdoc/>
    public void PauseMusic(bool paused) => MusicPlayer.StreamPaused = paused;

    /// <inheritdoc/>    
    public void PlayMusic(string name, float pos = 0f)
    {
        if (string.IsNullOrWhiteSpace(name) || !MusicEnabled || Music == null) return;
        if (!Music.ContainsKey(name))
            Logger.Warning(nameof(AudioManager), $":play music: '{name}' not found");
        else
        {
            Logger.Debug("playing music:", name);
            CurrentMusic = name;
            MusicPlayer.Stream = Music[name];
            MusicPlayer.Play(pos);
            MusicPlayer.Playing = true;
        }
    }

    /// <inheritdoc/>    
    public void PlayMusic(AudioStream audio, float pos = 0f)
    {
        if (audio == null || !MusicEnabled || Music == null) return;

        MusicPlayer.Stream = audio;
        var name = audio.ResourceName;
        CurrentMusic = name;
        MusicPlayer.Play(pos);
        MusicPlayer.Playing = true;
        Logger.Debug("playing music stream: ", name);
    }

    /// <inheritdoc/>
    public void PlaySfx(string name, string bus = "Sfx")
    {
        if (!SfxEnabled || string.IsNullOrWhiteSpace(name) || Sfx == null) return;

        if (!Sfx.ContainsKey(name))
            Logger.Warning($"play sfx: '{name}' not found");
        else
        {
            SfxPlayer.Stream = Sfx[name];
            SfxPlayer.Bus = bus;
            SfxPlayer.Play();
        }
    }

    /// <inheritdoc/>
    public void PlayVoice(string name, string bus = "Voice")
    {
        if (!VoiceEnabled || string.IsNullOrWhiteSpace(name) || Voice == null) return;

        if (!Voice.ContainsKey(name))
            Logger.Warning(nameof(AudioManager), $":play voice: '{name}' not found");
        else
        {
            PlayVoice(Voice[name], bus);
        }
    }

    /// <inheritdoc/>    
    public void PlayVoice(AudioStream voice, string bus = "Voice")
    {
        if (!VoiceEnabled || Voice == null) return;

        VoicePlayer.Bus = bus;
        VoicePlayer.Stream = voice;
        VoicePlayer.Play();
    }

    /// <inheritdoc/>        
    public void SetBgm(string name) => Bgm = name;

    /// <inheritdoc/>            
    public float StopMusic()
    {
        var lastPos = MusicPlayer.GetPlaybackPosition();
        MusicPlayer.Stop();
        return lastPos;
    } 
    #endregion

    /// <summary>
    /// Get the current <see cref="MusicPlayer"/> stream
    /// </summary>
    /// <returns></returns>
    internal AudioStream GetCurrentMusic() => MusicPlayer.Stream;

    /// <summary>
    /// Returns true if the <see cref="MusicPlayer"/> <see cref="AudioStreamPlayer.Playing"/>
    /// </summary>
    /// <returns></returns>
    internal bool IsMusicPlaying() => MusicPlayer.Playing;

    /// <summary>
    /// Plays the BGM stream name if set in <see cref="Bgm"/>
    /// </summary>
    /// <param name="pos"></param>
    internal void PlayBgm(float pos = 0)
    {
        if (!string.IsNullOrWhiteSpace(Bgm))
            PlayMusic(Bgm, pos);
    }

    /// <summary>
    /// Set bus volume. 1 = music
    /// </summary>
    /// <param name="busId"></param>
    /// <param name="musicVolume"></param>
    internal void SetMusicVolume(int busId, float musicVolume) => Godot.AudioServer.SetBusVolumeDb(busId, musicVolume);
}
