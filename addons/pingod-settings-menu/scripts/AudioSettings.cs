using Godot;

/// <summary>
/// Settings Menu for music, sfx, voice
/// </summary>
public partial class AudioSettings : MarginContainer
{
    private PinGodGame pinGod;
    private HSlider _volMusSlider;
    private Label _volMusLabel;
    private CheckButton _musicCheck;
    private HSlider _volSfxSlider;
    private Label _volSfxLabel;
    private CheckButton sfxCheck;
    private HSlider _volVoiceSlider;
    private Label _volVoiceLabel;
    private CheckButton _voiceCheck;
    private HSlider _volMasterSlider;
    private Label _volMasterLabel;

    /// <summary>
    /// Sets up the different types of audio for settings to be changed in a menu
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
            base._EnterTree();

            if (HasNode("/root/PinGodGame"))
            {
                pinGod = GetNode<PinGodGame>("/root/PinGodGame");
                SetupMaster();
                SetupSfx();
                SetupMusic();
                SetupVoice();
            }
        }
    }

    private void SetupMaster()
    {
        _volMasterSlider = GetNode<HSlider>("VBoxContainer/MasterContainer/HSlider");
        _volMasterSlider.Value = pinGod?.Adjustments?.MasterVolume ?? 0;

        _volMasterLabel = GetNode<Label>("VBoxContainer/MasterContainer/Label");
        _volMasterLabel.Text = $"{pinGod?.Adjustments?.MasterVolume}";
    }

    private void SetupVoice()
    {
        _volVoiceSlider = GetNode<HSlider>("VBoxContainer/VoiceContainer/HSlider");
        _volVoiceSlider.Value = pinGod?.Adjustments?.VoiceVolume ?? 0;

        _volVoiceLabel = GetNode<Label>("VBoxContainer/VoiceContainer/Label");
        _volVoiceLabel.Text = $"{pinGod?.Adjustments.VoiceVolume}";

        _voiceCheck = GetNode<CheckButton>("VBoxContainer/VoiceContainer/CheckButton");
        _voiceCheck.SetPressedNoSignal(pinGod?.Adjustments?.VoiceEnabled ?? false);
    }

    private void SetupSfx()
    {
        _volSfxSlider = GetNode<HSlider>("VBoxContainer/SfxContainer/HSlider");
        _volSfxSlider.Value = pinGod?.Adjustments?.SfxVolume ?? 0;

        _volSfxLabel = GetNode<Label>("VBoxContainer/SfxContainer/Label");
        _volSfxLabel.Text = $"{pinGod?.Adjustments?.SfxVolume ?? 0}";

        sfxCheck = GetNode<CheckButton>("VBoxContainer/SfxContainer/CheckButton");
        sfxCheck.SetPressedNoSignal(pinGod?.Adjustments?.SfxEnabled ?? false);
    }

    private void SetupMusic()
    {
        _volMusSlider = GetNode<HSlider>("VBoxContainer/MusicContainer/HSlider");
        _volMusSlider.Value = pinGod?.Adjustments.MusicVolume ?? 0;

        _volMusLabel = GetNode<Label>("VBoxContainer/MusicContainer/Label");
        _volMusLabel.Text = $"{pinGod?.Adjustments.MusicVolume}";

        _musicCheck = GetNode<CheckButton>("VBoxContainer/MusicContainer/CheckButton");
        _musicCheck.SetPressedNoSignal(pinGod?.Adjustments?.MusicEnabled ?? false);
    }

    void _on_VolumeSliderMaster_value_changed(float val)
    {
        if (pinGod != null) pinGod.Adjustments.MasterVolume = val;
        _volMasterLabel.Text = val > 0 ? $"+{val}" : val.ToString();
        AudioServer.SetBusVolumeDb(0, val);
    }

    void _on_VolumeSliderMusic_value_changed(float val)
    {
        if (pinGod != null) pinGod.Adjustments.MusicVolume = val;
        _volMusLabel.Text = val > 0 ? $"+{val}" : val.ToString();
        AudioServer.SetBusVolumeDb(1, val);
    }

    void _on_VolumeSliderSfx_value_changed(float val)
    {
        if (pinGod != null) pinGod.Adjustments.SfxVolume = val;
        _volSfxLabel.Text = val > 0 ? $"+{val}" : val.ToString();
        AudioServer.SetBusVolumeDb(2, val);
    }

    void _on_VolumeSliderVoice_value_changed(float val)
    {
        if (pinGod != null) pinGod.Adjustments.VoiceVolume = val;
        _volVoiceLabel.Text = val > 0 ? $"+{val}" : val.ToString();
        AudioServer.SetBusVolumeDb(3, val);
    }

    void _on_CheckButtonMusic_toggled(bool pressed) 
    {
        if (pinGod != null) pinGod.Adjustments.MusicEnabled = pressed;
    }
    void _on_CheckButtonSfx_toggled(bool pressed)
    {
        if (pinGod != null) pinGod.Adjustments.SfxEnabled = pressed;
    }
    void _on_CheckButtonVoice_toggled(bool pressed)
    {
        if (pinGod != null) pinGod.Adjustments.VoiceEnabled = pressed;
    }
}
