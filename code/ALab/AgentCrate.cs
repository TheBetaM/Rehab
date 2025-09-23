using Godot;
namespace Rehab;
public partial class AgentCrate : Agent
{
    public bool IsBroken;
    bool IsCheckPoint;
    bool IsBreakable = true;
    bool IsNitro;
    bool IsTNT;
    bool IsSwitch;
    bool IsReinforced;
    bool WasTriggered;
    static SphereShape3D ExplosionShape;

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

        string name = (string)Name;
        if (name.Contains("CheckPoint") || name.Contains("Level"))
            IsCheckPoint = true;
        else if (name.Contains("Iron") || name.Contains("Detonator"))
            IsBreakable = false;
        else if (name.Contains("Nitro"))
            IsNitro = true;
        else if (name.Contains("TNT"))
            IsTNT = true;
        else if (name.Contains("Reinforced"))
            IsReinforced = true;
        if (name.Contains("Switch"))
            IsSwitch = true;
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
            if (agent.spinTimer > 0f || agent.slideTimer > 0f || IsNitro || IsCheckPoint)
            {
                if (!IsReinforced)
                    CallDeferred("ForceBreak");
                return;
            }
        }
    }

    public void OnBodyExited(Node3D body)
    {

    }

    public void ForceBreak()
    {
        if (IsSwitch && !WasTriggered)
        {
            OnTrigger();
        }
        if (!IsBreakable) return;
        if (IsBroken) return;
        IsBroken = true;
        Set("collision_layer", 0);
        Set("collision_mask", 0);
        int slot = 7;
        if (IsNitro || IsTNT) slot = 4;
        if (IsCheckPoint) slot = 5;
        DoAnimation(slot, false);
        DoSound(1, 1f, -5.0f);
        if (SubActorsScenes != null && SubActorsScenes.Count != 0)
        {
            var item = (Agent)SubActorsScenes[0].Instantiate();
            if (item is AgentPickup pickup)
            {
                pickup.CrateTimer = 1.0f;
            }
            AddChild(item);
        }
        GetNode<Node3D>("Shadows").Visible = false;
        if (IsCheckPoint)
        {
            RehabGame.SetCheckPoint(GlobalPosition, GlobalRotationDegrees, ParentScene.Name, ((string)Name).Contains("Level"));
        }
        if (IsNitro || IsTNT)
        {
            DelayedExplosion();
        }
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
                crate.CallDeferred("ForceBreak");
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
        if (IsSwitch)
        {
            DoAnimation(6, false);
        }
        DoSound(2, 1f, 0f);
        DelayedTrigger();
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

    public override void OnMessage(int id)
    {
        if (IsCheckPoint && id == 138)
        {
            CallDeferred("ForceBreak");
        }
    }
}