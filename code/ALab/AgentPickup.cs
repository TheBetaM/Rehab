using Godot;
namespace Rehab;
public partial class AgentPickup : Agent
{
    bool isPickedUp;
    public bool isSpunAway;
    Vector3 SpinDirection = Vector3.One;
    float SpunTimer;
    Node3D pickupTarget = null;
    bool IsWumpa = true;
    public double CrateTimer;
    bool AnimMode = false;
    Vector3 UpPos = new(0f, 0.2f, 0f);
    Vector3 DownPos = new(0f, -0.2f, 0f);

    public override void _Ready()
    {
        base._Ready();

        RotationDegrees = new Vector3(0f, (GD.Randf() - 0.5f) * 360f, 0f);
        Set("collision_layer", 0);

        string name = (string)Name;
        if (!name.Contains("Pickup_Wumpa"))
            IsWumpa = false;
        else
        {
            CreateShadow(0, Vector2.One * 0.5f, 0);
            SubModels[ActiveModel].RotationDegrees = new Vector3(0f, (System.Random.Shared.NextSingle() * 360f) - 180f, 0f);
            SubModels[ActiveModel].Position = new Vector3(0f, (System.Random.Shared.NextSingle() * 0.4f) - 0.2f, 0f);
            if (System.Random.Shared.Next(2) == 0)
                AnimMode = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        SubModels[ActiveModel].RotateY(3f * (float)delta);
        if (CrateTimer > 0) CrateTimer -= delta;
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
            GlobalPosition = GlobalPosition.MoveToward(pickupTarget.GlobalPosition, 20f * (float)delta);
            if (GlobalPosition.DistanceTo(pickupTarget.GlobalPosition) < 0.1f)
            {
                RehabGame.AddWumpa(1);
                DoSound(1, (GD.Randf() / 5f) + 0.9f, 0f);
                ProcessMode = ProcessModeEnum.Disabled;;
                Visible = false;
            }
        }
        else if (IsWumpa)
        {
            if (AnimMode)
            {
                SubModels[ActiveModel].Position = SubModels[ActiveModel].Position.MoveToward(UpPos, 1f * (float)delta);
                if (SubModels[ActiveModel].Position.Y == UpPos.Y)
                    AnimMode = false;
            }
            else
            {
                SubModels[ActiveModel].Position = SubModels[ActiveModel].Position.MoveToward(DownPos, 1f * (float)delta);
                if (SubModels[ActiveModel].Position.Y == DownPos.Y)
                    AnimMode = true;
            }
        }
    }

    public void OnPickup(Node3D body)
    {
        if (isPickedUp) return;
        bool check = body is AgentCharacter;
        if (!check) return;
        if (IsWumpa && body is AgentCharacter agent)
        {
            if (agent.spinTimer > 0f && CrateTimer <= 0)
            {
                ForceSpun(body, false);
                return;
            }
        }
        ForcePickup(body);
    }

    public void ForceSpun(Node3D node, bool IsEnemy)
    {
        if (!IsWumpa)
        {
            if (IsEnemy) return;
            ForcePickup(node);
        }
        Set("collision_layer", 0);
        Set("collision_mask", 0);
        pickupTarget = node;
        isPickedUp = true;
        SpinDirection = GlobalPosition - node.GlobalPosition;
        SpinDirection = SpinDirection.Normalized();
        SpunTimer = 1f;
        isSpunAway = true;
        DoSound(2, (GD.Randf() / 5f) + 0.9f, 0f);
    }

    public void ForcePickup(Node3D node)
    {
        Set("collision_layer", 0);
        Set("collision_mask", 0);
        pickupTarget = node;
        isPickedUp = true;
        DoSound(1, 1f, 0f);
        if (IsWumpa) return;
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

    public override void OnNonSolidCollisionEnter(Node3D body)
    {
        OnPickup(body);
    }
}