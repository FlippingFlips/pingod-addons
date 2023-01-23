using Godot;

namespace PinGod.Core.Service
{
    /// <summary>
    /// Uses Godot Node as a base to provide PinGodGame access to a class
    /// </summary>
    public abstract partial class PinGodGameNode : Node
    {
        /// <summary>
        /// A reference to PinGodGame node
        /// </summary>
        public IPinGodGame pinGod;

        /// <summary>
        /// Gets a reference to <see cref="pinGod"/> in the root /root/PinGodGame
        /// </summary>
        public override void _EnterTree()
        {
            pinGod = GetNodeOrNull("/root/PinGodGame") as IPinGodGame;
        }
    }
}