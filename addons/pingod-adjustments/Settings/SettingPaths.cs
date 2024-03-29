﻿/// <summary>
/// string to godot project settings paths
/// </summary>
public static class SettingPaths
{
    /// <summary>
    /// godot display paths
    /// </summary>
    public static class DisplaySetPaths
    {
        /// <summary>
        /// window size always_on_top
        /// </summary>
        public const string ALWAYS_ON_TOP = "display/window/size/always_on_top";
        /// <summary>
        /// window stretch aspect
        /// </summary>
        public const string ASPECT = "display/window/stretch/aspect";        
        /// <summary>
        /// debug fps
        /// </summary>
        public const string FORCE_FPS = "debug/settings/fps/force_fps";        
        /// <summary>
        /// use_vsync window
        /// </summary>
        public const string USE_VSYNC = "display/window/vsync/use_vsync";
        /// <summary>
        /// vsync_via_compositor window
        /// </summary>
        public const string USE_VSYNC_COMPOSITOR = "display/window/vsync/vsync_via_compositor";
        /// <summary>
        /// window width
        /// </summary>
        public const string WIDTH = "display/window/size/viewport_width";
        /// <summary>
        /// window height
        /// </summary>
        public const string HEIGHT = "display/window/size/viewport_height";
        /// <summary>
        /// value test_width path in project settings
        /// </summary>
        public const string TEST_WIDTH = "display/window/size/test_width";
        /// <summary>
        /// value test_height path in project settings
        /// </summary>
        public const string TEST_HEIGHT = "display/window/size/test_height";
        /// <summary>
        /// enum int
        /// </summary>
        public const string CONTENT_SCALE_MODE = "display/window/stretch/mode";
        /// <summary>
        /// typeof double
        /// </summary>
        public const string CONTENT_SCALE = "display/window/stretch/scale";

        //display/window/size/mode
        //display/window/size/resizable
        //display/window/size/borderless
        //display/window/size/always_on_top
        //display/window/size/transparent
        //display/window/stretch/mode
        //display/window/stretch/aspect
        //display/window/stretch/scale
        //display/window/subwindows/embed_subwindows

        //editor
        //run/window_placement/rect_custom_position
    }
}
