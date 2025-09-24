using Godot;
namespace Rehab.Agents.Furniture.Sound;
public partial class Global_Ambient_Sound : AgentFurniture
{
    public override void _Ready()
    {
        base._Ready();

        if (ParentScene != null)
        {
            ParentScene.OnChunkEnter += OnChunkEnter;
        }
    }

    public override void OnChunkEnter(object a, System.EventArgs e)
    {
        RehabScene.Root.PlayAmbience(RegInt[0]);
    }
}