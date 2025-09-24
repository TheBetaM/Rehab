using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class Sound_Ambient_IceHub : AgentFurniture
{
    public override void _Ready()
    { 
        base._Ready();
        
        switch (RegInt[0])
        {
            case 1: DoSound(2, 1f, 0f, true); break;
            case 2: DoSound(3, 1f, 0f, true); break;
            case 3: DoSound(4, 1f, 0f, true); break;
            case 4: DoSound(1, 1f, 0f, true); break;
            case 5: DoSound(5, 1f, 0f, true); break;
            case 6: DoSound(6, 1f, 0f, true); break;
            default: DoSound(0, 1f, 0f, true); break;
        }
    }
}