﻿using Godot;
/// <summary>
/// Window / Display settings
/// </summary>
public partial class DisplaySettings
{
    /// <summary>
    /// Set window on top
    /// </summary>
    public bool AlwaysOnTop { get; set; } = true;
    /// <summary>
    /// 
    /// </summary>
    public int AspectOption { get; set; } = 4;
    /// <summary>
    /// Frame limiting. 0 no limit (default)
    /// </summary>
    public double FPS { get; set; } = 0;
    /// <summary>
    /// Is Full screen?
    /// </summary>
    public bool FullScreen { get; set; } = false;
    /// <summary>
    /// 
    /// </summary>
    public int Height { get; set; } = 600;
    /// <summary>
    /// Height game created in
    /// </summary>
    public int HeightDefault { get; set; } = 600;
    /// <summary>
    /// 
    /// </summary>
    public bool LowDpi { get; set; } = false;
    /// <summary>
    /// Create no window but runs game
    /// </summary>
    public bool NoWindow { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool Vsync { get; set; } = true;
    /// <summary>
    /// 
    /// </summary>
    public bool VsyncViaCompositor { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Width { get; set; } = 1024;
    /// <summary>
    /// Width game created in
    /// </summary>
    public int WidthDefault { get; set; } = 1024;
    /// <summary>
    /// StartSaver X position
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// StartSaver Y position
    /// </summary>
    public int Y { get; set; }
    public Window.ContentScaleModeEnum ContentScaleMode { get; set; }
}