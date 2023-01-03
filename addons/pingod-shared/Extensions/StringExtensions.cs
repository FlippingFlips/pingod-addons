using System.Globalization;

public static class StringExtensions
{
    /// <summary>
    /// Formats the score
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static string ToScoreString(this long points) => points.ToString("N0", CultureInfo.InvariantCulture);

    /// <summary>
    /// Formats the score
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static string ToScoreString(this int points) => points.ToString("N0", CultureInfo.InvariantCulture);
}
