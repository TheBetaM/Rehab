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
    public Vector3 pivot = Vector3.Zero;

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

        float camdirX = 0f;
        float camdirY = 0f;
        float camx = 0f;
        float camz = 0f;

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
            camangleX = camTarget.GlobalRotationDegrees.Y / 180.0f;
            camangleY = 0f;
        }
        
        camvector = GlobalTransform.Basis.Z;
        camvector.Y = 0f;
        camvector = camvector.Normalized();
        camright = new Vector3(camvector.Z, 0, -camvector.X);
        
        bool instant = false;
        if (mouse_last_motion == 2)
        {
            camdirX -= mouse_position.X;
            camdirY += mouse_position.Y;
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
            camangleY = Math.Clamp(camangleY, -3.0f, 3.0f);
        }
        else
        {
            camdirX = Math.Clamp(camdirX, -1.0f, 1.0f);
            camdirY = Math.Clamp(camdirY, -1.0f, 1.0f);
            camangleX += camdirX * (float)delta * 1.0f;
            camangleY += camdirY * (float)delta * 3.0f;
            camangleY = Math.Clamp(camangleY, -3.0f, 3.0f);
        }
        
        var angle = Vector3.Forward.Rotated(Vector3.Up, camangleX * (float)Math.PI);
        var camdist = 6.0f + (-Math.Abs(camangleY));
        var camheight = 4.5f + (camangleY * 1.5f);
        var pivotheight = 3.0f;
        
        camx += Math.Clamp((angle.X * camdist), -camdist, camdist);
        camz += Math.Clamp((angle.Z * camdist), -camdist, camdist);
        if (instant)
            pivot = camTarget.GlobalTransform.Origin + new Vector3(camx, camheight, camz);
        else
            pivot = pivot.Lerp(camTarget.GlobalTransform.Origin + new Vector3(camx, camheight, camz), (float)delta * 8.0f);
        GlobalPosition = GlobalPosition.Lerp(pivot, (float)delta * 8.0f);
        LookAt(camTarget.GlobalTransform.Origin + (Vector3.Up * pivotheight), Vector3.Up);
    }

    public void FullReset()
    {
        camTarget = null;
        GlobalPosition = Vector3.Zero;
        GlobalRotationDegrees = Vector3.Zero;
        camangleX = 0f;
        camangleY = 0f;
    }

    public void SetupCam(Node3D target)
    {
        FullReset();
        camTarget = target;
        camangleX = target.GlobalRotationDegrees.Y / 180f;

        float camx = 0f;
        float camz = 0f;
        var angle = Vector3.Forward.Rotated(Vector3.Up, camangleX * (float)Math.PI);
        var camdist = 6.0f + (-Math.Abs(camangleY));
        var camheight = 4.5f + (camangleY * 1.5f);
        var pivotheight = 3.0f;
        camx += Math.Clamp((angle.X * camdist), -camdist, camdist);
        camz += Math.Clamp((angle.Z * camdist), -camdist, camdist);
        GlobalPosition = camTarget.GlobalTransform.Origin + new Vector3(camx, camheight, camz);
        LookAt(camTarget.GlobalTransform.Origin + (Vector3.Up * pivotheight), Vector3.Up);
        camvector = GlobalTransform.Basis.Z;
        camvector = camvector.Normalized();
        camright = new Vector3(camvector.Z, 0, -camvector.X);
        camright = camright.Normalized();
    }

    public void CameraTriggerEntered(CameraTriggerVolume trig)
    {

    }

    public void CameraTriggerExited(CameraTriggerVolume trig)
    {

    }
}