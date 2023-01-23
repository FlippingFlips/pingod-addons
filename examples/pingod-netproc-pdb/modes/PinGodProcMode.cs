using Godot;
using NetProc.Domain;
using PinGod.Core.Service;
using PinGod.Game;
using PinGod.Core;

/// <summary>
/// Base PinGod P-ROC Mode class. This is a P-ROC Mode but with an added CanvasLayer from Godot. P-ROC modes have a layer property for the DMD. If you need to use a scene from Godot with a P-ROC mode then inherit this base <para/>
/// You would have a scene running with a CanvasLayer node named Modes, then this "Layer" is added to that with the layer priority set from P-ROC mode priority. The priority for CanvasLayer is the order it's shown. <para/>
/// When the mode is removed from the game modes then this layer is cleared from the (CanvasLayer)Modes. <para/>
/// This base mode needs the Resources autoload plug-in running to load packed scenes. You would add the scene that you're loading into this layer in the Resources.tscn. This scene is preloaded there, so when we need to add to this layer we can get it from the preloaded resources.
/// </summary>
public abstract class PinGodProcMode : Mode
{
    [Export(PropertyHint.File)] string defaultScene = "res://PinGodLogoScene.tscn";

    /// <summary>
    /// The Scene should contain a Modes node
    /// </summary>
    public CanvasLayer _modesCanvas { get; private set; }

    /// <summary>
    /// This modes Layer. Add scenes to this layer. The Godot CanvasLayer has a Layer property for priority.
    /// </summary>
    public CanvasLayer CanvasLayer { get; private set; }

    protected Resources _resources { get; private set; }
    public string Name { get; }
    public IPinGodGame PinGod { get; }

    protected readonly PinGodProcGameController _game;

    /// <summary>
    /// The node where we had our CanvasLayer
    /// </summary>
    [Export] string modesRootPath = "/root/ProcScene/Modes";

    /// <summary>
    /// Default scene to load
    /// </summary>
    /// <summary>
    /// Initializes a base P-ROC mode for PinGod. <para/> <see cref="CreateCanvasLayer"/>
    /// </summary>
    /// <param name="game"></param>
    /// <param name="priority">Layer on the canvas is a priority</param>
    /// <param name="pinGod">Need this to grab the root</param>
    /// <param name="defaultScene">path to default scene</param>
    /// <param name="loadDefaultScene">Load deefault scene when the object is create?</param>
    public PinGodProcMode(IGameController game, string name, int priority, IPinGodGame pinGod, string defaultScene = null, bool loadDefaultScene = true) : base(game, priority)
    {
        Name = name;
        PinGod = pinGod;

        _game = game as PinGodProcGameController;

        var pg = PinGod as PinGodGame;
        _modesCanvas = pg.GetNodeOrNull<CanvasLayer>(modesRootPath);
        _resources = pg.GetResources();
    }

    public virtual void AddChildSceneToCanvasLayer(Node node)
    {
        if(CanvasLayer == null)
        {
            CreateCanvasLayer(Name, Priority);
        }

        CanvasLayer.CallDeferred("add_child", node);
    }

    /// <summary>
    /// Creates a CanvasLayer to hold this mode in Godot scenes
    /// </summary>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    /// <param name="addAsChild">add the canvaslayer to the modes canvas layer</param>
    public virtual void CreateCanvasLayer(string name, int priority, bool addAsChild = true)
    {
        if (_modesCanvas != null)
        {
            CanvasLayer = new CanvasLayer() { Layer = priority, Name = name };
            if(addAsChild)
                _modesCanvas?.AddChild(CanvasLayer);
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
        if(CanvasLayer != null)
        {
            _modesCanvas?.RemoveChild(CanvasLayer);
            CanvasLayer?.QueueFree();
        }        
        base.ModeStopped();
    }

    public virtual void RemoveChildSceneFromCanvasLayer(Node node)
    {
        if(!node?.IsQueuedForDeletion() ?? false)
            CanvasLayer?.RemoveChild(node);
    }
}