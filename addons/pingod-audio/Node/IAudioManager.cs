using Godot;

public interface IAudioManager
{
    /// <summary>Background Music Default</summary>
    string Bgm { get; set; }

    /// <summary>Current music stream name</summary>
    string CurrentMusic { get; set; }

    /// <summary> Collection of music audiostream resources</summary>
    System.Collections.Generic.Dictionary<string, AudioStream> Music { get; }

    /// <summary>Background Music Default</summary>
    Godot.Collections.Dictionary<string, string> MusicAssets { get; set; }

    /// <summary> Disable / Enable music</summary>
    bool MusicEnabled { get; set; }

    /// <summary> Music player </summary>
    AudioStreamPlayer MusicPlayer { get; }

    /// <summary> Sound FX resources</summary>
    System.Collections.Generic.Dictionary<string, AudioStream> Sfx { get; }

    Godot.Collections.Dictionary<string, string> SfxAssets { get; }

    bool SfxEnabled { get; set; }

    AudioStreamPlayer SfxPlayer { get; }

    System.Collections.Generic.Dictionary<string, AudioStream> Voice { get; }

    bool VoiceEnabled { get; set; }

    AudioStreamPlayer VoicePlayer { get; }

    void _EnterTree();

    /// <summary> Loads and adds a music resource to the <see cref="Music"/> dictionary</summary>
    /// <param name="resource"></param>
    /// <param name="key"></param>
    void AddMusic(string resource, string key);

    /// <summary> Loads and adds a sfx resource stream to the <see cref="Sfx"/> dictionary</summary>
    /// <param name="resource"></param>
    /// <param name="key"></param>
    void AddSfx(string resource, string key);

    /// <summary>Loads and adds a voice resource stream to the <see cref="Voice"/> dictionary</summary>
    /// <param name="resource"></param>
    /// <param name="key"></param>
    void AddVoice(string resource, string key);

    /// <summary>Just logs debug music player finished. Override</summary>
    void MusicPlayer_finished();

    /// <summary> Pauses the stream loaded into the <see cref="MusicPlayer"/></summary>
    /// <param name="paused"></param>
    void PauseMusic(bool paused);

    /// <summary>Sets the <see cref="MusicPlayer"/> stream and plays the music from name</summary>
    /// <param name="name"></param>
    /// <param name="pos"></param>
    void PlayMusic(AudioStream audio, float pos = 0);

    /// <summary>Sets the <see cref="MusicPlayer"/> stream and plays the music from stream</summary>
    /// <param name="audio"></param>
    /// <param name="pos"></param>
    void PlayMusic(string name, float pos = 0);

    /// <summary>Plays sfx on the <see cref="SfxPlayer"/> with optional bus</summary>
    /// <param name="name"></param>
    /// <param name="bus"></param>
    void PlaySfx(string name, string bus = "Sfx");

    /// <summary>Plays a voice from name on the <see cref="VoicePlayer"/></summary>
    /// <param name="name"></param>
    /// <param name="bus"></param>
    void PlayVoice(AudioStream voice, string bus = "Voice");

    /// <summary>Plays a voice from stream on the <see cref="VoicePlayer"/></summary>
    /// <param name="voice"></param>
    /// <param name="bus"></param>
    void PlayVoice(string name, string bus = "Voice");

    /// <summary>Set the BGM flag. Background Music</summary>
    /// <param name="name"></param>
    void SetBgm(string name);

    /// <summary>Stops any music playing</summary>
    /// <returns>The position in secs where stopped</returns>
    float StopMusic();
}