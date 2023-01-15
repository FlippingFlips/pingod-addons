using Godot;
using NetProc.Domain;
using PinGod.Core.Service;
using PinGod.Game;

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
    protected readonly Resources _resources;

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
    /// <param name="defaultScene">path to default scene</param>
    /// <param name="loadDefaultScene">Load deefault scene when the object is create?</param>
    public PinGodProcMode(IGameController game, int priority, PinGodGame pinGod, string defaultScene = null, bool loadDefaultScene = true) : base(game, priority)
    {
        _modesCanvas = pinGod.GetNodeOrNull<CanvasLayer>(modesRootPath);
        CanvasLayer = new CanvasLayer() { Layer = priority };
        _modesCanvas.AddChild(CanvasLayer);

        _resources = pinGod.GetResources();

        if (!string.IsNullOrWhiteSpace(defaultScene) && loadDefaultScene)
        {
            this.defaultScene = defaultScene;
        }        
    }

    public virtual void AddChildSceneToCanvasLayer(Node node) => CanvasLayer.AddChild(node);

    public virtual void RemoveChildSceneFromCanvasLayer(Node node) => CanvasLayer.RemoveChild(node);

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
}