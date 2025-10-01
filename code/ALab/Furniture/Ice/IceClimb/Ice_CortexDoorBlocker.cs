using Godot;
namespace Rehab.Agents.Furniture.Ice.IceClimb;

public partial class Ice_CortexDoorBlocker : AgentFurniture
{
    public override void _Ready()
    {
        base._Ready();

        Flags &= ~AgentInstance.IFlags.Visible;
        Flags &= ~AgentInstance.IFlags.Collidable;
        Flags &= ~AgentInstance.IFlags.Flag5;
        UpdateFlags();
    }
    
    public override void OnMessage(int id)
    {
        if (id == 87)
        {
            Flags |= AgentInstance.IFlags.Visible;
            Flags |= AgentInstance.IFlags.Collidable;
            UpdateFlags();
        }
    }
}