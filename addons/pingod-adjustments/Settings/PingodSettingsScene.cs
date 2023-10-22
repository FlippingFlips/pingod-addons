using Godot;
using PinGod.Base;
using PinGod.Core;
using PinGod.Core.Service;

/// <summary>
/// PinGod menu settings. Like log levels and machine read write
/// </summary>
public partial class PingodSettingsScene : MarginContainer
{
    private Adjustments _adjustments;

    /// <summary>
    /// Sets text of labels using language translation files <see cref="Godot.Object.Tr(string)"/>
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        if (HasNode("/root/Adjustments"))
        {
            var adjustments = GetNode<AdjustmentsNode>("/root/Adjustments");
            _adjustments = adjustments._adjustments;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        var _stateDelaySpinbox = GetNode<SpinBox>("VBoxContainer/StatesDelaySpinBox");
        _stateDelaySpinbox.Value = _adjustments?.MachineStatesWriteDelay ?? 10;
        _stateDelaySpinbox.Prefix = Tr("SETT_STATE_DELAY");

        var _readStatesCheck = GetNode<CheckButton>("VBoxContainer/ReadStatesCheckButton");
        _readStatesCheck.SetPressedNoSignal(_adjustments?.MachineStatesRead ?? true);
        _readStatesCheck.Text = Tr("SETT_STATE_READ");

        var _writeStatsCheck = GetNode<CheckButton>("VBoxContainer/WriteStatesCheckButton");
        _writeStatsCheck.SetPressedNoSignal(_adjustments?.MachineStatesWrite ?? true);
        _writeStatsCheck.Text = Tr("SETT_STATE_WRITE");

        var logLvlSlider = GetNode<HSlider>("VBoxContainer/HBoxContainer/HSlider");
        var lvl = _adjustments?.LogLevel ?? 0;
        logLvlSlider.Value = (int)lvl;
        UpdateLoggerText();
    }

    private void UpdateLoggerText()
    {
        GetNode<Label>("VBoxContainer/HBoxContainer/Label2").Text = _adjustments?.LogLevel.ToString() ?? string.Empty;
    }

    void _on_StatesDelaySpinBox_changed(int val)
    {
        if (_adjustments != null) _adjustments.MachineStatesWriteDelay = val;
    }

    void _on_ReadStatesCheckButton_toggled(bool pressed)
    {
        if (_adjustments != null) _adjustments.MachineStatesRead = pressed;
    }

    void _on_WriteStatesCheckButton_toggled(bool pressed)
    {
        if (_adjustments != null) _adjustments.MachineStatesWrite = pressed;
    }

    void _on_HSlider_value_changed(float val)
    {
        var lvl = (LogLevel)val;
        if (_adjustments != null) _adjustments.LogLevel = lvl;
        UpdateLoggerText();
    }
}
