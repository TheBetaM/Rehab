using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class SoundSpot_Lava : AgentFurniture
{
    public override void _Ready()
    {
        base._Ready();

        DelayedSoundStart();
    }
    
    async void DelayedSoundStart()
    {
        await ToSignal(GetTree().CreateTimer(RegFloat[4]), SceneTreeTimer.SignalName.Timeout);
        DoSound(0, 1f, 0f, true);
    }
}