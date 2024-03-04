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
    AnimationPlayer AnimPlayer;
    Camera3D AnimCamera;

    public override void _Ready()
    {
        RehabScene.Root.StopMusic();
        if (RehabGame.VoiceLang != RehabGame.VoiceLanguage.English)
        {
            NestedAudioUpdate(this);
        }
        AnimPlayer = GetNode<AnimationPlayer>(AnimPlayerPath);
        AnimCamera = GetNode<Camera3D>(AnimCameraPath);
        AnimPlayer.AnimationFinished += AnimDone;
        AnimCamera.Current = true;
        AnimPlayer.Play(AnimName);
    }

    void AnimDone(StringName anim)
    {
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