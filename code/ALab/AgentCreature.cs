using Godot;
namespace Rehab;
public partial class AgentCreature : Agent
{
    enum AngleSlot{
        UnkAngle = 0,
    }
    enum FloatSlot{
        UnkFloat1 = 0,
        UnkFloat2 = 1,
        UnkFloat3 = 2,
        UnkFloat4 = 3,
        UnkFloat5 = 4,
        UnkFloat6 = 5,
    }
    enum IntSlot{
        AgentType = 0,
        UnkInt1 = 1,
        UnkInt2 = 2,
    }

    public override void _Ready()
    {
        base._Ready();

        DoAnimation(1, true);
	    CreateShadow(0, Vector2.One, 0);
    }

    public virtual void ForceDeath()
    {
        ForceDeathAsync();
    }

    async void ForceDeathAsync()
    {
        DoAnimation(11, true);
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        DoAnimation(4, false);
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        ProcessMode = ProcessModeEnum.Disabled;
        Visible = false;
    }

    public virtual void ForcePanic()
    {
        Scale = Scale / 4f;
        DoAnimation(1, true);
        Set("collision_layer", 0);
        Set("collision_mask", 0);
    }
    
}