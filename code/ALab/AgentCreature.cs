using Godot;
namespace Rehab;
public partial class AgentCreature : Agent
{
    enum AngleSlot{
        UnkAngle = 0,
    }
    enum FloatSlot{
        UnkFloat1 = 0,
        UnkFloat2 = 1,
        UnkFloat3 = 2,
        UnkFloat4 = 3,
        UnkFloat5 = 4,
        UnkFloat6 = 5,
    }
    enum IntSlot{
        AgentType = 0,
        UnkInt1 = 1,
        UnkInt2 = 2,
    }

    public override void _Ready()
    {
        base._Ready();

        DoAnimation(1, true);
	    CreateShadow(0, Vector2.One, 0);
    }
}