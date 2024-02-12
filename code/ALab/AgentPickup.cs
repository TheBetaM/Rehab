using Godot;
namespace Rehab;
public partial class AgentPickup : Agent
{
    bool isPickedUp;
    bool isSpunAway;
    Vector3 SpinDirection = Vector3.One;
    float SpunTimer;
    Node3D pickupTarget = null;
    bool IsWumpa = true;

    public override void _Ready()
    {
        base._Ready();

        RotationDegrees = new Vector3(0f, (GD.Randf() - 0.5f) * 360f, 0f);
        Set("contact_monitor", true);
        Set("max_contacts_reported", 256);
        Set("collision_layer", 0);
        Set("freeze_mode", (int) RigidBody3D.FreezeModeEnum.Kinematic);
        Connect("body_entered", Callable.From<Node3D>(OnPickup));
        if (Name != "Pickup_Wumpa")
            IsWumpa = false;
        else
            CreateShadow(0, Vector2.One * 0.5f, 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        RotateY(3f * (float)delta);
        if (!Visible) ProcessMode = ProcessModeEnum.Disabled;
        if (isPickedUp)
        {
            if (isSpunAway)
            {
                GlobalPosition += SpinDirection * (float)delta * 30f;
                SpunTimer -= (float)delta;
                if (SpunTimer <= 0.0)
                {
                    ProcessMode = ProcessModeEnum.Disabled;
                    Visible = false;
                }
                return;
            }
            if (pickupTarget == null)
            {
                ProcessMode = ProcessModeEnum.Disabled;
                Visible = false;
                return;
            }
            GlobalPosition = GlobalPosition.MoveToward(pickupTarget.GlobalPosition, 15f * (float)delta);
            if (GlobalPosition.DistanceTo(pickupTarget.GlobalPosition) < 0.1f)
            {
                RehabGame.AddWumpa(1);
                DoSound(1, (GD.Randf() / 5f) + 0.9f, 0f);
                ProcessMode = ProcessModeEnum.Disabled;;
                Visible = false;
            }
        }
    }

    public void OnPickup(Node3D body)
    {
        if (isPickedUp) return;
        bool check = body is AgentCharacter;
        if (!check) return;
        AgentCharacter agent = (AgentCharacter)body;
        Set("collision_layer", 0);
        Set("collision_mask", 0);
        pickupTarget = body;
        isPickedUp = true;
        if (IsWumpa)
        {
            if (agent.spinTimer > 0f)
            {
                SpinDirection = GlobalPosition - body.GlobalPosition;
                SpinDirection = SpinDirection.Normalized();
                SpunTimer = 1f;
                isSpunAway = true;
                DoSound(2, (GD.Randf() / 5f) + 0.9f, 0f);
            }
            return;
        }
        DoSound(1, 1f, 0f);
        Visible = false;
        switch (Name)
        {
            case "Pickup_Crystal": RehabGame.AddCrystal(); break;
            case "Pickup_Gem_Blue": RehabGame.AddGem(0); break;
            case "Pickup_Gem_Clear": RehabGame.AddGem(1); break;
            case "Pickup_Gem_Green": RehabGame.AddGem(2); break;
            case "Pickup_Gem_Purple": RehabGame.AddGem(3); break;
            case "Pickup_Gem_Red": RehabGame.AddGem(4); break;
            case "Pickup_Gem_Yellow": RehabGame.AddGem(5); break;
            case "Pickup_ExtraLife":
            case "Pickup_ExtraLifeCortex":
            case "Pickup_ExtraLifeNina": RehabGame.AddLives(1); break;
            default: break;
        }
    }
}