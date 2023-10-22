using System.Collections.Generic;
using System.Text.Json;

namespace PinGod.Base
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PinStates : Dictionary<string, PinStateObject>
    {
        /// <summary>
        /// Gets all states with the number [,2(state)]
        /// </summary>
        /// <returns></returns>
        public byte[,] GetStates()
        {
            if (Count <= 0) return null;
            byte[,] arr = new byte[Count, 2];
            int i = 0;
            foreach (var item in Values)
            {
                arr[i, 0] = item.Num;
                arr[i, 1] = item.State;
                i++;
            }
            return arr;
        }

        /// <summary>
        /// Run through all but only assign what we have in dictionary
        /// </summary>
        /// <param name="stateCount"></param>
        /// <returns></returns>
        public byte[] GetStatesArray(int stateCount = 32)
        {
            byte[] arr = new byte[stateCount];
            foreach (var item in Values)
            {
                if (item.Num * 2 <= arr.Length)
                {
                    arr[item.Num * 2] = item.Num;
                    arr[item.Num * 2 + 1] = item.State;
                }
            }
            return arr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateCount"></param>
        /// <returns></returns>
        public int[] GetLedStatesArray(int stateCount = 64)
        {
            int[] arr = new int[stateCount];
            foreach (var item in Values)
            {
                if (item.Num * 3 <= arr.Length)
                {
                    arr[item.Num * 3] = item.Num;
                    arr[item.Num * 3 + 1] = item.State;
                    arr[item.Num * 3 + 2] = item.Color;
                }
            }

            return arr;
        }

        /// <summary>
        /// Gets all states with the number [num,3]
        /// </summary>
        /// <returns>[num, state, colour]</returns>
        public int[,] GetLedStates()
        {
            if (Count <= 0) return null;

            int[,] arr = new int[Count, 3];
            int i = 0;
            foreach (var item in Values)
            {
                arr[i, 0] = item.Num;
                arr[i, 1] = item.State;
                arr[i, 2] = item.Color;
                i++;
            }
            return arr;
        }

        /// <summary>
        /// Slow
        /// </summary>
        /// <returns></returns>
        public string GetStatesJson()
        {
            if (Keys.Count > 0)
            {
                var states = GetStates();
                if (states != null)
                    return JsonSerializer.Serialize(states);
            }

            return string.Empty;
        }

        /// <summary>
        /// Slow?
        /// </summary>
        /// <returns></returns>
        public string GetLedStatesJson()
        {
            if (Keys.Count > 0)
            {
                var states = GetLedStates();
                if (states != null)
                    return JsonSerializer.Serialize(states);
            }

            return string.Empty;
        }
    }
}