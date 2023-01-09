using Godot;
using PinGod.Base;
using static Godot.DisplayServer;

namespace PinGod.Core
{
    /// <summary>
    /// Wrapper for <see cref="Godot.DisplayServer"/>
    /// </summary>
    public static class Display
    {
        public static Godot.Vector2i WinGetPos(this IPinGodGame pinGod)
        => WindowGetPosition();

        public static Godot.Vector2i WinGetSize(this IPinGodGame pinGod)
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
            var w = ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.WIDTH);
            //TODO: add v-sync modes. display/window/vsync/vsync_mode
            var displaySettings = new DisplaySettings()
            {
                AspectOption = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.ASPECT),
                AlwaysOnTop = (bool)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.ALWAYS_ON_TOP),
                Width = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.WIDTH),
                Height = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.HEIGHT),
                WidthDefault = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.WIDTH),
                HeightDefault = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.HEIGHT),
                Vsync = (bool)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.USE_VSYNC),
                FPS = (int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.FORCE_FPS),
                ContentScaleMode = ((int)ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.CONTENT_SCALE)),
            };
            return displaySettings;
        }
    }

    public class ProjectSettingsDisplay
    {

    }
}
