using Godot;
namespace Rehab.Agents.Crate;

public partial class CheckPoint : AgentCrate
{
    public override int BreakAnimSlot => 5;
    public override bool BreakOnTouch => true;
    public override void OnMessage(int id)
    {
        if (id == 138)
        {
            CallDeferred("Crate_ForceBreak");
        }
    }

    public override void Crate_AfterForceBreak()
    {
        RehabGame.SetCheckPoint(GlobalPosition, GlobalRotationDegrees, ParentScene.Name, false);
    }
}