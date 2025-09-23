using Godot;
namespace Rehab.Agents.Crate;

public partial class TNT : AgentCrate
{
    public override int BreakAnimSlot => 4;

    public override void Crate_AfterForceBreak()
    {
        DelayedExplosion();
    }
}