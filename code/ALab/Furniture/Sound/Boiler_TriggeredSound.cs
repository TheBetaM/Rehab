using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class Boiler_TriggeredSound : AgentFurniture
{
    public override void OnMessage(int id)
    {
        if (id == 4)
        {
            switch (SubType)
            {
                case 1: DoSound(0, 1f, 0f); break;
                case 2: DoSound(1, 1f, 0f); break;
                case 3: DoSound(2, 1f, 0f); break;
                case 4: DoSound(3, 1f, 0f); break;
            }
        }
    }
}