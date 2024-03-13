using Godot;
namespace Rehab;
public partial class AnimationPlayerCutscene : AnimationPlayer
{
    public CutsceneChunk ParentScene;

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
    }

    public void PauseCutscene()
    {
        Pause();
    }
    public void UnPauseCutscene(float pos)
    {
        // unpausing an animation is currently bugged
        Play();
        Advance(pos);
        Active = true;
    }
    public void ForceLoop()
    {
        GetAnimation(CurrentAnimation).LoopMode = Animation.LoopModeEnum.Linear;
    }
    public void CustomPlay(StringName name = null, double blend = -1, float speed = 1, bool fromEnd = false)
    {
        Play(name, blend, speed, fromEnd);
    }
}