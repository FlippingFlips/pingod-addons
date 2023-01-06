using Godot;
using static Godot.DisplayServer;

/// <summary>
/// Wrapper for <see cref="Godot.DisplayServer"/>
/// </summary>
public static class Display
{
    public static Godot.Vector2i WinGetPos(this PinGodGame pinGod)
    => WindowGetPosition();

    public static Godot.Vector2i WinGetSize(this PinGodGame pinGod)
        => WindowGetSize();

    /// <summary>
    /// Set windows flags like border less
    /// </summary>
    /// <param name="pingod"></param>
    /// <param name="flags"></param>
    /// <param name="on"></param>
    /// <param name="windowId"></param>
    public static void SetWinFlag(WindowFlags flags, bool on, int windowId = 0)
        => WindowSetFlag(flags, on, windowId);

    /// <summary>
    /// Toggle the flag on window
    /// </summary>
    /// <param name="pinGod"></param>
    /// <param name="flags"></param>
    /// <param name="windowId"></param>
    public static void ToggleWinFlag(WindowFlags flags, int windowId = 0)
        => WindowSetFlag(flags, !WindowGetFlag(flags), windowId);

    public static DisplaySettings GetDisplayProjectSettings()
    {
        //TODO: add v-sync modes. display/window/vsync/vsync_mode
        var displaySettings = new DisplaySettings()
        {
            AspectOption = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.ASPECT),
            AlwaysOnTop = (bool)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.ALWAYS_ON_TOP),
            WidthDefault = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.WIDTH),
            HeightDefault = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.HEIGHT),
            Width = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.TEST_WIDTH),
            Height = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.TEST_HEIGHT),
            Vsync = (bool)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC),
            FPS = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.FORCE_FPS),
            ContentScaleMode = (Window.ContentScaleModeEnum)((int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.CONTENT_SCALE)),
        };
        return displaySettings;
    }
}

public class ProjectSettingsDisplay
{

}
