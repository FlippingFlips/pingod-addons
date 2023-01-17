using Godot;
using NetProc.Domain;
using PinGod.Core.Service;
using PinGod.Game;
using PinGod.Core;

/// <summary>
/// Base PinGod P-ROC class
/// </summary>
public abstract class PinGodProcMode : Mode
{
    /// <summary>
    /// The Scene should contain a Modes node
    /// </summary>
    public CanvasLayer _modesCanvas { get; private set; }

    /// <summary>
    /// This modes Layer. Add scenes to this layer. The Godot CanvasLayer has a Layer property for priority.
    /// </summary>
    public CanvasLayer CanvasLayer { get; private set; }

    protected Resources _resources { get; private set; }
    public IPinGodGame PinGod { get; }

    [Export(PropertyHint.File)] string defaultScene = "res://PinGodLogoScene.tscn";

    /// <summary>
    /// The node where we had our CanvasLayer
    /// </summary>
    [Export] string modesRootPath = "/root/ProcScene/Modes";

    /// <summary>
    /// Default scene to load
    /// </summary>
    /// <summary>
    /// Initializes a base P-ROC mode for PinGod. <para/> <see cref="GetAndCreateCanvasLayers"/>
    /// </summary>
    /// <param name="game"></param>
    /// <param name="priority">Layer on the canvas is a priority</param>
    /// <param name="pinGod">Need this to grab the root</param>
    /// <param name="defaultScene">path to default scene</param>
    /// <param name="loadDefaultScene">Load deefault scene when the object is create?</param>
    public PinGodProcMode(IGameController game, int priority, IPinGodGame pinGod, string defaultScene = null, bool loadDefaultScene = true) : base(game, priority)
    {
        PinGod = pinGod;
        GetAndCreateCanvasLayers(priority);        
    }

    public virtual void AddChildSceneToCanvasLayer(Node node) => CanvasLayer.CallDeferred("add_child", node);

    /// <summary>
    /// Gets the Modes canvas layer from the scene, creates a CanvasLayer for this mode to display. <para/>
    /// Uses the <see cref="Resources"/> root singleton to load packed scenes, but it's probably better that it is used and done by the mode inheriting this and then using the <see cref="AddChildSceneToCanvasLayer(Node)"/>
    /// </summary>
    public virtual void GetAndCreateCanvasLayers(int priority)
    {
        var pg = PinGod as PinGodGame;
        _modesCanvas = pg.GetNodeOrNull<CanvasLayer>(modesRootPath);
        if (_modesCanvas != null)
        {
            CanvasLayer = new CanvasLayer() { Layer = priority };
            _modesCanvas?.AddChild(CanvasLayer);

            _resources = pg.GetResources();
        }
        else { Logger.Warning(nameof(PinGodGameMode), ": no Modes canvas found"); }
    }
    /// <summary>
    /// Loads a packed scene and adds child
    /// </summary>
    /// <param name="scenePath"></param>
    public virtual void LoadDefaultSceneToCanvas(string scenePath)
    {
        if (_modesCanvas != null)
        {
            var res = GD.Load<PackedScene>(scenePath);
            var inst = res.Instantiate();
            CanvasLayer?.AddChild(inst);
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

    public virtual void RemoveChildSceneFromCanvasLayer(Node node) => CanvasLayer.RemoveChild(node);
}