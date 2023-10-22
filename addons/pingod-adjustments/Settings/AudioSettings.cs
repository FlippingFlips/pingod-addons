using Godot;
using PinGod.Core;
using PinGod.Core.Service;

/// <summary>
/// Settings Menu for music, sfx, voice
/// </summary>
public partial class AudioSettings : MarginContainer
{
	private AdjustmentsNode _adjustments;
	public Adjustments Adjustments;

	#region User Interface
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
	#endregion

	/// <summary>
	/// Sets up the different types of audio for settings to be changed in a menu
	/// </summary>
	public override void _Ready()
	{
		base._Ready();
		if (HasNode("/root/Adjustments"))
		{
			_adjustments = GetNode("/root/Adjustments") as AdjustmentsNode;
			Adjustments = _adjustments._adjustments;
			SetupMaster();
			SetupSfx();
			SetupMusic();
			SetupVoice();
		}
		else
		{
			Logger.WarningRich(nameof(AudioSettings), ":[color=yellow]", "AdjustmentsScript wasn't found in /root/Adjustments. Used for settings menu screen.[/color]");
			this.QueueFree();
		}
	}

	private void SetupMaster()
	{
		_volMasterSlider = GetNode<HSlider>("VBoxContainer/MasterContainer/HSlider");
		_volMasterSlider.Value = Adjustments?.MasterVolume ?? 0;
		AudioServer.SetBusVolumeDb(0, Adjustments.MasterVolume);

		_volMasterLabel = GetNode<Label>("VBoxContainer/MasterContainer/Label");
		_volMasterLabel.Text = $"{Adjustments?.MasterVolume}";
	}

	private void SetupVoice()
	{
		_volVoiceSlider = GetNode<HSlider>("VBoxContainer/VoiceContainer/HSlider");
		_volVoiceSlider.Value = Adjustments?.VoiceVolume ?? 0;

		_volVoiceLabel = GetNode<Label>("VBoxContainer/VoiceContainer/Label");
		_volVoiceLabel.Text = $"{Adjustments.VoiceVolume}";
		AudioServer.SetBusVolumeDb(3, Adjustments.VoiceVolume);

		_voiceCheck = GetNode<CheckButton>("VBoxContainer/VoiceContainer/CheckButton");
		_voiceCheck.SetPressedNoSignal(Adjustments?.VoiceEnabled ?? false);
	}

	private void SetupSfx()
	{
		_volSfxSlider = GetNode<HSlider>("VBoxContainer/SfxContainer/HSlider");
		_volSfxSlider.Value = Adjustments?.SfxVolume ?? 0;

		_volSfxLabel = GetNode<Label>("VBoxContainer/SfxContainer/Label");
		_volSfxLabel.Text = $"{Adjustments?.SfxVolume ?? 0}";
		AudioServer.SetBusVolumeDb(2, Adjustments.SfxVolume);

		sfxCheck = GetNode<CheckButton>("VBoxContainer/SfxContainer/CheckButton");
		sfxCheck.SetPressedNoSignal(Adjustments?.SfxEnabled ?? false);
	}

	private void SetupMusic()
	{
		_volMusSlider = GetNode<HSlider>("VBoxContainer/MusicContainer/HSlider");
		_volMusSlider.Value = Adjustments?.MusicVolume ?? 0;

		_volMusLabel = GetNode<Label>("VBoxContainer/MusicContainer/Label");
		_volMusLabel.Text = $"{Adjustments.MusicVolume}";
		AudioServer.SetBusVolumeDb(1, Adjustments.MusicVolume);

		_musicCheck = GetNode<CheckButton>("VBoxContainer/MusicContainer/CheckButton");
		_musicCheck.SetPressedNoSignal(Adjustments?.MusicEnabled ?? false);
	}

	void _on_VolumeSliderMaster_value_changed(float val)
	{
		if (_volMasterLabel == null) return;
		if (Adjustments != null) Adjustments.MasterVolume = val;
		_volMasterLabel.Text = val > 0 ? $"+{val}" : val.ToString();
		AudioServer.SetBusVolumeDb(0, val);
	}

	void _on_VolumeSliderMusic_value_changed(float val)
	{
		if (_volMusLabel == null) return;
		if (Adjustments != null) Adjustments.MusicVolume = val;
		_volMusLabel.Text = val > 0 ? $"+{val}" : val.ToString();
		AudioServer.SetBusVolumeDb(1, val);
	}

	void _on_VolumeSliderSfx_value_changed(float val)
	{
		if (_volSfxLabel == null) return;
		if (Adjustments != null) Adjustments.SfxVolume = val;
		_volSfxLabel.Text = val > 0 ? $"+{val}" : val.ToString();
		AudioServer.SetBusVolumeDb(2, val);
	}

	void _on_VolumeSliderVoice_value_changed(float val)
	{
		if (Adjustments != null) Adjustments.VoiceVolume = val;
		AudioServer.SetBusVolumeDb(3, val);
		if (_volVoiceLabel != null)
		{
			_volVoiceLabel.Text = val > 0 ? $"+{val}" : val.ToString();
		}
	}

	void _on_CheckButtonMusic_toggled(bool pressed) 
	{
		if (Adjustments != null) Adjustments.MusicEnabled = pressed;
	}
	void _on_CheckButtonSfx_toggled(bool pressed)
	{
		if (Adjustments != null) Adjustments.SfxEnabled = pressed;
	}
	void _on_CheckButtonVoice_toggled(bool pressed)
	{
		if (Adjustments != null) Adjustments.VoiceEnabled = pressed;
	}
}
