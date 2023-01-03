using System;

namespace BasicGameGodot.addons.pingod_memorymap
{
    /// <summary>
    /// Switch changed detected memory map
    /// </summary>
    public class SwitchEventArgs : EventArgs
    {
        public SwitchEventArgs(int num, byte value)
        {
            Num = num;
            Value = value;
        }

        public int Num { get; }
        /// <summary>
        /// 0 = off, 1 = on. 1 - 255 could be used for doing extra with just switch
        /// </summary>
        public byte Value { get; }
    }
}
