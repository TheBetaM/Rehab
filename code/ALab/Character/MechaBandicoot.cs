using Godot;
namespace Rehab.Agents.Character;

public partial class MechaBandicoot : AgentCharacter
{
    public override CharacterType CharType => CharacterType.MechaBandicoot;
    public override float WalkSpeed => 12f;
    public override float RunSpeed => 12f;
    public override float SpinLength => 0f;
    public override float CrawlSpeed => 0f;
    public override float SlideSpeed => 0f;
    public override float SlideTime => 0f;
}