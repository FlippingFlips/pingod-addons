using Godot;
using PinGod.Base;
using PinGod.Core.Service;

namespace PinGod.AutoLoad
{
    [Tool]
    public partial class PinGodBottomPanel : Control
    {
        private MachineNode _machineNode;
        public override void _ExitTree()
        {
            if (Engine.IsEditorHint())
            {
                _machineNode?.QueueFree();
            }
        }

        public override void _Ready()
        {
            if (Engine.IsEditorHint())
            {
                //         var machine = Godot.Engine.GetSingleton("Machine");
                //         if (machine != null)
                //             GD.Print("found machine");
                //else
                //{
                //	var scene = GD.Load("res://autoload/Machine.tscn") as PackedScene;

                //	_machineNode = scene.Instantiate() as MachineNode;

                //	if(_machineNode!=null)
                //                 GD.Print("found machine");
                //         }

                //GetTree().ReloadCurrentScene();

                GD.Print("hello");
                if (Machine.Switches?.Count > 0)
                {
                    GD.Print("Switches found");
                }
                else
                {
                    GD.Print("Create tree");
                    var tree = GetNode<Tree>(nameof(Tree));
                    TreeItem root = tree.CreateItem();
                    root.SetText(0, "Switches");
                    tree.HideRoot = true;
                    TreeItem child1 = tree.CreateItem(root);
                    child1.SetText(0, "Heelo");
                    child1.SetChecked(1, true);

                    TreeItem child2 = tree.CreateItem(root);
                    TreeItem subchild1 = tree.CreateItem(child1);
                    subchild1.SetText(0, "Subchild1");
                }
            }
        }
    }
}
