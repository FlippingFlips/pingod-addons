using Godot;
using PinGod.Base;

namespace PinGod.Core
{
    /// <summary>
    /// Represents a Switch in a pinball machine
    /// </summary>
    public partial class Switch
    {
        /// <summary>
        /// Initialize with number only
        /// </summary>
        /// <param name="num"></param>
        public Switch(byte num) { Num = num; }
        /// <summary>
        /// Initialize with number and options for ball searching
        /// </summary>
        /// <param name="num"></param>
        /// <param name="ballSearch"></param>
        public Switch(byte num, BallSearchSignalOption ballSearch) { Num = num; BallSearch = ballSearch; }

        /// <summary>
        /// Initialize with name and number with options for ball searching
        /// </summary>
        /// <param name="name"></param>
        /// <param name="num"></param>
        /// <param name="ballSearch"></param>
        public Switch(string name, byte num, BallSearchSignalOption ballSearch) { Name = name; Num = num; BallSearch = ballSearch; }

        /// <summary>
        /// Initialize Switch name + num
        /// </summary>
        /// <param name="name"></param>
        /// <param name="num"></param>
        public Switch(string name, byte num) { Name = name; Num = num; }
        /// <summary>
        /// Initialize
        /// </summary>
        public Switch()
        {
            Time = Godot.Time.GetTicksMsec();
        }
        /// <summary>
        /// Name of the switch
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Number of the switch
        /// </summary>
        public byte Num { get; set; }
        /// <summary>
        /// Ball search options
        /// </summary>
        public BallSearchSignalOption BallSearch { get; set; }
        /// <summary>
        /// Flag set by actions and SetSwitch
        /// </summary>
        public bool IsEnabled() => State > 0;

        /// <summary>
        /// Time last active
        /// </summary>
        public ulong Time { get; set; }
        public byte State { get; set; }
        /// <summary>
        /// Is set when Time is set. This is to save the time before it is set <para/>
        /// When the game gets the switch command the time is set so we need the time before that <para/>
        /// It would be needed in cases we need to check a switch is set before it gets set
        /// </summary>
        public ulong TimePrevious { get; private set; }

        /// <summary>
        /// Sets a switch manually, pushes a InputEventAction to Input
        /// </summary>
        /// <param name="pressed"></param>
        /// <returns></returns>
        public void SetSwitchAction(bool pressed)
        {
            Input.ParseInputEvent(new InputEventAction() { Action = ToString(), Pressed = pressed });
            State = (byte)(pressed ? 1 : 0);
        }

        /// <summary>
        /// Sets <see cref="IsEnabled"/> and sets the time of the switch <para/>
        /// TimePrevious is set to save the Time before the Time is set
        /// </summary>
        /// <param name="enabled"></param>
        public void SetSwitch(byte state)
        {
            State = state;
            TimePrevious = Time;
            Time = Godot.Time.GetTicksMsec();
            //Logger.Verbose(nameof(Switch), $":{Name}:{Num}={State}"); TODO: move this log elsewhere
        }

        /// <summary>
        /// Checks the current input event. IsActionPressed(sw+num)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsActionOn(InputEvent input)
        {
            if (input is InputEventMouse) return false;

            bool active = input.IsActionPressed(ToString());
            if (active)
            {
                SetSwitch(1);
            }
            return active;
        }
        /// <summary>
        /// Checks the current input event. IsActionReleased(sw+num)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsActionOff(InputEvent input)
        {
            bool released = input.IsActionReleased(ToString());
            if (released)
            {
                SetSwitch(0);
            }
            return released;
        }
        /// <summary>
        /// Checks if On/Off - Action pressed sw{num}
        /// </summary>
        /// <returns></returns>
        public bool IsActionOn()
        {
            State = (byte)(Input.IsActionPressed(ToString()) == true ? 1 : 0);
            return IsEnabled();
        }

        /// <summary>
        /// Time in milliseconds since switch used
        /// </summary>
        /// <returns></returns>
        public ulong TimeSinceChange()
        {
            if (TimePrevious > 0)
            {
                return Godot.Time.GetTicksMsec() - TimePrevious;
            }

            return 0;
        }
        /// <summary>
        /// The godot action name. swNum. sw60 or sw81
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "sw" + Num;
    }
}