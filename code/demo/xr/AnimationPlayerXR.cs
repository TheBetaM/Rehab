using Godot;
namespace Rehab;
public partial class AnimationPlayerXR : AnimationPlayer
{
    [Export]
    public string[] AnimSequences;
    [Export]
    public string EndAnimName = "cutscene_end";
    public CutsceneChunk ParentScene;
    int AnimSequenceID = 0;
    bool IsBusy;

    public override void _Ready()
    {
        var parent = GetParent();
        while (ParentScene == null && parent != null)
        {
            if (parent is CutsceneChunk chunk)
                ParentScene = chunk;
            else
                parent = parent.GetParent();
        }
        
        AnimSequenceID = 0;
    }

    public void XR_StartSequence(int id)
    {
        if (IsBusy) return;
        IsBusy = true;
        AnimSequenceID = id;
        Play(AnimSequences[id]);
        UnBusy();
    }

    public void XR_Next()
    {
        if (IsBusy) return;
        IsBusy = true;
        AnimSequenceID++;
        Play(AnimSequences[AnimSequenceID]);
        UnBusy();
    }

    async void UnBusy()
    {
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        IsBusy = false;
    }

    public void XR_Pause()
    {
        Pause();
    }
    public void XR_UnPause(float pos)
    {
        // unpausing an animation is currently bugged
        Play();
        Advance(pos);//Advance(13.74d);
        Active = true;
    }

    public void XR_End()
    {
        Play("cutscene_end");
        ParentScene.AnimDone("cutscene");
    }
}