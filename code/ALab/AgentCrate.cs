using Godot;
namespace Rehab;
public partial class AgentCrate : Agent
{
    public override void _Ready()
    {
        base._Ready();

        CreateShadow(1, Vector2.One, 0);
    }
}