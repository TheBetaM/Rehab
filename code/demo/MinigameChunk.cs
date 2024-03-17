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
    [Export]
    public int MinigameAmbID = 112;

    public override void InitGame()
    {
        base.InitGame();
        RehabGame.ResetGame();
        if (PortraitMode && OS.GetName() == "Android" && !RehabScene.Root.XR_Enabled)
        {
            DisplayServer.ScreenSetOrientation(DisplayServer.ScreenOrientation.SensorPortrait);
        }
        RehabScene.Root.PlayAmbience(MinigameAmbID);
    }

    public override void AnimDone(StringName anim)
    {
        if (!CutsceneActive) return;
        CutsceneActive = false;
        AnimPlayer.Play(EndAnimName);
        RehabScene.PlayerCam.Current = true;
        RehabScene.Root.PlayMusic(MinigameMusicID);
        ChunkEnter();
    }

    public override void _Process(double delta)
    {
        if (!CutsceneActive || !ActiveScene) return;
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
        if (XR_CameraFollow)
        {
            RehabScene.Root.XR_Origin.GlobalPosition = AnimCamera.GlobalPosition;
        }
    }

}