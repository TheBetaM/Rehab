using Godot;
namespace Rehab;
public partial class MinigameChunk : ChunkScene
{

    [Export]
    public bool PortraitMode;

    public override void _Ready()
    {
        if (PortraitMode && OS.GetName() == "Android")
        {
            DisplayServer.ScreenSetOrientation(DisplayServer.ScreenOrientation.SensorPortrait);
        }
    }

}