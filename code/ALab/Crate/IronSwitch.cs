using Godot;
namespace Rehab.Agents.Crate;

public partial class IronSwitch : AgentCrate
{
    public override bool IsBreakable => false;

    public override void Crate_ForceBreak()
    {
        if (!WasTriggered)
        {
            OnTrigger();
        }
    }

    public override void Crate_AfterTrigger()
    {
        DoAnimation(6, false);
    }
}