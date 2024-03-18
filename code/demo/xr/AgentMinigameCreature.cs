using Godot;
namespace Rehab;
public partial class AgentMinigameCreature : AgentCreature
{

    [Export]
    float RoamRange = 2f;
    [Export]
    int PlayerGrappledAnimSlot = 8;
    [Export]
    int DamagedAnimSlot = 8;
    [Export]
    int DeadAnimSlot = 10;
    [Export]
    int WalkAnimSlot = 1;
    [Export]
    int DeadSoundSlot = -1;
    [Export]
    int IdleSoundSlot = -1;
    [Export]
    public bool CanBeGrappled = true;
    [Export]
    public bool CanBeTargeted = true;
    Vector3 SpawnPoint;
    Vector3 TargetPoint;
    CreatureState State = CreatureState.Idle;
    double StateTimer;
    Vector3 OrigScale;

    enum CreatureState
    {
        Idle = 0,
        Walking,
        Alerted,
        Grappled,
        Damaged,
        Dead,
    }

    public override void _Ready()
    {
        base._Ready();
        SpawnPoint = Position;
        OrigScale = Scale;
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;
        StateTimer -= delta;
        if (StateTimer < 0f)
        {
            DecideState();
        }
        switch (State)
        {
            default: break;
            case CreatureState.Damaged:
                TranslateObjectLocal(Vector3.Forward * (float)delta);
            break;
            case CreatureState.Grappled:
                if (Scale.X > OrigScale.X / 4f)
                {
                    Scale = Scale.MoveToward(OrigScale / 4f, (float)delta * 36f);
                }
            break;
            case CreatureState.Walking:
                Position = Position.MoveToward(TargetPoint, 1f * (float)delta);
            break;
        }
        
    }

    void DecideState()
    {
        switch (State)
        {
            default:
            case CreatureState.Grappled:
            break;
            case CreatureState.Damaged:
                State = CreatureState.Dead;
                StateTimer = 1d;
            break;
            case CreatureState.Dead:
                ProcessMode = ProcessModeEnum.Disabled;
                Visible = false;
            break;
            case CreatureState.Idle:
            case CreatureState.Alerted:
            case CreatureState.Walking:
                StateTimer = (System.Random.Shared.NextDouble() * 3f) + 0.5f;
                float X = SpawnPoint.X + (System.Random.Shared.NextSingle() * RoamRange);
                float Z = SpawnPoint.Z + (System.Random.Shared.NextSingle() * RoamRange);
                TargetPoint = new Vector3(X, Position.Y, Z);
                LookAt(TargetPoint);
                if (System.Random.Shared.Next(5) == 0)
                {
                    State = CreatureState.Idle;
                }
                else
                {
                    State = CreatureState.Walking;
                }
            break;
        }
        AnimState();
    }

    void AnimState()
    {
        switch (State)
        {
            default:
            case CreatureState.Idle:
                DoAnimation(1, true);
                if (System.Random.Shared.Next(5) == 0)
                {
                    DoSound(IdleSoundSlot, 1f, 0f);
                }
            break;
            case CreatureState.Damaged:
                DoAnimation(DamagedAnimSlot, true);
            break;
            case CreatureState.Dead:
                DoAnimation(DeadAnimSlot, false);
                DoSound(DeadSoundSlot, 1f, 0f);
            break;
            case CreatureState.Walking:
                DoAnimation(WalkAnimSlot, true);
                if (System.Random.Shared.Next(5) == 0)
                {
                    DoSound(IdleSoundSlot, 1f, 0f);
                }
            break;
            case CreatureState.Grappled:
                DoAnimation(PlayerGrappledAnimSlot, true);
                DoSound(IdleSoundSlot, 1f, 0f);
            break;
        }
    }

    public override void ForceDeath()
    {
        if (State == CreatureState.Damaged || State == CreatureState.Dead) return;
        State = CreatureState.Damaged;
        StateTimer = 1d;
        CanBeTargeted = false;
        AnimState();
    }

    public override void ForcePanic()
    {
        State = CreatureState.Grappled;
        StateTimer = 1d;
        CanBeTargeted = false;
        Set("collision_layer", 0);
        Set("collision_mask", 0);
        AnimState();
    }

}