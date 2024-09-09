using Godot;

/// <summary>Uses Godot Node as a base to provide PinGodGame access to a class</summary>
public abstract partial class PinGodGameControl : Control
{
    /// <summary>A reference to PinGodGame node</summary>
    public IPinGodGame _pinGod;

    /// <summary>A reference to PinGodGame resources</summary>
    public Resources _resources;

    /// <summary>Gets a reference to <see cref="pinGod"/> in the root /root/PinGodGame</summary>
    public override void _EnterTree()
    {
        _pinGod = GetNodeOrNull(Paths.ROOT_PINGODGAME) as IPinGodGame;
        _resources = GetNodeOrNull(Paths.ROOT_RESOURCES) as Resources;
    }
}
