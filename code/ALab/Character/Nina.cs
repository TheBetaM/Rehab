using Godot;
namespace Rehab.Agents.Character;

public partial class Nina : AgentCharacter
{
    public override CharacterType CharType => CharacterType.Nina;
    public override float WalkSpeed => 2.5f;
    public override float RunSpeed => 7f;
    public override float SpinLength => 0.7f;
    public override float CrawlSpeed => 0f;
    public override float SlideSpeed => 0f;
    public override float SlideTime => 0f;
}