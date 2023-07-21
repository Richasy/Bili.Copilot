namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

public class ExternalSubtitleStream : ExternalStream
{
    /// <summary>
    /// Initial Url. Url can be the converted format from the original input
    /// </summary>
    public string   DirectUrl       { get; set; }

    public bool     Converted       { get; set; }
    public bool     Downloaded      { get; set; }
    public Language Language        { get; set; } = Language.Unknown;
    public float    Rating          { get; set; } // 1.0-10.0 (0: not set)
    // TODO: Add confidence rating (maybe result is for other movie/episode) | Add Weight calculated based on rating/downloaded/confidence (and lang?) which can be used from suggesters
    public string   Title           { get; set; }
}
