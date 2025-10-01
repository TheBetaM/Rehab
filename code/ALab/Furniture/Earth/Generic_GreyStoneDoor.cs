using Godot;
namespace Rehab.Agents.Furniture.Earth;

public partial class Generic_GreyStoneDoor : AgentFurniture
{
    public override void _Ready()
    {
        base._Ready();

        DoAnimation(1, false);
    }

    public override void OnMessage(int id)
    {
        if (id == 87)
        {
            DoAnimation(1, false);
        }
        else if (id == 88)
        {
            DoAnimation(1, false, true);
        }
    }
}