using Godot;
namespace Rehab.Agents.Pickup;
public partial class ExtraLife : AgentPickup
{
    public override void GotPickup()
    {
        RehabGame.AddLives(1);
    }
}