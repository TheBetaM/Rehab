using Godot;
namespace Rehab.Agents.Pickup;

public partial class Gem_Purple : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddGem(3);
    }
}