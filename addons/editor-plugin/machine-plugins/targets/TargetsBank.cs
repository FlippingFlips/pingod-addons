using Godot;
using PinGod.Core;
using PinGod.Core.Service;

namespace PinGod.EditorPlugins
{
    [Tool]
    /// <summary>
    /// Handles a bank of targets, the light states and watches for completion <para/>
    /// TODO: check this class is good after changing everything
    /// </summary>
    public partial class TargetsBank : Node
    {
        #region Exports
        /// <summary>
        /// Switch Names
        /// </summary>
        [Export] protected string[] _target_switches;
        /// <summary>
        /// Lamp names
        /// </summary>
        [Export] protected string[] _target_lamps;
        /// <summary>
        /// Led names
        /// </summary>
        [Export] protected string[] _target_leds;
        /// <summary>
        /// Inverse the booleans?
        /// </summary>
        [Export] protected bool _inverse_lamps;
        /// <summary>
        /// Reset targets when complete
        /// </summary>
        [Export] protected bool _reset_when_completed = true;
        #endregion

        #region Signals
        /// <summary>
        /// Fired when all <see cref="_targetValues"/> are complete
        /// </summary>
        [Signal] public delegate void OnTargetsCompletedEventHandler();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swName"></param>
        /// <param name="complete">will be true if the target was false beforehand, meaning it was just completed</param>

        [Signal] public delegate void OnTargetActivatedEventHandler(string swName, bool complete);
        #endregion

        #region Fields
        /// <summary>
        /// Plugin access from /root/
        /// </summary>
        private MachineNode _machine;

        /// <summary>
        /// Array of booleans if target is complete same length as <see cref="_target_switches"/>
        /// </summary>
        public bool[] _targetValues;

        /// <summary>
        /// All targets completed?
        /// </summary>
        protected bool _targetsCompleted;
        #endregion

        /// <summary>
        /// Removes this node if no <see cref="_target_switches"/> have been set
        /// </summary>
        public override void _EnterTree()
        {
            if (!Engine.IsEditorHint())
            {
                if (_target_switches == null)
                {
                    Logger.Error(nameof(TargetsBank), ":no target switches assigned. removing mode");
                    this.QueueFree();
                }
                else
                {
                    _targetValues = new bool[_target_switches.Length];
                    if (HasNode("/root/Machine"))
                    {
                        _machine = GetNode<MachineNode>("/root/Machine");
                        _machine.SwitchCommand += OnSwitchCommandHandler;
                    }
                    else { Logger.Debug(nameof(TargetsBank), $": no {nameof(MachineNode)} plugin found", _targetValues.Length); }

                    Logger.Debug(nameof(TargetsBank), ":setting target values ", _targetValues.Length);
                }
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
            //todo: probably will need this in to check if game is playing
            //if (!pinGod.GameInPlay || pinGod.IsTilted) return;        
            for (int i = 0; i < _target_switches.Length; i++)
            {
                if (name == _target_switches[i])
                {
                    Logger.Debug(nameof(TargetsBank), ":active: ", _target_switches[i]);
                    SetTargetComplete(i);
                    if (CheckTargetsCompleted(i))
                        TargetsCompleted();
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the next target to complete
        /// </summary>
        internal int AdvanceTarget()
        {
            if (!_targetsCompleted)
            {
                for (int i = 0; i < _targetValues.Length; i++)
                {
                    if (!_targetValues[i])
                    {
                        SetTargetComplete(i);
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Checks if all targets are completed
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool CheckTargetsCompleted(int index)
        {
            for (int i = 0; i < _targetValues.Length; i++)
            {
                if (!_targetValues[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Reset target booleans and <see cref="_targetsCompleted"/>
        /// </summary>
        public void ResetTargets()
        {
            _targetValues = new bool[_target_switches.Length];
            _targetsCompleted = false;
        }

        /// <summary>
        /// Returns whether the target was set or not. Emits <see cref="OnTargetActivatedEventHandler"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool SetTargetComplete(int index)
        {
            if (!_targetValues[index])
            {
                _targetValues[index] = true;
                EmitSignal(nameof(OnTargetActivated), new Variant[] { _target_switches[index], true });
                UpdateLamps();
            }
            else
            {
                EmitSignal(nameof(OnTargetActivated), new Variant[] { _target_switches[index], false });
            }

            return _targetValues[index];
        }


        /// <summary>
        /// Targets were completed, reset values?
        /// </summary>
        /// <param name="reset"></param>
        public virtual void TargetsCompleted(bool reset = true)
        {
            if (!_targetsCompleted)
            {
                _targetsCompleted = true;
                EmitSignal(nameof(OnTargetsCompleted));

                if (_reset_when_completed)
                {
                    Logger.Debug(nameof(TargetsBank), ":targets complete, resetting");
                    ResetTargets();
                }
                else
                {
                    Logger.Debug(nameof(TargetsBank), ":targets complete");
                }
            }
        }

        /// <summary>
        /// Updates the lamps with matched to the same length as the switches
        /// </summary>
        public virtual void UpdateLamps()
        {
            //todo: this is using pingodgame to do lamps
            /*
            if (_target_lamps?.Length > 0)
            {
                for (int i = 0; i < _target_switches?.Length; i++)
                {
                    if (_targetValues[i]) pinGod.SetLampState(_target_lamps[i], _inverse_lamps ? (byte)0 : (byte)1);
                    else pinGod.SetLampState(_target_lamps[i], _inverse_lamps ? (byte)1 : (byte)0);
                }
            }
            if (_target_leds?.Length > 0)
            {
                for (int i = 0; i < _target_leds?.Length; i++)
                {
                    if (_targetValues[i]) pinGod.SetLedState(_target_leds[i], _inverse_lamps ? (byte)0 : (byte)1, 0);
                    else pinGod.SetLedState(_target_leds[i], _inverse_lamps ? (byte)1 : (byte)0, 0);
                }
            }
            */
        }
    }
}
