using System;
using Godot;
namespace Rehab;
public partial class PlayerCamera : Camera3D
{
    float camangleX = 0f;
    float camangleY = 0f;
    public Vector3 camvector = Vector3.Zero;
    public Node3D camTarget;
    public Vector3 camright = Vector3.Zero;
    Vector2 mouse_position = Vector2.Zero;
    int mouse_last_motion = 0;
    Vector2 mouse_add = Vector2.Zero;
    public SpringArm3D Arm;
    public Node3D ArmPivot;

    public override void _Ready()
    {
        Arm = (SpringArm3D)GetParent();
        ArmPivot = (Node3D)Arm.GetParent();
    }

    public override void _Input(InputEvent input)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (!RehabGame.UseMouseCamera) return;
            
        if (input is InputEventMouseMotion mouse)
        {
            mouse_position = mouse_add + mouse.Relative;
            mouse_add = Vector2.Zero;
            mouse_last_motion = 2;
        }
    }

    public override void _Process(double delta)
    {
        if (camTarget == null) return;

        float pivotheight = 3.0f;
        //ArmPivot.GlobalPosition = camTarget.GlobalPosition + Vector3.Up * 3.0f;
        ArmPivot.GlobalPosition = ArmPivot.GlobalPosition.Lerp(camTarget.GlobalPosition + Vector3.Up * pivotheight, (float)delta * 8.0f);
        float camdirX = 0f;
        float camdirY = 0f;

        if (Input.IsActionPressed("pad1_rstick_left"))
        {
            if (!RehabGame.InvertCameraX)
                camdirX -= Input.GetActionStrength("pad1_rstick_left");
            else
                camdirX += Input.GetActionStrength("pad1_rstick_left");
        }
        if (Input.IsActionPressed("pad1_rstick_right"))
        {
            if (!RehabGame.InvertCameraX)
                camdirX += Input.GetActionStrength("pad1_rstick_right");
            else
                camdirX -= Input.GetActionStrength("pad1_rstick_right");
        }
        if (Input.IsActionPressed("pad1_rstick_up"))
        {
            if (!RehabGame.InvertCameraY)
                camdirY += Input.GetActionStrength("pad1_rstick_up");
            else
                camdirY -= Input.GetActionStrength("pad1_rstick_up");
        }
        if (Input.IsActionPressed("pad1_rstick_down"))
        {
            if (!RehabGame.InvertCameraY)
                camdirY -= Input.GetActionStrength("pad1_rstick_down");
            else
                camdirY += Input.GetActionStrength("pad1_rstick_down");
        }
        if (Input.IsActionJustPressed("pad1_L1") && !RehabScene.Root.XR_Enabled)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            FreeLookCamera freeCam = RehabScene.FreeLookCam;
            freeCam.GlobalPosition = GlobalPosition;
            freeCam.GlobalRotationDegrees = GlobalRotationDegrees;
            Current = false;
            freeCam.Current = true;
            freeCam.cooldown = 0.1f;
            ProcessMode = ProcessModeEnum.Disabled;
            freeCam.ProcessMode = ProcessModeEnum.Always;
            camTarget.ProcessMode = ProcessModeEnum.Disabled;
            RehabGame.DisplayMessage("FREECAM " + Tr("#FE-On"));
            return;
        }
        if (Input.IsActionPressed("pad1_R3"))
        {
            camangleX = (camTarget.GlobalRotationDegrees.Y / 180.0f) + 1f;
            camangleY = 0f;
        }
        
        camvector = GlobalTransform.Basis.Z;
        camvector.Y = 0f;
        camvector = camvector.Normalized();
        camright = new Vector3(camvector.Z, 0, -camvector.X);
        
        bool instant = false;
        if (mouse_last_motion == 2)
        {
            camdirX -= mouse_position.X * RehabGame.MouseSensitivity;
            camdirY += mouse_position.Y * RehabGame.MouseSensitivity;
            instant = true;
            mouse_last_motion = 1;
        }
        else if (mouse_last_motion == 1)
        {
            mouse_add = -mouse_position;
            mouse_last_motion = 0;
        }
        
        if (instant)
        {
            camangleX += camdirX / 800.0f;
            camangleY += camdirY / 200.0f;
        }
        else
        {
            camdirX = Math.Clamp(camdirX, -1.0f, 1.0f);
            camdirY = Math.Clamp(camdirY, -1.0f, 1.0f);
            camangleX += camdirX * (float)delta * 1.0f;
            camangleY += camdirY * (float)delta * 1.0f;
        }
        camangleY = Math.Clamp(camangleY, -1.0f, 1.0f);
        var camangleYdeg = (camangleY * -60f) - 15f;
        if (instant)
        {
            ArmPivot.RotationDegrees = new Vector3(camangleYdeg, camangleX * 180f, 0f);
        }
        else
        {
            float rad = Mathf.DegToRad(camangleX * 180f);
            rad = Mathf.LerpAngle(ArmPivot.Rotation.Y, rad, (float)delta * 16.0f);
            rad = Mathf.RadToDeg(rad);
            ArmPivot.RotationDegrees = ArmPivot.RotationDegrees.Lerp(new Vector3(camangleYdeg, rad, 0f), (float)delta * 8.0f);
        }
        LookAt(camTarget.GlobalTransform.Origin + (Vector3.Up * pivotheight), Vector3.Up);
    }

    public void FullReset()
    {
        camTarget = null;
        Arm.ClearExcludedObjects();
        Arm.GlobalPosition = Vector3.Zero;
        Arm.GlobalRotationDegrees = Vector3.Zero;
        ArmPivot.GlobalPosition = Vector3.Zero;
        ArmPivot.GlobalRotationDegrees = Vector3.Zero;
        camangleX = 0f;
        camangleY = 0f;
    }

    public void SetupCam(Node3D target)
    {
        FullReset();
        camTarget = target;
        camangleX = (target.GlobalRotationDegrees.Y / 180f) + 1f;

        float pivotheight = 3.0f;
        ArmPivot.GlobalPosition = camTarget.GlobalPosition + Vector3.Up * pivotheight;
        camvector = GlobalTransform.Basis.Z;
        camvector = camvector.Normalized();
        camright = new Vector3(camvector.Z, 0, -camvector.X);
        camright = camright.Normalized();
        LookAt(camTarget.GlobalTransform.Origin + (Vector3.Up * pivotheight), Vector3.Up);
        Arm.AddExcludedObject((Rid)target.Call("get_rid"));
    }

    public void CameraTriggerEntered(CameraTriggerVolume trig)
    {

    }

    public void CameraTriggerExited(CameraTriggerVolume trig)
    {

    }
}