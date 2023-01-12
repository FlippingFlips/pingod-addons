using Godot;
using PinGod.Base;
using PinGod.Core;
using System;

/// <summary>
/// PinGod menu settings. Like log levels and machine read write
/// </summary>
public partial class PingodSettingsScene : MarginContainer
{
    private IPinGodGame pinGod;

    /// <summary>
    /// Sets text of labels using language translation files <see cref="Godot.Object.Tr(string)"/>
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        if (HasNode("/root/PinGodGame"))
        {
            pinGod = GetNode<IPinGodGame>("/root/PinGodGame");
        }
    }

    public override void _Ready()
    {
        base._Ready();
        var _stateDelaySpinbox = GetNode<SpinBox>("VBoxContainer/StatesDelaySpinBox");
        _stateDelaySpinbox.Value = pinGod?.Adjustments?.MachineStatesWriteDelay ?? 10;
        _stateDelaySpinbox.Prefix = Tr("SETT_STATE_DELAY");

        var _readStatesCheck = GetNode<CheckButton>("VBoxContainer/ReadStatesCheckButton");
        _readStatesCheck.SetPressedNoSignal(pinGod?.Adjustments?.MachineStatesRead ?? true);
        _readStatesCheck.Text = Tr("SETT_STATE_READ");

        var _writeStatsCheck = GetNode<CheckButton>("VBoxContainer/WriteStatesCheckButton");
        _writeStatsCheck.SetPressedNoSignal(pinGod?.Adjustments?.MachineStatesWrite ?? true);
        _writeStatsCheck.Text = Tr("SETT_STATE_WRITE");

        var logLvlSlider = GetNode<HSlider>("VBoxContainer/HBoxContainer/HSlider");
        var lvl = pinGod?.Adjustments?.LogLevel ?? 0;
        logLvlSlider.Value = (int)lvl;
        UpdateLoggerText();
    }

    private void UpdateLoggerText()
    {
        GetNode<Label>("VBoxContainer/HBoxContainer/Label2").Text = pinGod?.Adjustments?.LogLevel.ToString() ?? string.Empty;
    }

    void _on_StatesDelaySpinBox_changed(int val)
    {
        if (pinGod != null) pinGod.Adjustments.MachineStatesWriteDelay = val;
    }

    void _on_ReadStatesCheckButton_toggled(bool pressed)
    {
        if (pinGod != null) pinGod.Adjustments.MachineStatesRead = pressed;
    }

    void _on_WriteStatesCheckButton_toggled(bool pressed)
    {
        if (pinGod != null) pinGod.Adjustments.MachineStatesWrite = pressed;
    }

    void _on_HSlider_value_changed(float val)
    {
        var lvl = (LogLevel)val;
        if (pinGod != null) pinGod.Adjustments.LogLevel = lvl;
        UpdateLoggerText();
    }
}
