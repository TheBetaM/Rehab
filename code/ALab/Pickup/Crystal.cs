using Godot;
namespace Rehab.Agents.Pickup;

public partial class Crystal : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddCrystal();
    }
}