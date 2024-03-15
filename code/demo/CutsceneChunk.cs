using Godot;
namespace Rehab;
public partial class CutsceneChunk : ChunkScene
{
    [Export]
    public NodePath AnimPlayerPath;
    [Export]
    public NodePath AnimCameraPath;
    [Export]
    public string AnimName = "cutscene";
    [Export]
    public bool XR_CameraFollow = false;
    public AnimationPlayer AnimPlayer;
    public Camera3D AnimCamera;
    public Vector3 LastCamPos;
    public bool CutsceneActive = false;

    public override void InitGame()
    {
        RehabScene.Root.StopMusic();
        RehabScene.GameHUD.OnUnPause();
        if (RehabGame.VoiceLang != RehabGame.VoiceLanguage.English)
        {
            NestedAudioUpdate(this);
        }
        AnimPlayer = GetNode<AnimationPlayer>(AnimPlayerPath);
        AnimCamera = GetNode<Camera3D>(AnimCameraPath);
        AnimCamera.Current = true;
        if (RehabScene.Root.XR_Enabled)
        {
            LastCamPos = AnimCamera.GlobalPosition;
            RehabScene.Root.XR_Origin.XR_Camera.Position = Vector3.Zero;
            RehabScene.Root.XR_Origin.GlobalPosition = AnimCamera.GlobalPosition;
            RehabScene.Root.XR_Origin.GlobalRotation = AnimCamera.GlobalRotation;
            XRServer.CenterOnHmd(XRServer.RotationMode.ResetButKeepTilt, true);
            RehabScene.Root.XR_Origin.ResetOrientation();
        }
        StartCutscene();
    }

    public async void StartCutscene()
    {
        CutsceneActive = true;
        if (RehabScene.Root.XR_Enabled)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        if (AnimPlayer is AnimationPlayerXR xr)
        {
            xr.XR_StartSequence(0);
        }
        else
        {
            AnimPlayer.AnimationFinished += AnimDone;
            AnimPlayer.Play(AnimName);
        }
    }

    public override void _Process(double delta)
    {
        if (!CutsceneActive) return;
        if (Input.IsActionJustPressed("pad1_start"))
        {
            RehabScene.Root.StartPauseMenu(false);
            return;
        }
        if (!RehabScene.Root.XR_Enabled) return;
        if (XR_CameraFollow)
        {
            RehabScene.Root.XR_Origin.GlobalPosition = AnimCamera.GlobalPosition;
        }
        else
        {
            if (LastCamPos.DistanceTo(AnimCamera.Position) > 4f)
            {
                ReorientXRCam();
            }
            LastCamPos = AnimCamera.Position;
        }
    }

    public async void ReorientXRCam()
    {
        RehabScene.Root.Visible = false;
        LastCamPos = AnimCamera.GlobalPosition;
        ProcessMode = ProcessModeEnum.Disabled;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        RehabScene.Root.XR_Origin.XR_Camera.Position = Vector3.Zero;
        RehabScene.Root.XR_Origin.GlobalPosition = AnimCamera.GlobalPosition;
        RehabScene.Root.XR_Origin.GlobalRotation = AnimCamera.GlobalRotation;
        XRServer.CenterOnHmd(XRServer.RotationMode.ResetButKeepTilt, true);
        RehabScene.Root.XR_Origin.ResetOrientation();
        await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);
        RehabScene.Root.Visible = true;
        ProcessMode = ProcessModeEnum.Inherit;
    }

    public async void ReorientXRCamNode(Node3D node)
    {
        RehabScene.Root.Visible = false;
        LastCamPos = AnimCamera.GlobalPosition;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        RehabScene.Root.XR_Origin.XR_Camera.Position = Vector3.Zero;
        RehabScene.Root.XR_Origin.GlobalPosition = node.GlobalPosition + (Vector3.Up * 0.75f);
        RehabScene.Root.XR_Origin.GlobalRotationDegrees = new Vector3(node.GlobalRotationDegrees.X, node.GlobalRotationDegrees.Y + 180f, node.GlobalRotationDegrees.Z);
        XRServer.CenterOnHmd(XRServer.RotationMode.ResetButKeepTilt, true);
        RehabScene.Root.XR_Origin.ResetOrientation();
        await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);
        RehabScene.Root.Visible = true;
    }

    public void ReorientXRCamInstant()
    {
        LastCamPos = AnimCamera.GlobalPosition;
        RehabScene.Root.XR_Origin.XR_Camera.Position = Vector3.Zero;
        RehabScene.Root.XR_Origin.GlobalPosition = AnimCamera.GlobalPosition;
        RehabScene.Root.XR_Origin.GlobalRotation = AnimCamera.GlobalRotation;
        XRServer.CenterOnHmd(XRServer.RotationMode.ResetButKeepTilt, true);
        RehabScene.Root.XR_Origin.ResetOrientation();
    }

    public virtual void AnimDone(StringName anim)
    {
        CutsceneActive = false;
        RehabScene.Root.Visible = true;
        RehabScene.Root.ExitLevel(false);
    }

    void NestedAudioUpdate(Node parent)
    {
        if (parent is AudioStreamPlayer audio)
        {
            if (audio.Stream != null && audio.Stream.ResourcePath.Contains("English"))
            {
                string dirPath = RehabGame.GetVoicePath();
                string audioPath = audio.Stream.ResourcePath.Replace("English", dirPath);
                if (ResourceLoader.Exists(audioPath))
                {
                    audio.Stream = (AudioStream)ResourceLoader.Load(audioPath);
                }
            }
        }
        else if (parent is AudioStreamPlayer2D audio2)
        {
            if (audio2.Stream != null && audio2.Stream.ResourcePath.Contains("English"))
            {
                string dirPath = RehabGame.GetVoicePath();
                string audioPath = audio2.Stream.ResourcePath.Replace("English", dirPath);
                if (ResourceLoader.Exists(audioPath))
                {
                    audio2.Stream = (AudioStream)ResourceLoader.Load(audioPath);
                }
            }
        }
        else if (parent is AudioStreamPlayer3D audio3)
        {
            if (audio3.Stream != null && audio3.Stream.ResourcePath.Contains("English"))
            {
                string dirPath = RehabGame.GetVoicePath();
                string audioPath = audio3.Stream.ResourcePath.Replace("English", dirPath);
                if (ResourceLoader.Exists(audioPath))
                {
                    audio3.Stream = (AudioStream)ResourceLoader.Load(audioPath);
                }
            }
        }
        foreach (var a in parent.GetChildren())
        {
            NestedAudioUpdate(a);
        }
    }
}