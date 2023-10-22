using System;

/// <summary> Model </summary>
public partial class HighScore
{
    /// <summary> Player </summary>
    public string Name { get; set; }

    /// <summary> HighScore Title </summary>
    public string Title { get; set; }

    /// <summary> Amount </summary>
    public long Points { get; set; }

    /// <summary> Created </summary>
    public DateTime Created { get; set; }
}