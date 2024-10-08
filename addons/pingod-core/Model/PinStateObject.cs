﻿namespace PinGod.Base
{
    /// <summary> Base state object machine coil, led, lamp</summary>
    public partial class PinStateObject
    {
        /// <summary> Num</summary>
        public byte Num { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public uint NumberPROC { get; set; }

        /// <summary>mm x position on the playfield</summary>
        public float? XPos { get; set; }

        /// <summary>mm y position on the playfield</summary>
        public float? YPos { get; set; }

        /// <summary> Light state </summary>
        public byte State { get; set; }

        /// <summary> Led / Lamp color</summary>
        public int Color { get; set; } = 255;

        /// <summary> Create new </summary>
        /// <param name="num"></param>
        /// <param name="state"></param>
        /// <param name="color"></param>
        public PinStateObject(byte num, byte state = 0, int color = 0)
        {
            Num = num;
            State = state;
            Color = color;
        }
    }
}