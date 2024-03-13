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
        var handL = RehabScene.Root.XR_Origin.XR_HandL;
        var handR = RehabScene.Root.XR_Origin.XR_HandR;
        handL.ToggleHandCollision(true);
        handR.ToggleHandCollision(true);
    }

    public override void AnimDone(StringName anim)
    {
        if (!CutsceneActive) return;
        CutsceneActive = false;
        ReorientXRCamNode(AgentCharacter.activeCharacter);
        AnimPlayer.Play(EndAnimName);
        var handL = RehabScene.Root.XR_Origin.XR_HandL.HandModel;
        var handR = RehabScene.Root.XR_Origin.XR_HandR.HandModel;
        var handLcol = RehabScene.Root.XR_Origin.XR_HandL;
        var handRcol = RehabScene.Root.XR_Origin.XR_HandR;
        handLcol.ToggleHandCollision(false);
        handRcol.ToggleHandCollision(false);
        handL.Reattach();
        handR.Reattach();
        RehabScene.Root.PlayMusic(MinigameMusicID);
    }

    public void PauseAnim(string path)
    {
        //GetNode<AnimationPlayer>(path).Pause();
        GetNode<AnimationPlayerXR>(path).PauseCutscene();
    }

    public void PlayAnim(string path, float pos)
    {
        //GetNode<AnimationPlayer>(path).Play(anim);
        GetNode<AnimationPlayerXR>(path).UnPauseCutscene(pos);
    }
}