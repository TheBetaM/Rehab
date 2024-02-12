using Godot;
namespace Rehab;
public partial class AgentCrate : Agent
{
    enum FloatSlot{
        UnkFloat1 = 0,
        UnkFloat2 = 0,
        UnkFloat3 = 0,
    }

    public override void _Ready()
    {
        base._Ready();

        CreateShadow(1, Vector2.One, 0);
    }
}