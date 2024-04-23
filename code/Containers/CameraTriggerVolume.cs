using Godot;
using System;
namespace Rehab;
public partial class CameraTriggerVolume : TriggerVolume
{
    [Export] public int Camera1Type;
    [Export] public int Camera2Type;
    [Export] public CamFlags CamHeader;
    [Export] public ushort UnkShort;
    [Export] public float UnkFloat1;
    [Export] public Vector4 UnkCoords1;
    [Export] public Vector4 UnkCoords2;
    [Export] public float UnkFloat2;
    [Export] public float UnkFloat3;
    [Export] public uint FoV1;
    [Export] public uint FoV2;
    [Export] public uint UnkUInt3;
    [Export] public uint UnkUInt4;
    [Export] public int AngleAroundFocus1;
    [Export] public int AngleAroundFocus2;
    [Export] public float Distance1;
    [Export] public float Distance2;
    [Export] public float UnkFloat6;
    [Export] public float MoveSpeed;
    [Export] public uint UnkUInt7;
    [Export] public int UnkInt8;
    [Export] public uint UnkUInt9;
    [Export] public float UnkFloat8;
    [Export] public byte UnkByte;

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

    [Flags]
    public enum CamFlags : uint
    {
        Flag0 = 1 << 0,
        Flag1 = 1 << 1,
        Flag2 = 1 << 2,
        DistanceToFocus = 1 << 3,
        Flag4 = 1 << 4,
        Flag5 = 1 << 5,
        Flag6 = 1 << 6,
        FieldOfView = 1 << 7,
        LocalOffsetFromFocus = 1 << 8,
        Flag9 = 1 << 9,
        Flag10 = 1 << 10,
        LockControl = 1 << 11,
        Flag12 = 1 << 12,
        MoveSpeed = 1 << 13,
        Flag14 = 1 << 14,
        Flag15 = 1 << 15,
        Flag16 = 1 << 16,
        Flag17 = 1 << 17,
        Flag18 = 1 << 18,
        LockControlLookUp = 1 << 19,
        LockVerticalAngle = 1 << 20,
        UnusedFixedPoint = 1 << 21,
        Flag22 = 1 << 22,
        Flag23 = 1 << 23,
        Flag24 = 1 << 24,
        Flag25 = 1 << 25,
        Flag26 = 1 << 26,
        Flag27 = 1 << 27,
        Flag28 = 1 << 28,
        Flag29 = 1 << 29,
        Flag30 = 1 << 30,
        Flag31 = (uint)1 << 31,
        
    }

}