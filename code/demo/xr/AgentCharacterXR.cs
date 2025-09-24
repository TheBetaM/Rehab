using System.Collections.Generic;
using System;
using Godot;
namespace Rehab;

public partial class AgentCharacterXR : AgentCharacter
{
    public override void _PhysicsProcess(double delta)
    {
        ActiveActorTypes[(int)CharType] = GetPath();
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        UpdateDynamicCollision((float)delta);
        if (activeCharacter != this) return;

        UpdateMovement((float)delta);
        UpdateFootStep((float)delta);
        RehabScene.Root.XR_Origin.GlobalPosition = GlobalPosition + (Vector3.Up * RehabGame.XR_Height);
    }

    void UpdateMovement(float delta)
    {
        if (BlockMovement) return;
        Vector3 direction = Vector3.Zero;
        bool isJumping = false;
        bool onFloor = (bool)Call("is_on_floor");

        direction.X -= Input.GetActionStrength(RehabGame.Pad_Dpad_Up);
        direction.X += Input.GetActionStrength(RehabGame.Pad_Dpad_Down);
        if (direction.X == 0)
        {
            direction.X -= Input.GetActionStrength(RehabGame.Pad_LStick_Up);
            direction.X += Input.GetActionStrength(RehabGame.Pad_LStick_Down);
        }
        direction.Z += Input.GetActionStrength(RehabGame.Pad_Dpad_Right);
        direction.Z -= Input.GetActionStrength(RehabGame.Pad_Dpad_Left);
        if (direction.Z == 0)
        {
            direction.Z += Input.GetActionStrength(RehabGame.Pad_LStick_Right);
            direction.Z -= Input.GetActionStrength(RehabGame.Pad_LStick_Left);
        }

        direction = direction.Clamp(-Vector3.One, Vector3.One);
        var camvector = RehabScene.Root.XR_Origin.XR_Camera.GlobalTransform.Basis.Z;
        camvector.Y = 0f;
        camvector = camvector.Normalized();
        var camright = new Vector3(camvector.Z, 0, -camvector.X);
        direction = (direction.X * camvector) + (direction.Z * camright);
        direction.Y = 0f;
        float dirLength = Math.Abs(direction.Length());
        var pressed = dirLength > 0.05;

        if (Input.IsActionPressed(RehabGame.Pad_Cross))
        {

        }
        if (Input.IsActionJustPressed(RehabGame.Pad_Triangle))
        {
            RehabGame.DisplayHUD();
        }
        if (Input.IsActionJustPressed(RehabGame.Pad_R1))
        {

        }
        if (Input.IsActionJustPressed(RehabGame.Pad_Square))
        {

        }
        if (Input.IsActionJustPressed(RehabGame.Pad_Circle))
        {

        }

        var speed = RunSpeed;
        if (dirLength < 0.3f)
            speed = 0;
        else if (dirLength < 0.8f)
            speed = WalkSpeed;
        direction = direction.Normalized();

        if (pressed)
        {
            char_velocity.X = direction.X * speed;
            char_velocity.Z = direction.Z * speed;
            float atan = (float)Math.Atan2(direction.X, direction.Z);
            var targetRot = new Vector3(0, atan, 0);
            if (speed != 0)
                GlobalRotation = targetRot;
            else
            {
                GlobalRotation = GlobalRotation.Lerp(targetRot, 5f * delta);
                GlobalRotation = new Vector3(0, GlobalRotation.Y, 0);
            }
            if (!isJumping && onFloor)
            {
                if (speed == 0)
                    DoAnimation(9, true);
                else if (speed == WalkSpeed)
                    DoAnimation(10, true);
                else
                    DoAnimation(11, true);
            }
        }
        else
        {
            char_velocity.X = 0f;
            char_velocity.Z = 0f;
            if (!isJumping && onFloor && spinTimer <= 0f)
            {
                DoAnimation(8, true);
            }
        }

        if (!isJumping && !onFloor)
        {
            coyoteTimer -= delta;
            if (coyoteTimer <= 0f && spinTimer <= 0.0)
            {
                DoAnimation(27, false);
            }
        }

        if (!onFloor)
        {
            if (coyoteTimer <= 0f)
                char_velocity.Y -= AirGravity * delta;
            else
                char_velocity.Y = 0f;
        }
        else
        {
            coyoteTimer = 0.1f;
        }

        Set("velocity", char_velocity);
        Call("move_and_slide");
        var colData = (KinematicCollision3D)Call("get_last_slide_collision");
        if (colData == null) return;
        var colCount = colData.GetCollisionCount();
        if (colCount == 0) return;
        for (int i = 0; i < colCount; i++)
        {
            var hit = colData.GetCollider(i);
            if (hit is AgentCrate crate)
            {
                crate.OnBodyEntered(this);
            }
            else if (hit is Agents.Furniture.School.Boiler.DoubleDoor_Anim furn)
            {
                furn.OnDoorTouch(this);
            }
        }
    }

}