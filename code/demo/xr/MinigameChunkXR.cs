using Godot;
namespace Rehab;
public partial class MinigameChunkXR : MinigameChunk
{
    public override void _Ready()
    {
        base._Ready();
        HandCol();
    }

    async void HandCol()
    {
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        var handL = (RehabXRHand)RehabScene.Root.XR_Origin.XR_HandL.HandModel;
        var handR = (RehabXRHand)RehabScene.Root.XR_Origin.XR_HandR.HandModel;
        handL.ToggleHandCollisions(true);
        handR.ToggleHandCollisions(true);
    }

    public override void AnimDone(StringName anim)
    {
        CutsceneActive = false;
        var handL = (RehabXRHand)RehabScene.Root.XR_Origin.XR_HandL.HandModel;
        var handR = (RehabXRHand)RehabScene.Root.XR_Origin.XR_HandR.HandModel;
        handL.ToggleHandCollisions(false);
        handR.ToggleHandCollisions(false);
        ReorientXRCam();
    }

    public void PauseAnim(string path)
    {
        //GetNode<AnimationPlayer>(path).Pause();
        GetNode<AnimationPlayerXR>(path).XR_Pause();
    }

    public void PlayAnim(string path, float pos)
    {
        //GetNode<AnimationPlayer>(path).Play(anim);
        GetNode<AnimationPlayerXR>(path).XR_UnPause(pos);
    }
}