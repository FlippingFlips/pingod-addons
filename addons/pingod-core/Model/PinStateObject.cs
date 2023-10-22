namespace PinGod.Base
{

    /// <summary> Base state object machine coil, led, lamp</summary>
    public partial class PinStateObject
    {
        /// <summary> Number</summary>
        public byte Num { get; set; }

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