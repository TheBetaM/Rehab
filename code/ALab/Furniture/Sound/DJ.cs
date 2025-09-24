using Godot;
namespace Rehab.Agents.Furniture.Sound;

public partial class DJ : AgentFurniture
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
        RehabScene.Root.PlayMusic(RegInt[0]);
        RehabGame.SetLevelID(RegInt[2]);
    }
}