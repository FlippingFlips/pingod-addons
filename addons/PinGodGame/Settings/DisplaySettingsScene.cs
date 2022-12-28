using Godot;
using System;

/// <summary>
/// Settings Menu options for changing the window / display settings.
/// </summary>
public class DisplaySettingsScene : MarginContainer
{
    private DisplaySettings _displaySettings;
    private PinGodGame pinGod;
    /// <summary>
    /// Sets up and gets <see cref="_displaySettings"/>
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();

        pinGod = GetNode<PinGodGame>("/root/PinGodGame");
        _displaySettings = pinGod.GameSettings.Display;
    }

    /// <summary>
    /// Gets settings and setups default UI controls.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        //setup options for stretch modes
        var stretchOption = GetNode<OptionButton>("VBoxContainer/StretchAspectOptionButton");
        foreach (SceneTree.StretchAspect item in Enum.GetValues(typeof(SceneTree.StretchAspect)))
        {
            stretchOption.AddItem(item.ToString(), (int)item);
        }

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
        PinGodStretchAspect aspect = (PinGodStretchAspect)_displaySettings.AspectOption;//()Enum.Parse(typeof(PinGodStretchAspect), );
        stretchOption.Selected = (int)aspect;        

    }

    void _on_CheckButtonAlwaysOnTop_toggled(bool pressed)
    {
        Logger.LogInfo("on top pressed " + pressed);
        OS.SetWindowAlwaysOnTop(pressed);
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.ALWAYS_ON_TOP, pressed);
        _displaySettings.AlwaysOnTop= pressed;
    }

    void _on_CheckButtonFullScreen_toggled(bool pressed)
    {        
        CallDeferred(nameof(SetFullScreen), pressed);
    }

    void _on_CheckButtonVsync_toggled(bool pressed)
    {
        OS.VsyncEnabled = pressed;
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC, pressed);
        _displaySettings.Vsync = pressed;
    }

    void _on_CheckButtonVsyncComp_toggled(bool pressed)
    {
        OS.VsyncViaCompositor = pressed;
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC_COMPOSITOR, pressed);
        _displaySettings.VsyncViaCompositor = pressed;
    }

    void _on_SpinBoxFPS_value_changed(float value)
    {
        Logger.LogDebug("fps changed");
        Engine.TargetFps = (int)value;
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.FORCE_FPS, Engine.TargetFps);
        _displaySettings.FPS = (int)value;
    }

    void _on_ResetDefaultButton_button_up()
    {
        if(_displaySettings.WidthDefault > 50 && _displaySettings.HeightDefault > 50)
        {
            OS.WindowSize = new Vector2(_displaySettings.WidthDefault, _displaySettings.HeightDefault);                 
            //don't save the project settings width / height as this will override in the settings. when changed here it will add it into the override.cfg
        }
    }

    void _on_StretchAspectOptionButton_item_selected(int index)
    {         
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.ASPECT, ((PinGodStretchAspect)index).ToString());
        pinGod.SetMainSceneAspectRatio();
        _displaySettings.AspectOption = index;
    }

    private void SetFullScreen(bool pressed)
    {
        OS.WindowFullscreen = pressed;
        pinGod.GameSettings.Display.FullScreen = pressed;
    }
}
