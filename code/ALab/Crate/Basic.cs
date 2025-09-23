using Godot;
namespace Rehab.Agents.Crate;

public partial class Basic : AgentCrate
{
    public override void Crate_AfterForceBreak()
    {
        if (SubActorsScenes != null && SubActorsScenes.Count != 0)
        {
            var item = (Agent)SubActorsScenes[0].Instantiate();
            if (item is AgentPickup pickup)
            {
                pickup.CrateTimer = 1.0f;
            }
            AddChild(item);
        }
    }
}