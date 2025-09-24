using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class School_Hub_Ambient_Sounds : AgentFurniture
{
    public override void _Ready()
    { 
        base._Ready();
        
        DoSound(0, 1f, 0f, true);
    }
}