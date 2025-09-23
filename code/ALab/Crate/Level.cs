using Godot;
namespace Rehab.Agents.Crate;

public partial class Level : CheckPoint
{
    public override void Crate_AfterForceBreak()
    {
        RehabGame.SetCheckPoint(GlobalPosition, GlobalRotationDegrees, ParentScene.Name, true);
    }
}