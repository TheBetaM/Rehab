using System;
using Godot;
namespace Rehab;
public partial class FreeLookCamera : Camera3D
{
    const float SHIFT_MULTIPLIER = 2.5f;
    const float ALT_MULTIPLIER = 1.0f / SHIFT_MULTIPLIER;

    [Export(PropertyHint.Range,"0.0,1.0,")]
    public float sensitivity = 0.25f;

    Vector2 _mouse_position = new Vector2();
    float _total_pitch = 0.0f;

    Vector3 _direction = new Vector3();
    Vector3 _velocity = new Vector3();
    float _acceleration = 30f;
    float _deceleration = -10f;
    float _vel_multiplier = 4;

    float _w = 0f;
    float _s = 0f;
    float _a = 0f;
    float _d = 0f;
    float _q = 0f;
    float _e = 0f;
    bool _shift = false;
    bool _alt = false;

    public float cooldown = 0.1f;

    public override void _Input(InputEvent input)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;

        if (input is InputEventMouseMotion mouseMotion)
        {
            _mouse_position = mouseMotion.Relative;
        }

        if (input is InputEventMouseButton mouseButton)
        {
            switch (mouseButton.ButtonIndex)
            {
                case MouseButton.Right:
                if (mouseButton.Pressed)
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                else
                    Input.MouseMode = Input.MouseModeEnum.Visible;
                break;

                case MouseButton.WheelUp:
                _vel_multiplier = Math.Clamp(_vel_multiplier * 1.1f, 0.2f, 20f);
                break;

                case MouseButton.WheelDown:
                _vel_multiplier = Math.Clamp(_vel_multiplier / 1.1f, 0.2f, 20f);
                break;
            }
        }

        if (input is InputEventKey key)
        {
            switch (key.Keycode)
            {
                case Key.W: _w = key.Pressed? 1f : 0f; break;
                case Key.S: _s = key.Pressed? 1f : 0f; break;
                case Key.A: _a = key.Pressed? 1f : 0f; break;
                case Key.D: _d = key.Pressed? 1f : 0f; break;
                case Key.Q: _q = key.Pressed? 1f : 0f; break;
                case Key.E: _e = key.Pressed? 1f : 0f; break;
                case Key.Shift: _shift = key.Pressed; break;
                case Key.Alt: _alt = key.Pressed; break;
                case Key.F: ExitCam(); break;
            }
        }
    }

    public override void _Process(double delta)
    {
        _update_mouselook();
        _update_movement(delta);
        UpdateCamControls(delta);
    }

    void UpdateCamControls(double delta)
    {
        if (cooldown > 0)
        {
            cooldown -= (float)delta;
            return;
        }
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
            return;
        
        
        _w = Input.GetActionStrength("pad1_lstick_up");
        _s = Input.GetActionStrength("pad1_lstick_down");
        _d = Input.GetActionStrength("pad1_lstick_right");
        _a = Input.GetActionStrength("pad1_lstick_left");
        _e = Input.GetActionStrength("pad1_cross");
        _q = Input.GetActionStrength("pad1_triangle");
        if (Input.IsActionJustPressed("pad1_L1")){
            ExitCam();
            return;
        }

        Vector3 targetDeg = GlobalRotationDegrees;
        if (Input.IsActionPressed("pad1_rstick_left"))
            targetDeg.Y += 90.0f * (float)delta * Input.GetActionStrength("pad1_rstick_left");
        else if (Input.IsActionPressed("pad1_rstick_right"))
            targetDeg.Y += -90.0f * (float)delta * Input.GetActionStrength("pad1_rstick_right");
        if (Input.IsActionPressed("pad1_rstick_up"))
            targetDeg.X += 90.0f * (float)delta * Input.GetActionStrength("pad1_rstick_up");
        else if (Input.IsActionPressed("pad1_rstick_down"))
            targetDeg.X += -90.0f * (float)delta * Input.GetActionStrength("pad1_rstick_down");
        GlobalRotationDegrees = targetDeg;
        
    }

    void ExitCam()
    {
        if (RehabGame.UseMouseCamera)
            Input.MouseMode = Input.MouseModeEnum.Captured;
        else
            Input.MouseMode = Input.MouseModeEnum.Visible;
        
        PlayerCamera mainCam = RehabScene.PlayerCam;
        Current = false;
        mainCam.Current = true;
        ProcessMode = ProcessModeEnum.Disabled;
        mainCam.ProcessMode = ProcessModeEnum.Inherit;
        mainCam.camTarget.ProcessMode = ProcessModeEnum.Inherit;
        RehabGame.DisplayMessage("FREECAM " + Tr("#FE-Off"));
    }

    void _update_movement(double delta)
    {
        _direction = new Vector3(
            _d - _a,
            _e - _q,
            _s - _w
        );

        Vector3 offset = _direction.Normalized() * _acceleration * _vel_multiplier * (float)delta + _velocity.Normalized() * _deceleration * _vel_multiplier * (float)delta;
        
        float speed_multi = 1.0f;
        if (_shift) speed_multi *= SHIFT_MULTIPLIER;
        if (_alt) speed_multi *= ALT_MULTIPLIER;

        if (_direction == Vector3.Zero && offset.LengthSquared() > _velocity.LengthSquared())
        {
            _velocity = Vector3.Zero;
        }
        else
        {
            _velocity.X = Math.Clamp(_velocity.X + offset.X, -_vel_multiplier, _vel_multiplier);
            _velocity.Y = Math.Clamp(_velocity.Y + offset.Y, -_vel_multiplier, _vel_multiplier);
            _velocity.Z = Math.Clamp(_velocity.Z + offset.Z, -_vel_multiplier, _vel_multiplier);

            Translate(_velocity * (float)delta * speed_multi);
        }
    }

    void _update_mouselook()
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        _mouse_position *= sensitivity * RehabGame.MouseSensitivity;
        float yaw = _mouse_position.X;
        float pitch = _mouse_position.Y;
        _mouse_position = Vector2.Zero;

        pitch = Math.Clamp(pitch, -90f - _total_pitch, 90f - _total_pitch);
        _total_pitch += pitch;

        RotateY(Mathf.DegToRad(-yaw));
        RotateObjectLocal(new Vector3(1f,0f,0f), Mathf.DegToRad(-pitch));
    }

    public void FullReset(){
        _mouse_position = Vector2.Zero;
        _total_pitch = 0f;
        _direction = Vector3.Zero;
        _velocity = Vector3.Zero;
        _acceleration = 30;
        _deceleration = -10;
        _vel_multiplier = 4;
    }


}