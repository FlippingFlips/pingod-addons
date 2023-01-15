using Godot;
using NetProc.Domain;
using PinGod.Game;
using Switch = NetProc.Domain.Switch;

/// <summary>
/// Base PinGod P-ROC class
/// </summary>
public abstract class PinGodProcMode : Mode
{
    /// <summary>
    /// The node where we had our CanvasLayer
    /// </summary>
    [Export] string modesRootPath = "/root/ProcScene/Modes";

    /// <summary>
    /// Default scene to load
    /// </summary>

    [Export(PropertyHint.File)] string defaultScene = "res://PinGodLogoScene.tscn";

    /// <summary>
    /// This modes Layer. Add scenes to this layer. The Godot CanvasLayer has a Layer property for priority.
    /// </summary>
    public readonly CanvasLayer CanvasLayer;

    /// <summary>
    /// The Scene should contain a Modes node
    /// </summary>
    CanvasLayer _modesCanvas;

    /// <summary>
    /// Initializes a P-ROC mode
    /// </summary>
    /// <param name="game"></param>
    /// <param name="priority">Layer on the canvas is a priority</param>
    /// <param name="pinGod">Need this to grab the root</param>
    public PinGodProcMode(IGameController game, int priority, PinGodGame pinGod) : base(game, priority)
    {
        _modesCanvas = pinGod.GetNodeOrNull<CanvasLayer>(modesRootPath);
        if(_modesCanvas != null)
        {
            var res = GD.Load<PackedScene>(defaultScene);
            CanvasLayer = new CanvasLayer() { Layer = priority };
            var inst = res.Instantiate();
            CanvasLayer.AddChild(inst);
            _modesCanvas.AddChild(CanvasLayer);
        }        
    }

    /// <summary>
    /// Removes the <see cref="CanvasLayer"/> from the Modes
    /// </summary>
    public override void ModeStopped()
    {
        _modesCanvas?.RemoveChild(CanvasLayer);
        CanvasLayer?.QueueFree();
        base.ModeStopped();
    }

    public bool sw_flipperLwL_active(Switch sw = null)
    {

        Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active) + $": {sw.TimeSinceChange()}");
        Game.Modes.Remove(this);
        return true;
    }

    public bool sw_flipperLwL_active_for_1s(Switch sw = null)
    {
        Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active) + $": {sw.TimeSinceChange()}");
        return true;
    }
}