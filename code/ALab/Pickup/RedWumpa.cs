using Godot;
namespace Rehab.Agents.Pickup;

public partial class RedWumpa : AgentPickup
{

    public override void PickupPostSpawn()
    {
        IsWumpa = true;
        CreateShadow(0, Vector2.One * 0.5f, 0);
        SubModels[ActiveModel].RotationDegrees = new Vector3(0f, (System.Random.Shared.NextSingle() * 360f) - 180f, 0f);
        SubModels[ActiveModel].Position = new Vector3(0f, (System.Random.Shared.NextSingle() * 0.4f) - 0.2f, 0f);
        if (System.Random.Shared.Next(2) == 0)
            AnimMode = true;
    }
}