using Godot;
namespace Rehab;
public partial class MinigameChunk : CutsceneChunk
{

    [Export]
    public bool PortraitMode;
    [Export]
    public string EndAnimName = "cutscene_end";
    [Export]
    public int MinigameMusicID = 54;

    public override void _Ready()
    {
        base._Ready();
        if (PortraitMode && OS.GetName() == "Android" && !RehabScene.Root.XR_Enabled)
        {
            DisplayServer.ScreenSetOrientation(DisplayServer.ScreenOrientation.SensorPortrait);
        }
    }

    public override void AnimDone(StringName anim)
    {
        if (!CutsceneActive) return;
        CutsceneActive = false;
        AnimPlayer.Play(EndAnimName);
        RehabScene.PlayerCam.Current = true;
        RehabScene.Root.PlayMusic(MinigameMusicID);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pad1_start"))
        {
            RehabScene.Root.StartPauseMenu(false);
            return;
        }
        if (Input.IsActionJustPressed("pad1_triangle"))
        {
            AnimDone("");
            return;
        }
        if (XR_CameraFollow && CutsceneActive)
        {
            RehabScene.Root.XR_Origin.GlobalPosition = AnimCamera.GlobalPosition;
        }
    }

}