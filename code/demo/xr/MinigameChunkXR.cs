using Godot;
namespace Rehab;
public partial class MinigameChunkXR : MinigameChunk
{
    public override void InitGame()
    {
        base.InitGame();
        HandCol();
    }

    async void HandCol()
    {
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        if (!CutsceneActive) return;
        var handL = RehabScene.Root.XR_Origin.XR_HandL;
        var handR = RehabScene.Root.XR_Origin.XR_HandR;
        handL.ToggleHandCollision(true);
        handR.ToggleHandCollision(true);
        handL.HandModel.IsRestricted = true;
        handR.HandModel.IsRestricted = true;
    }

    public override void AnimDone(StringName anim)
    {
        if (!CutsceneActive) return;
        CutsceneActive = false;
        ReorientXRCamNode(AgentCharacter.activeCharacter);
        AnimPlayer.Play(EndAnimName);
        var handL = RehabScene.Root.XR_Origin.XR_HandL;
        var handR = RehabScene.Root.XR_Origin.XR_HandR;
        handL.ToggleHandCollision(false);
        handR.ToggleHandCollision(false);
        handL.HandModel.Reattach();
        handR.HandModel.Reattach();
        handL.HandModel.IsRestricted = false;
        handR.HandModel.IsRestricted = false;
        RehabScene.Root.PlayMusic(MinigameMusicID);
        ChunkEnter();
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