using Godot;
using static Godot.DisplayServer;

namespace PinGod.Core
{
    /// <summary>
    /// Wrapper for <see cref="Godot.DisplayServer"/>
    /// </summary>
    public static class Display
    {
        public static void SetAlwaysOnTop(bool onTop) => WindowSetFlag(WindowFlags.AlwaysOnTop, onTop);

        public static Window.ContentScaleModeEnum GetContentScale(Node node)
            => node.GetTree().Root.ContentScaleMode;

        public static Window.ContentScaleAspectEnum GetAspectOption(Node node)
            => node.GetTree().Root.ContentScaleAspect;

        public static void SetContentScale(Node node, Window.ContentScaleModeEnum contentScale)
            => node.GetTree().Root.ContentScaleMode = contentScale;

        public static void SetAspectOption(Node node, Window.ContentScaleAspectEnum aspectOpt)
            => node.GetTree().Root.ContentScaleAspect = aspectOpt;

        /// <summary>
        /// Sets main window position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetPosition(int x, int y) => WindowSetPosition(new Vector2I(x, y));

        /// <summary>
        /// Sets main window size
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public static void SetSize(int w, int h) => WindowSetSize(new Vector2I(w, h));

        public static Godot.Vector2I WinGetPos(this IPinGodGame pinGod) => WindowGetPosition();

        public static Godot.Vector2I WinGetSize(this IPinGodGame pinGod) => WindowGetSize();

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
