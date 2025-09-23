using Godot;
namespace Rehab.Agents.Crate;

public partial class Nitro : AgentCrate
{
    public override int BreakAnimSlot => 4;
    public override bool BreakOnTouch => true;

    public override void Crate_AfterForceBreak()
    {
        DelayedExplosion();
    }
}