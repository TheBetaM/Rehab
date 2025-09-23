using Godot;
namespace Rehab;
public partial class AgentCrate : Agent
{
    [Export] public bool OutlineCrate;
    public bool IsBroken;
    public bool WasTriggered;
    static SphereShape3D ExplosionShape;
    public virtual int BreakAnimSlot => 7;
    public virtual bool BreakOnTouch => false;
    public virtual bool IsReinforced => false;
    public virtual bool IsBreakable => true;

    public AgentCrate()
    {
        if (ExplosionShape == null)
        {
            ExplosionShape = new SphereShape3D
            {
                Radius = 2.0f//3.5f
            };
        }
    }

    public override void _Ready()
    {
        base._Ready();

        CreateShadow(1, Vector2.One, 0);

        if (OutlineCrate)
        {
            Set("collision_layer", 0);
            Set("collision_mask", 0);
            NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Disabled;
            DoAnimation(3, false);
        }
    }

    public override void OnNonSolidCollisionEnter(Node3D body)
    {
        OnBodyEntered(body);
    }

    public void OnBodyEntered(Node3D body)
    {
        if (body is AgentCharacter agent)
        {
            if (agent.spinTimer > 0f || agent.slideTimer > 0f || BreakOnTouch)
            {
                if (!IsReinforced)
                    CallDeferred("Crate_ForceBreak");
                return;
            }
        }
    }

    public void OnBodyExited(Node3D body)
    {

    }

    public virtual void Crate_ForceBreak()
    {
        if (!IsBreakable || IsBroken) return;
        IsBroken = true;
        Set("collision_layer", 0);
        Set("collision_mask", 0);
        DoAnimation(BreakAnimSlot, false);
        DoSound(1, 1f, -5.0f);
        GetNode<Node3D>("Shadows").Visible = false;
        Crate_AfterForceBreak();
    }
    public virtual void Crate_AfterForceBreak()
    {

    }

    public async void DelayedExplosion()
    {
        await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters3D();
        query.Shape = ExplosionShape;
        query.Transform = query.Transform.Translated(GlobalPosition + new Vector3(0f, 0.5f, 0f));
        var result = spaceState.IntersectShape(query);
        foreach (var item in result)
        {
            if (!item.ContainsKey("collider")) continue;
            var hit = (GodotObject)item["collider"];
            if (hit is AgentCrate crate)
            {
                crate.CallDeferred("Crate_ForceBreak");
            }
            else if (hit is AgentCharacter player)
            {
                // todo
            }
            else if (hit is AgentPickup pickup)
            {
                pickup.ForceSpun(this, true);
            }
        }
    }

    public void OnTrigger()
    {
        WasTriggered = true;
        if (OutlineCrate)
        {
            DoAnimation(0, false);
            UpdateLayers(ParentScene.ChunkLayer);
        }
        DoSound(2, 1f, 0f);
        Crate_AfterTrigger();
        DelayedTrigger();
    }

    public virtual void Crate_AfterTrigger()
    {

    }

    public async void DelayedTrigger()
    {
        await ToSignal(GetTree().CreateTimer(0.15f), SceneTreeTimer.SignalName.Timeout);
        if (LinkInstance == null) return;
        foreach (var item in LinkInstance)
        {
            if (item is AgentCrate acrate)
            {
                acrate.OnTrigger();
            }
            else if (item is AgentInstance inst && inst.Actor is AgentCrate crate)
            {
                crate.OnTrigger();
            }
        }
    }

    
}