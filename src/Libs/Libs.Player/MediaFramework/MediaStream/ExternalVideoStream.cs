namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

public class ExternalVideoStream : ExternalStream
{
    public double   FPS             { get; set; }
    public int      Height          { get; set; }
    public int      Width           { get; set; }

    public bool     HasAudio        { get; set; }
}
