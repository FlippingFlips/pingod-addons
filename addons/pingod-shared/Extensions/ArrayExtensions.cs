using System;
using System.Linq;

public static class ArrayExtensions
{
    /// <summary>
    /// Randomizes an array and returns a given length
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static int[] ShuffleArray(this int[] arr, int length)
    {
        Random random = new Random();
        return arr.OrderBy(x => random.Next()).Take(length).ToArray();
    }
}
