using Godot;
namespace Rehab.Agents.Furniture.Util;

public partial class Util_CavernHousekeeping : AgentFurniture
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
        RehabScene.Root.PlayMusic(28);
        RehabGame.SetLevelID(3);
    }
}