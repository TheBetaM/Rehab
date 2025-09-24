using Godot;
namespace Rehab;

public partial class AgentChiChiGrass : Agent
{
    enum FloatSlot
    {
        Mass = 0,
        MunchDurationInSeconds = 1,
        HorizontalReelDistance = 2,
        VerticalReelDistance = 3,
    }
    enum AngleSlot{
        AttachmentRotationLimit = 0,
    }
}