using Godot;
namespace Rehab.Agents.Pickup;

public partial class Gem_Yellow : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddGem(5);
    }
}