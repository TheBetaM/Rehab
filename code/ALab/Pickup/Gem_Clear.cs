using Godot;
namespace Rehab.Agents.Pickup;

public partial class Gem_Clear : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddGem(1);
    }
}