using Godot;
using PinGod.Base;
using PinGod.Core;
using PinGod.Core.Service;
using System;

/// <summary>
/// Settings Menu options for changing the window / display settings.
/// </summary>
public partial class DisplaySettingsScene : MarginContainer
{
    private Adjustments _adjustments;
    private DisplaySettings _displaySettings;
    //private PinGodGame pinGod;
    /// <summary>
    /// Sets up and gets <see cref="_displaySettings"/>
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();        
    }

    /// <summary>
    /// Sets up the input controls
    /// </summary>
    public override void _Ready()
    {
        Logger.Debug(nameof(DisplaySettingsScene), ":", nameof(_Ready));
        base._Ready();

        if (HasNode("/root/Adjustments"))
        {
            Logger.Debug(nameof(DisplaySettingsScene), ": getting adjustments from AdjustmentsScript");
            _adjustments = GetNode<AdjustmentsNode>("/root/Adjustments")._adjustments;
            _displaySettings = _adjustments?.Display;
        }
        else { Logger.WarningRich(nameof(DisplaySettingsScene), nameof(_EnterTree), ":[color=yellow]", "AdjustmentsScript not found in /root/Adjustments. Used for saving display adjustments[/color]"); }

        if (_displaySettings != null)
        {
            //ContentScaleAspectEnum        
            var stretchOption = GetNode<OptionButton>("VBoxContainer/StretchAspectOptionButton");
            foreach (var item in Enum.GetValues(typeof(Window.ContentScaleAspectEnum)))
            {
                var i = (long)item;
                var ii = (int)i;
                stretchOption.AddItem(item.ToString(), ii);
            }
            StretchAspect aspect = (StretchAspect)_displaySettings?.AspectOption;
            stretchOption.Selected = (int)aspect;

            //ContentScaleModeEnum
            var scaleOption = GetNode<OptionButton>("VBoxContainer/ScaleModeOptionButton");
            foreach (var item in Enum.GetValues(typeof(Window.ContentScaleModeEnum)))
            {
                var i = (long)item;
                var ii = (int)i;
                scaleOption.AddItem(item.ToString(), ii);
            }
            scaleOption.Selected = (int)_displaySettings.ContentScaleMode;

            GetNode<Label>("VBoxContainer/HBoxContainer/DefaultWindowSizeLabel").Text =
            $"ORIGINAL RESOLUTION: {_displaySettings.WidthDefault} X {_displaySettings.HeightDefault}";

            GetNode<CheckButton>("VBoxContainer/CheckButtonFullScreen").SetPressedNoSignal(_displaySettings.FullScreen);
            GetNode<CheckButton>("VBoxContainer/CheckButtonVsync").SetPressedNoSignal(_displaySettings.Vsync);
            GetNode<CheckButton>("VBoxContainer/CheckButtonVsyncComp").SetPressedNoSignal(_displaySettings.VsyncViaCompositor);
            GetNode<CheckButton>("VBoxContainer/CheckButtonAlwaysOnTop").SetPressedNoSignal(_displaySettings.AlwaysOnTop);
            GetNode<SpinBox>("VBoxContainer/SpinBoxFPS").Value = _displaySettings.FPS;
            //(bool)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC);
            //(bool)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC_COMPOSITOR)
            //var top = (bool)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.ALWAYS_ON_TOP);        
            //var fpsStr = ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.FORCE_FPS).ToString();
            //int.TryParse(fpsStr, out var fps);

            //aspect ratio
            //var val = ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.ASPECT).ToString();
        }        
    }

    void _on_CheckButtonAlwaysOnTop_toggled(bool pressed)
    {
        Logger.Debug(nameof(DisplaySettingsScene), ":on top pressed " + pressed);
        Display.SetAlwaysOnTop(pressed);
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.ALWAYS_ON_TOP, pressed);
        _displaySettings.AlwaysOnTop= pressed;
    }

    void _on_CheckButtonFullScreen_toggled(bool pressed)
    {        
        CallDeferred(nameof(SetFullScreen), pressed);
    }

    void _on_CheckButtonVsync_toggled(bool pressed)
    {
        var enabled = pressed ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;

        DisplayServer.WindowSetVsyncMode(enabled);
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC, pressed);
        _displaySettings.Vsync = pressed;
    }

    void _on_CheckButtonVsyncComp_toggled(bool pressed)
    {
        //TODO: godot4 added different options
        //OS.VsyncViaCompositor = pressed;
        //DisplayServer.vsy
        //ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC_COMPOSITOR, pressed);
        //_displaySettings.VsyncViaCompositor = pressed;
    }

    void _on_SpinBoxFPS_value_changed(float value)
    {        
        Engine.MaxFps = (int)value;
        Logger.Debug(nameof(DisplaySettingsScene), ":fps changed (0 = no limit):", Engine.MaxFps);
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.FORCE_FPS, Engine.MaxFps);
        _displaySettings.FPS = (int)value;
    }

    void _on_ResetDefaultButton_button_up()
    {
        Vector2I wH = new Vector2I(1024,600);
        if(_displaySettings == null)
        {
            var ps = Display.GetDisplayProjectSettings();
            wH = new Vector2I(ps.Width, ps.Height);
        }
        else
        {
            wH = new Vector2I(_displaySettings.WidthDefault, _displaySettings.HeightDefault);
        }

        DisplayServer.WindowSetSize(wH);
    }

    void _on_StretchAspectOptionButton_item_selected(int index)
    {         
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.ASPECT, ((StretchAspect)index).ToString());
        var asp = (StretchAspect)Enum.Parse(typeof(StretchAspect), ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.ASPECT).ToString());
        GetTree().Root.ContentScaleAspect = (Window.ContentScaleAspectEnum)asp;
        _displaySettings.AspectOption = index;        
    }

    void _on_scale_mode_option_button_item_selected(int index)
    {
        //ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths., ((PinGodStretchAspect)index).ToString());        
        GetTree().Root.ContentScaleMode = (Window.ContentScaleModeEnum)_displaySettings.ContentScaleMode;
        _displaySettings.ContentScaleMode = index;
    }

    private void SetFullScreen(bool enabled)
    {
        if (!enabled)
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        else
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);        
        _displaySettings.FullScreen = enabled;
    }
}
