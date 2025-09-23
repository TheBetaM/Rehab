using Godot;
namespace Rehab.Agents.Character;

public partial class Cortex : AgentCharacter
{
    public override CharacterType CharType => CharacterType.Cortex;
    public override float WalkSpeed => 2.5f;
    public override float RunSpeed => 7f;
    public override float SpinLength => 0f;
    public override float CrawlSpeed => 1.75f;
    public override float SlideSpeed => 18f;
    public override float SlideTime => 0.4f;
}