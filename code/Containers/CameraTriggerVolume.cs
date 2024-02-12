using Godot;
namespace Rehab;
public partial class CameraTriggerVolume : TriggerVolume
{
    [Export] public int Camera1Type;
    [Export] public int Camera2Type;
    [Export] public int CameraHeader;
    [Export] public int CameraHeader2;
    [Export] public int UnkShort;
    [Export] public int UnkFloat1;
    [Export] public int UnkCoords1;
    [Export] public int UnkCoords2;
    [Export] public int UnkCoords3;
    [Export] public int UnkCoords4;
    [Export] public int UnkFloat2;
    [Export] public int UnkFloat3;
    [Export] public int UnkUInt1;
    [Export] public int UnkUInt2;
    [Export] public int UnkUInt3;
    [Export] public int UnkUInt4;
    [Export] public int UnkInt5;
    [Export] public int UnkInt6;
    [Export] public int UnkFloat4;
    [Export] public int UnkFloat5;
    [Export] public int UnkFloat6;
    [Export] public int UnkFloat7;
    [Export] public int UnkUInt7;
    [Export] public int UnkInt8;
    [Export] public int UnkUInt9;
    [Export] public int UnkFloat8;
    [Export] public int UnkByte;

    public override void _Ready()
    {
        BodyEntered += OnEnter;
        BodyExited += OnExit;
    }

    void OnEnter(Node3D body)
    {
        if (body != AgentCharacter.activeCharacter) return;
        RehabScene.PlayerCam.CameraTriggerEntered(this);
    }

    void OnExit(Node3D body)
    {
        if (body != AgentCharacter.activeCharacter) return;
        RehabScene.PlayerCam.CameraTriggerExited(this);
    }

}