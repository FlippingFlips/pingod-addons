using Godot;
using PinGod.Core.Interface;
using System.Collections.Generic;

namespace PinGod.Core.Service
{
    /// <summary>
    /// Helper class for audio. <para/>
    /// note: if you want the finished signal on audio to work when the file is an ogg, the loop must be unchecked then reimported from the import tab
    /// </summary>
    public partial class AudioManager : Node, IAudioManager
    {
        #region Exports

        /// <summary>
        /// Background music key
        /// </summary>
        [Export] public string Bgm { get; set; }

        /// <summary>
        /// Collection of assets to load on startup
        /// </summary>
        [Export] public Godot.Collections.Dictionary<string, string> MusicAssets { get; set; }

        /// <summary>
        /// Assets dictionary with some default sounds
        /// </summary>
        [Export]
        public Godot.Collections.Dictionary<string, string> SfxAssets { get; private set; } = new Godot.Collections.Dictionary<string, string>() {
        { "credit" , "res://addons/assets/audio/sfx/credit.wav"},
        { "tilt" , "res://addons/assets/audio/sfx/tilt.wav"},
        { "warning" , "res://addons/assets/audio/sfx/tilt_warning.wav"}
    };

        /// <summary>
        /// Collection of voice assets to load on startup
        /// </summary>
        [Export] public Godot.Collections.Dictionary<string, string> VoiceAssets = new Godot.Collections.Dictionary<string, string>();

        #endregion


        public string CurrentMusic { get; set; }
        public Dictionary<string, AudioStream> Music { get; private set; }
        [Export]        
        public bool MusicEnabled { get; set; }        
        public AudioStreamPlayer MusicPlayer { get; private set; }        
        public Dictionary<string, AudioStream> Sfx { get; private set; }
        public bool SfxEnabled { get; set; }
        public AudioStreamPlayer SfxPlayer { get; private set; }
        /// <summary>
        /// Voice sound resources
        /// </summary>
        public Dictionary<string, AudioStream> Voice { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public bool VoiceEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AudioStreamPlayer VoicePlayer { get; private set; }

        /// <summary>
        /// Initializes the AudioStreamPlayers. Loads sound pack resources and adds any assets found into the dictionaries
        /// </summary>
        public override void _EnterTree()
        {
            if (!Engine.IsEditorHint())
            {
                //create players and add to tree
                MusicPlayer = new();
                SfxPlayer = new();
                VoicePlayer = new();
                AddChild(MusicPlayer);
                AddChild(SfxPlayer);
                AddChild(VoicePlayer);

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

        /// <summary>
        /// Loads and adds a music resource to the <see cref="Music"/> dictionary
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="key"></param>
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

        /// <summary>
        /// Loads and adds a sfx resource stream to the <see cref="Sfx"/> dictionary
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="key"></param>
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

        /// <summary>
        /// Loads and adds a voice resource stream to the <see cref="Voice"/> dictionary
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="key"></param>
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

        /// <summary>
        /// Just logs debug music player finished
        /// </summary>
        public void MusicPlayer_finished()
        {
            Logger.Debug($"{MusicPlayer.Stream?.ResourceName} - music player finished");
        }

        /// <summary>
        /// Pauses the stream loaded into the <see cref="MusicPlayer"/>
        /// </summary>
        /// <param name="paused"></param>
        public void PauseMusic(bool paused) => MusicPlayer.StreamPaused = paused;

        /// <summary>
        /// Sets the <see cref="MusicPlayer"/> stream and plays the music from name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
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

        /// <summary>
        /// Sets the <see cref="MusicPlayer"/> stream and plays the music from stream
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="pos"></param>
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

        /// <summary>
        /// Plays sfx on the <see cref="SfxPlayer"/> with optional bus
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bus"></param>
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

        /// <summary>
        /// Plays a voice from name on the <see cref="VoicePlayer"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bus"></param>
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

        /// <summary>
        /// Plays a voice from stream on the <see cref="VoicePlayer"/>
        /// </summary>
        /// <param name="voice"></param>
        /// <param name="bus"></param>
        public void PlayVoice(AudioStream voice, string bus = "Voice")
        {
            if (!VoiceEnabled || Voice == null) return;

            VoicePlayer.Bus = bus;
            VoicePlayer.Stream = voice;
            VoicePlayer.Play();
        }

        /// <summary>
        /// Set the BGM flag. Background Music
        /// </summary>
        /// <param name="name"></param>
        public void SetBgm(string name) => Bgm = name;

        /// <summary>
        /// Stops any music playing
        /// </summary>
        /// <returns>The position in secs where stopped</returns>
        public float StopMusic()
        {
            var lastPos = MusicPlayer.GetPlaybackPosition();
            MusicPlayer.Stop();
            return lastPos;
        }

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
}
