using Godot;
namespace Rehab.Agents.Pickup;

public partial class Gem_Red : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddGem(4);
    }
}