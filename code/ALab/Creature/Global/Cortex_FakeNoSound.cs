using Godot;
namespace Rehab.Agents.Creature;

public partial class Cortex_FakeNoSound : AgentCreature
{
    public override void _Ready()
    {
        base._Ready();

        DoAnimation(1, true);
        CreateShadow(0, Vector2.One, 0);
    }
}