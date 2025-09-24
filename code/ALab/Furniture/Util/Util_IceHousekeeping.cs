using Godot;
namespace Rehab.Agents.Furniture.Util;

public partial class Util_IceHousekeeping : AgentFurniture
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
        RehabScene.Root.PlayMusic(1);
        RehabGame.SetLevelID(6);
    }
}