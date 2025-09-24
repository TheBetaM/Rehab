using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class SoundSpot_CamShaft : AgentFurniture
{
    public override void _Ready()
    {
        base._Ready();

        DelayedSoundStart();
    }
    
    async void DelayedSoundStart()
    {
        await ToSignal(GetTree().CreateTimer(RegFloat[4]), SceneTreeTimer.SignalName.Timeout);
        switch (RegInt[0])
        {
            case 1: DoSound(0, 1f, 0f, true); break;
            case 2: DoSound(1, 1f, 0f, true); break;
            case 3: DoSound(2, 1f, 0f, true); break;
            case 4: DoSound(3, 1f, 0f, true); break;
            case 5: DoSound(4, 1f, 0f, true); break;
            case 6: DoSound(5, 1f, 0f, true); break;
            case 7: DoSound(6, 1f, 0f, true); break;
            case 8: DoSound(7, 1f, 0f, true); break;
            default: DoSound(0, 1f, 0f, true); break;
        }
    }
}