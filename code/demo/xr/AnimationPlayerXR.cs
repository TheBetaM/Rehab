using Godot;
namespace Rehab;
public partial class AnimationPlayerXR : AnimationPlayerCutscene
{
    [Export]
    public string[] AnimSequences;
    [Export]
    public string EndAnimName = "cutscene_end";
    public int AnimSequenceID = 0;
    public bool IsBusy;

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

    public void XR_End()
    {
        ParentScene.AnimDone("cutscene");
    }

    public async void UnBusy()
    {
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        IsBusy = false;
    }
}