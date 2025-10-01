using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class Sound_Ambient_IceHub : AgentFurniture
{
    public override void _Ready()
    { 
        base._Ready();
        
        switch (SubType)
        {
            case 1: DoSound(2, 1f, 0f, true); break; //1.0
            case 2: DoSound(3, 1f, 0f, true); break; //0.0
            case 3: DoSound(4, 1f, 0f, true); break; //1.0
            case 4: DoSound(1, 1f, 0f, true); break; //0.6
            case 5: DoSound(5, 1f, 0f, true); break; //0.4
            case 6: DoSound(6, 1f, 0f, true); break; //0.0
            default: DoSound(0, 1f, 0f, true); break; //0.7
        }
    }
}