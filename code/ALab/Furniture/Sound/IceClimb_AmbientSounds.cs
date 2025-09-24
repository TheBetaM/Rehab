using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class IceClimb_AmbientSounds : AgentFurniture
{
    public override void _Ready()
    { 
        base._Ready();
        
        switch (SubType)
        {
            case 1: DoSound(4, 1f, 0f, true); break;
            case 2: DoSound(5, 1f, 0f, true); break;
            case 4: DoSound(10, 1f, 0f, true); break;
            case 3: break; // COM_ICECLIMB_AMBIENT_SOUNDS_WATERSPLASHES
        }
    }
}