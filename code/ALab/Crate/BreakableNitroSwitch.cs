using Godot;
namespace Rehab.Agents.Crate;

public partial class BreakableNitroSwitch : IronSwitch
{
    public override bool IsBreakable => true;
}