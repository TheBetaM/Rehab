using Godot;
namespace Rehab;
public partial class AgentCreature : Agent
{
    enum AngleSlot{
        TurnSpeed = 0,
    }
    enum FloatSlot{
        GravityStrength = 0,
        MovementSpeed = 1,
        StartFallSpeed = 2,
        TerminalVelocity = 3,
        ClampAboveGroundHeight = 4,
    }
    enum IntSlot{
        HitPoints = 0,
    }

    public override void _Ready()
    {
        base._Ready();

        // temp
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