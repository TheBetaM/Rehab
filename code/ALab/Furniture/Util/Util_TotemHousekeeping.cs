using Godot;
namespace Rehab.Agents.Furniture.Util;

public partial class Util_TotemHousekeeping : AgentFurniture
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
        // todo: only activate in demo chunks
        RehabScene.Root.PlayMusic(29);
    }
}