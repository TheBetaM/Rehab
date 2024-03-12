using Godot;
namespace Rehab;
public partial class MinigameChunk : CutsceneChunk
{

    [Export]
    public bool PortraitMode;
    [Export]
    public string EndAnimName = "cutscene_end";

    public override void _Ready()
    {
        base._Ready();
        if (PortraitMode && OS.GetName() == "Android")
        {
            DisplayServer.ScreenSetOrientation(DisplayServer.ScreenOrientation.SensorPortrait);
        }
    }

    public override void AnimDone(StringName anim)
    {
        CutsceneActive = false;
        RehabScene.PlayerCam.Current = true;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pad1_start"))
        {
            RehabScene.Root.StartPauseMenu(false);
            return;
        }
        if (XR_CameraFollow && CutsceneActive)
        {
            RehabScene.Root.XR_Origin.GlobalPosition = AnimCamera.GlobalPosition;
        }
    }

}