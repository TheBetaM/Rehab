using Godot;
namespace Rehab.Agents.Pickup;

public partial class Gem_Blue : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddGem(0);
    }
}