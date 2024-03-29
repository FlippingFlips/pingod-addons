﻿namespace PinGod.Core.Service
{
    /// <summary>A base mode added to the Group named "Mode". GamePlay events </summary>
    public abstract partial class PinGodGameMode : PinGodGameNode
    {
        /// <summary>Adds the mode to a group named Mode </summary>
        public override void _Ready()
        {
            base._Ready();

            AddToGroup("Mode");
        }

        /// <summary> Mode group signals</summary>
        protected virtual void OnBallDrained() { }

        /// <summary>Mode group signals</summary>
        protected virtual void OnBallSaved() { }

        /// <summary>Mode group signals</summary>
        protected virtual void OnBallStarted() { }

        /// <summary>Mode group signals</summary>
        protected virtual void UpdateLamps() { }

    }
}