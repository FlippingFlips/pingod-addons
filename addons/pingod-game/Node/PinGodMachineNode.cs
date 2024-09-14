using Godot;

public abstract partial class PinGodMachineNode : Node
{
    protected MachineNode _machine;

    /// <summary>Gets a MachineNode</summary>
    public override void _EnterTree()
    {
        base._EnterTree();

        if (!Engine.IsEditorHint())
        {
            if (HasNode(Paths.ROOT_MACHINE))
                _machine = GetNode<MachineNode>(Paths.ROOT_MACHINE);
            else 
            { 
                Logger.Warning(nameof(PinGodMachineNode), $": no {nameof(MachineNode)} plug-in found");
                this.QueueFree();
            }
        }
    }
}
