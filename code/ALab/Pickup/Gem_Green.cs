using Godot;
namespace Rehab.Agents.Pickup;

public partial class Gem_Green : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddGem(2);
    }
}