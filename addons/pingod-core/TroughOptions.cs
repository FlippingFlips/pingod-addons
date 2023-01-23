using System.Collections.Generic;

namespace PinGod.Core
{
    /// <summary>
    /// Trough settings
    /// </summary>
    public partial class TroughOptions
    {
        /// <summary>
        /// Initializes trough mech settings for a game
        /// </summary>
        /// <param name="switches"></param>
        /// <param name="coil"></param>
        public TroughOptions(string[] switches, string coil)
        {
            Switches = switches;
            Coil = coil;

            //TODO: move out of here, into trough?
            GameSwitches = new List<Switch>();
            if (switches?.Length > 0)
            {
                for (int i = 0; i < switches.Length; i++)
                {
                    if (Machine.Switches.ContainsKey(switches[i]))
                        GameSwitches.Add(Machine.Switches[switches[i]]);
                }
            }
        }

        /// <summary>
        /// used by the trough to get quicker access to switches
        /// </summary>
        public List<Switch> GameSwitches { get; }

        /// <summary>
        /// Switch names
        /// </summary>
        public string[] Switches { get; }
        /// <summary>
        /// Main coil to pulse
        /// </summary>
        public string Coil { get; }
        /// <summary>
        /// Shooter lane, Plunger Lane switch name
        /// </summary>
        public string PlungerLaneSw { get; }
        ///// <summary>
        ///// Auto plunger solenoid
        ///// </summary>
        //public string AutoPlungerCoil { get; }
        /// <summary>
        /// Switches like out-lanes to early save
        /// </summary>
        public string[] EarlySaveSwitches { get; }
        /// <summary>
        /// Default ball save time
        /// </summary>
        public int BallSaveSeconds { get; set; }
        /// <summary>
        /// Seconds left in a multi-ball
        /// </summary>
        public int MballSaveSeconds { get; set; }
        /// <summary>
        /// Ball save lamp name
        /// </summary>
        public string BallSaveLamp { get; }
        /// <summary>
        /// Ball save led name
        /// </summary>
        public string BallSaveLed { get; }
        /// <summary>
        /// How many balls to save when in ball save
        /// </summary>
        public int NumBallsToSave { get; set; }
    }
}