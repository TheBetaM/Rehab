using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class DJ_Triggerable : AgentFurniture
{
    public override void OnMessage(int id)
    {
        if (id == 87 || id == 138)
            RehabScene.Root.PlayMusic(RegInt[0]);
    }
}