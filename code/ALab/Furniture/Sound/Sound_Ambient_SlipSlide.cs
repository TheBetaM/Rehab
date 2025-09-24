using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class Sound_Ambient_SlipSlide : AgentFurniture
{
    public override void _Ready()
    { 
        base._Ready();
        
        DoSound(0, 1f, 0f, true);
    }
}