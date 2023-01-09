using Godot;

namespace PinGod.Core.Interface
{
    public interface IAudioManager
    {
        string Bgm { get; set; }
        /// <summary>
        /// Current music stream name
        /// </summary>
        string CurrentMusic { get; set; }
        /// <summary>
        /// Collection of music audiostream resources
        /// </summary>
        System.Collections.Generic.Dictionary<string, AudioStream> Music { get; }
        Godot.Collections.Dictionary<string, string> MusicAssets { get; set; }
        /// <summary>
        /// Disable / Enable music
        /// </summary>
        bool MusicEnabled { get; set; }
        /// <summary>
        /// Music player
        /// </summary>
        AudioStreamPlayer MusicPlayer { get; }
        /// <summary>
        /// Sound FX resources
        /// </summary>
        System.Collections.Generic.Dictionary<string, AudioStream> Sfx { get; }
        Godot.Collections.Dictionary<string, string> SfxAssets { get; }
        bool SfxEnabled { get; set; }
        AudioStreamPlayer SfxPlayer { get; }
        System.Collections.Generic.Dictionary<string, AudioStream> Voice { get; }
        bool VoiceEnabled { get; set; }
        AudioStreamPlayer VoicePlayer { get; }
        void _EnterTree();

        void AddMusic(string resource, string key);
        void AddSfx(string resource, string key);
        void AddVoice(string resource, string key);
        void MusicPlayer_finished();
        void PauseMusic(bool paused);
        void PlayMusic(AudioStream audio, float pos = 0);
        void PlayMusic(string name, float pos = 0);
        void PlaySfx(string name, string bus = "Sfx");
        void PlayVoice(AudioStream voice, string bus = "Voice");
        void PlayVoice(string name, string bus = "Voice");
        void SetBgm(string name);
        float StopMusic();
    }
}