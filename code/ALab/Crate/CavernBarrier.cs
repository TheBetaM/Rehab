using Godot;
namespace Rehab.Agents.Crate;

public partial class CavernBarrier : AgentCrate
{
    public override int BreakAnimSlot => 1;
    public override bool BreakOnTouch => true;
}