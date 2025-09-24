using Godot;
namespace Rehab;

public partial class AgentChiChiGrass : Agent
{
    enum FloatSlot
    {
        MunchDurationInSeconds = 0,
        HorizontalReelDistance = 1,
        VerticalReelDistance = 2,
    }
    enum AngleSlot{
        AttachmentRotationLimit = 0,
    }
}