using Godot;
using PinGod.Core;
using PinGod.Core.Service;

namespace PinGod.Modes
{
    /// <summary>
    /// Inherits from Node, is a mode
    /// </summary>
    public partial class ServiceMenu : Node
    {
        /// <summary>
        /// The default label in the scene, assigned on scene Ready
        /// </summary>
        protected Label menuNameLabel;

        /// <summary>
        /// Pingod game reference
        /// </summary>
        protected IPinGodGame pinGod;
        private MachineNode _pingodMachine;

        /// <summary>
        /// Just gets a reference to <see cref="pinGod"/>, this should be invoked if overriding the method
        /// </summary>
        public override void _EnterTree()
        {
            pinGod = GetNode(Paths.ROOT_PINGODGAME) as IPinGodGame;

            if (HasNode("/root/" + nameof(MachineNode)))
            {
                _pingodMachine = GetNode<MachineNode>(Paths.ROOT_MACHINE);
                _pingodMachine.SwitchCommand += OnSwitchCommandHandler;
            }
        }

        /// <summary>
        /// Switch handlers for lanes and slingshots
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void OnSwitchCommandHandler(string name, byte index, byte value)
        {
            if (value <= 0) return;
            switch (name)
            {
                case "enter":
                    OnEnter();
                    break;
                case "up":
                    OnUp();
                    break;
                case "down":
                    OnDown();
                    break;
                case "exit":
                    OnExit();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the label in the center of screen. <see cref="menuNameLabel"/>
        /// </summary>
        public override void _Ready()
        {
            menuNameLabel = GetNode("CenterContainer/Label") as Label;
        }
        /// <summary>
        /// Fired with Down switch.
        /// </summary>
        public virtual void OnDown() { pinGod.PlaySfx("enter"); }

        /// <summary>
        /// Fired with Enter switch.
        /// </summary>
        public virtual void OnEnter() { pinGod.PlaySfx("enter"); }

        /// <summary>
        /// Fired with Exit switch. Emits "ServiceMenuExit" and removes from the scene, plays "exit" sfx
        /// </summary>
        public virtual void OnExit()
        {
            pinGod.PlaySfx("exit");
            //pinGod.EmitSignal("ServiceMenuExit"); //TODO
            this.QueueFree();
        }

        /// <summary>
        /// Fired with Up switch, plays "enter" sfx
        /// </summary>
        public virtual void OnUp() { pinGod.PlaySfx("enter"); }
    }
}
