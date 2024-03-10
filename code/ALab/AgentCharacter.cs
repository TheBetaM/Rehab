using System.Collections.Generic;
using System;
using Godot;
namespace Rehab;
public partial class AgentCharacter : Agent
{
    public enum CharASlot {
        UnkAngle1 = 0,
        UnkAngle2 = 1,
        UnkAngle3 = 2,
        UnkAngle4 = 3,
        UnkAngle5 = 4,
        UnkAngle6 = 5,
        UnkAngle7 = 6,
        UnkAngle8 = 7,
        UnkAngle9 = 8,
    }
    public enum CharFSlot {
        UnkFloat01 = 0,
        AirGravity = 1,
        UnkFloat03 = 2,
        BaseGravity = 3,
        WalkSpeedPercentage = 4,
        UnkFloat06 = 5,
        WalkSpeed = 6,
        RunSpeed = 7,
        StrafingSpeed = 8,
        SpinThrowForwardForce = 9,
        SpinLength = 10,
        SpinDelay = 11,
        UnkFloat13 = 12,
        UnkFloat14 = 13,
        Static15 = 14,
        JumpAirSpeed = 15,
        JumpHeight = 16,
        UnkFloat17Jump = 17,
        UnkFloat18Jump = 18,
        JumpEdgeSpeed = 19,
        DoubleJumpHeight = 20,
        UnkFloat21DoubleJump = 21,
        UnkFloat22DoubleJump = 22,
        UnkFloat23SlideJump = 23,
        UnkFloat24SlideJump = 24,
        UnkFloat25SlideJump = 25,
        UnkFloat26SlideJump = 26,
        UnkFloat27 = 27,
        UnkFloat28 = 28,
        UnkFloat29 = 29,
        UnkFloat30 = 30,
        BodyslamHangTime = 31,
        BodyslamUpwardForce = 32,
        BodyslamGravityForce = 33,
        FlyingKickHangTime = 34,
        FlyingKickForwardSpeed = 35,
        FlyingKickGravity = 36,
        RadialBlastTimeToStart = 37,
        UnkFloat38RadialBlast = 38,
        UnkFloat39RadialBlast = 39,
        CrawlSpeed = 40,
        CrawlTimeFromStand = 41,
        CrawlTimeToStand = 42,
        CrawlTimeToRun = 43,
        SlideSpeed = 44,
        UnkFloat45Slide = 45,
        UnkFloat46Slide = 46,
        UnkFloat47Slide = 47,
        UnkFloat48Slide = 48,
        UnkFloat49Slide = 49,
        GunButtonHoldTimeToStartCharging = 50,
        GunChargeTime = 51,
        GunTimeBetweenChargedShots = 52,
        GunTimeBetweenShots = 53,
        UnkFloat54 = 54,
        RadialBlastChargeTime = 55,
    }
    public enum CharISlot {
        AgentType = 0,
        UnkInt = 1,
        Health = 2,
    }

    Vector3 char_velocity = Vector3.Zero;
    PlayerCamera physCam;
    public bool isReparenting;
    public bool isSwitchingChunks;
    float headdirX;
    float headdirY;
    float footsteptimer;
    bool footsteplast;
    bool gravityOn = true;
    public float spinTimer;
    bool isCrouched;
    float slideTimer;
    float coyoteTimer;

    AudioStream FS_Dirt_1;
    AudioStream FS_Dirt_2 ;
    AudioStream FS_Grass_1;
    AudioStream FS_Grass_2;
    AudioStream FS_Metal_1;
    AudioStream FS_Metal_2;
    AudioStream FS_Sand_1;
    AudioStream FS_Sand_2;
    AudioStream FS_Stone_1;
    AudioStream FS_Stone_2;
    AudioStream FS_Water_1;
    AudioStream FS_Water_2;
    AudioStream FS_Wood_1;
    AudioStream FS_Wood_2;
    AudioStream FS_Tile_1;
    AudioStream FS_Tile_2;
    AudioStream FS_Slippy;

    public static AgentCharacter activeCharacter;
    public static Dictionary<int, string> ActiveActorTypes = new();

    public override void _Ready()
    {
        base._Ready();

        if (!ActiveActorTypes.ContainsKey(RegInt[(int)CharISlot.AgentType]))
            ActiveActorTypes[RegInt[(int)CharISlot.AgentType]] = GetPath();
        else
        {
            Visible = false;
            ProcessMode = ProcessModeEnum.Disabled;
        }
        
        CreateShadow(0, Vector2.One, 0);
        
        if (activeCharacter != null)
            return;
        
        activeCharacter = this;
        physCam = RehabScene.PlayerCam;
        physCam.SetupCam(this);
        if (RehabScene.Root.XR_Enabled)
        {
            XR_Setup();
        }

        FS_Dirt_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_dirt_3.res");
        FS_Dirt_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_dirt_5.res");
        FS_Grass_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_grass_2.res");
        FS_Grass_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_grass_3.res");
        FS_Metal_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_metal_1.res");
        FS_Metal_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_metal_5.res");
        FS_Sand_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_sand_1.res");
        FS_Sand_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_sand_3.res");
        FS_Stone_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_stone_3.res");
        FS_Stone_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_stone_5.res");
        FS_Water_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/FS_WAT1.res");
        FS_Water_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/FS_WAT2.res");
        FS_Wood_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_wood_1.res");
        FS_Wood_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/fs_wood_2.res");
        FS_Tile_1 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/L09_Cortex_boots_tile_2.res");
        FS_Tile_2 = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Surface/L09_Cortex_boots_tile_7.res");
        FS_Slippy = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/L03_TotemHokum/L03_Tribesmn_fs.res");
    }

    public override void _ExitTree()
    {
        if (isReparenting)
        {
            isReparenting = false;
            return;
        }
        if (ActiveActorTypes.ContainsKey(RegInt[(int)CharISlot.AgentType]))
        {
            if (ActiveActorTypes[RegInt[(int)CharISlot.AgentType]] == GetPath())
                ActiveActorTypes.Remove(RegInt[(int)CharISlot.AgentType]);
        }
        if (activeCharacter == this)
            activeCharacter = null;
    }

    public override void _PhysicsProcess(double delta)
    {
        ActiveActorTypes[RegInt[(int)CharISlot.AgentType]] = GetPath();
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (activeCharacter != this) return;
        if (Input.IsActionJustPressed("pad1_start"))
        {
            RehabScene.Root.StartPauseMenu(false);
            return;
        }
        
        UpdateMovement((float)delta);
        UpdateHeadAnim((float)delta);
        UpdateFootStep((float)delta);
        if (RehabScene.Root.XR_Enabled)
        {
            RehabScene.Root.XR_Origin.GlobalPosition = GlobalPosition + (Vector3.Up * 0.75f);
        }
    }

    void UpdateMovement(float delta)
    {
        Vector3 direction = Vector3.Zero;
        bool isJumping = false;
        bool onFloor = (bool)Call("is_on_floor");
        
        direction.X -= Input.GetActionStrength("pad1_dpad_up");
        direction.X += Input.GetActionStrength("pad1_dpad_down");
        if (direction.X == 0)
        {
            direction.X -= Input.GetActionStrength("pad1_lstick_up");
            direction.X += Input.GetActionStrength("pad1_lstick_down");
        }
        direction.Z += Input.GetActionStrength("pad1_dpad_right");
        direction.Z -= Input.GetActionStrength("pad1_dpad_left");
        if (direction.Z == 0)
        {
            direction.Z += Input.GetActionStrength("pad1_lstick_right");
            direction.Z -= Input.GetActionStrength("pad1_lstick_left");
        }
        
        direction = direction.Clamp(-Vector3.One, Vector3.One);
        if (RehabScene.Root.XR_Enabled)
        {
            var camvector = RehabScene.Root.XR_Origin.XR_Camera.GlobalTransform.Basis.Z;
            camvector.Y = 0f;
            camvector = camvector.Normalized();
            var camright = new Vector3(camvector.Z, 0, -camvector.X);
            direction = (direction.X * camvector) + (direction.Z * camright);
        }
        else
        {
            direction = (direction.X * physCam.camvector) + (direction.Z * physCam.camright);
        }
        direction.Y = 0f;
        float dirLength = Math.Abs(direction.Length());
        var pressed = dirLength > 0.05;
        
        if (spinTimer > 0f)
            spinTimer -= delta;
        if (slideTimer > 0f)
            slideTimer -= delta;
        
        if (Input.IsActionPressed("pad1_cross"))
        {
            char_velocity.Y += 80 * delta;
            if (ActiveAnim != 19 && spinTimer <= 0.0)
            {
                DoAnimation(19, false);
                DoSound(0, 1f, 0f);
            }
            isJumping = true;
        }
        if (Input.IsActionJustPressed("pad1_triangle"))
        {
            RehabGame.DisplayHUD();
        }
        if (Input.IsActionJustPressed("pad1_R1"))
        {
            char_velocity.Y = 0f;
            gravityOn = !gravityOn;
            if (gravityOn)
                RehabGame.DisplayMessage("GRAVITY " + Tr("#FE-On"));
            else
                RehabGame.DisplayMessage("GRAVITY " + Tr("#FE-Off"));
        }
        if (Input.IsActionJustPressed("pad1_square") && spinTimer <= 0f && RegFloat[(int)CharFSlot.SpinLength] > 0f && !isCrouched)
        {
            spinTimer = RegFloat[(int)CharFSlot.SpinLength];
            DoAnimation(14, true);
            if (System.Random.Shared.Next(0, 2) == 0)
                DoSound(2, 1f, 0f);
            else
                DoSound(3, 1f, 0f);
        }
        if (Input.IsActionJustPressed("pad1_circle"))
        {
            if (pressed && slideTimer <= 0f && onFloor && RegFloat[(int)CharFSlot.SlideSpeed] > 0f)
            {
                slideTimer = 0.4f;
                DoAnimation(36, true);
                DoSound(6, 1f, 0f);
            }
        }
        if (Input.IsActionPressed("pad1_circle"))
        {
            if (!isCrouched && RegFloat[(int)CharFSlot.CrawlSpeed] > 0f && onFloor && slideTimer <= 0f)
            {
                isCrouched = true;
                DoAnimation(32, true);
            }
        }
        if (Input.IsActionJustReleased("pad1_circle"))
        {
            if (isCrouched)
                isCrouched = false;
        }
        
        var speed = RegFloat[(int)CharFSlot.RunSpeed];
        if (dirLength < 0.3f)
            speed = 0;
        else if (dirLength < 0.8f)
            speed = RegFloat[(int)CharFSlot.WalkSpeed];
        if (isCrouched && pressed)
            speed = RegFloat[(int)CharFSlot.CrawlSpeed];
        if (slideTimer > 0f)
            speed = RegFloat[(int)CharFSlot.SlideSpeed];
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
                GlobalRotation = GlobalRotation.Lerp(targetRot, 5f * delta); //GlobalRotation.Slerp(targetRot, 5f * delta);
                GlobalRotation = new Vector3(0, GlobalRotation.Y, 0);
            }
            if (!isJumping && onFloor && spinTimer <= 0.0 && slideTimer <= 0.0)
            {
                if (isCrouched)
                {
                    if (speed == 0)
                        DoAnimation(32, true);
                    else
                        DoAnimation(34, true);
                }
                else if (speed == 0)
                    DoAnimation(9, true);
                else if (speed == RegFloat[(int)CharFSlot.WalkSpeed])
                    DoAnimation(10, true);
                else
                    DoAnimation(11, true);
            }
        }
        else
        {
            if (slideTimer <= 0f)
            {
                char_velocity.X = 0f;
                char_velocity.Z = 0f;
            }
            if (!isJumping && onFloor && spinTimer <= 0f)
            {
                if (!isCrouched && slideTimer <= 0f)
                    DoAnimation(8, true);
                else
                {
                    if (slideTimer <= 0f)
                        DoAnimation(32, true);
                    else
                        DoAnimation(36, true);
                }
            }
        }
            
        if (!isJumping && !onFloor)
        {
            coyoteTimer -= delta;
            if (coyoteTimer <= 0f && spinTimer <= 0.0 && slideTimer <= 0.0)
            {
                DoAnimation(27, false);
            }
        }
        
        if (!onFloor)
        {
            if (gravityOn && (coyoteTimer <= 0f || isJumping))
                char_velocity.Y -= RegFloat[(int)CharFSlot.AirGravity] * delta;
            else
                char_velocity.Y = 0f;
        }
        else
        {
            coyoteTimer = 0.1f;
        }
        
        Set("velocity", char_velocity);
        Call("move_and_slide");
    }

    void UpdateHeadAnim(float delta)
    {
        if (headdirX > 0f)
        {
            headdirX -= delta * 0.4f;
            headdirX = Math.Clamp(headdirX, 0f, 0.8f);
        }
        else
        {
            headdirX += delta * 0.4f;
            headdirX = Math.Clamp(headdirX, -0.8f, 0f);
        }
        
        if (headdirY > 0f)
        {
            headdirY -= delta * 0.5f;
            headdirY = Math.Clamp(headdirY, 0f, 1.0f);
        }
        else
        {
            headdirY += delta * 0.75f;
            headdirY = Math.Clamp(headdirY, -1.5f, 0f);
        }
        
        var oldX = headdirX;
        var oldY = headdirY;
        if (Input.IsActionPressed("pad1_rstick_left"))
            if (!RehabGame.InvertCameraX)
                headdirX -= Input.GetActionStrength("pad1_rstick_left") * delta * 4.0f;
            else
                headdirX += Input.GetActionStrength("pad1_rstick_left") * delta * 4.0f;
        if (Input.IsActionPressed("pad1_rstick_right"))
            if (!RehabGame.InvertCameraX)
                headdirX += Input.GetActionStrength("pad1_rstick_right") * delta * 4.0f;
            else
                headdirX -= Input.GetActionStrength("pad1_rstick_right") * delta * 4.0f;
        if (Input.IsActionPressed("pad1_rstick_up"))
            if (!RehabGame.InvertCameraY)
                headdirY += Input.GetActionStrength("pad1_rstick_up") * delta * 4.0f;
            else
                headdirY -= Input.GetActionStrength("pad1_rstick_up") * delta * 4.0f;
        if (Input.IsActionPressed("pad1_rstick_down"))
            if (!RehabGame.InvertCameraY)
                headdirY -= Input.GetActionStrength("pad1_rstick_down") * delta * 4.0f;
            else
                headdirY += Input.GetActionStrength("pad1_rstick_down") * delta * 4.0f;
        
        if (isCrouched || slideTimer > 0f)
        {
            headdirX = oldX;
            headdirY = oldY;
        }
        headdirX = Math.Clamp(headdirX, -0.8f, 0.8f);
        headdirY = Math.Clamp(headdirY, -1.5f, 1.0f);
        
        SubModels[ActiveModel].GetNode<AnimationPlayer>("AnimationPlayer").CallbackModeProcess = AnimationMixer.AnimationCallbackModeProcess.Manual;
        SubModels[ActiveModel].GetNode<AnimationPlayer>("AnimationPlayer").Advance(delta);
        if (ActiveSkeleton != null && JointsConst[2] != -1 && ActiveSkeleton.GetBoneCount() > JointsConst[2])
        {
            var headBoneRot = ActiveSkeleton.GetBonePoseRotation(JointsConst[2]);
            var headBoneEuler = headBoneRot.GetEuler();
            headBoneEuler.X += headdirY;
            headBoneEuler.Y += headdirX;
            //headBoneEuler.Z -= headdirX;
            ActiveSkeleton.SetBonePoseRotation(JointsConst[2], Quaternion.FromEuler(headBoneEuler));
        }
    }
    
    void UpdateFootStep(float delta)
    {
        if (ActiveAnim != 11 && ActiveAnim != 10) return;
        
        footsteptimer -= delta;
        if (footsteptimer > 0f) return;

        footsteplast = !footsteplast;
        var clip1 = FS_Dirt_1;
        var clip2 = FS_Dirt_2;
        var space_state = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(GlobalPosition + (Vector3.Up * 1.0f), GlobalPosition + (Vector3.Up * -3.0f));
        var rid = (Rid)Call("get_rid");
        query.Exclude = new Godot.Collections.Array<Rid>() { rid };
        var result = space_state.IntersectRay(query);
        if (result.ContainsKey("collider"))
        {
            var body = (Node)result["collider"];
            switch (body.GetParent().Name)
            {
                case "Normal_Rock":
                    clip1 = FS_Stone_1;
                    clip2 = FS_Stone_2;
                    break;
                case "Normal_Grass":
                    clip1 = FS_Grass_1;
                    clip2 = FS_Grass_2;
                    break;
                case "Normal_Metal":
                case "Slippy_Metal":
                    clip1 = FS_Metal_1;
                    clip2 = FS_Metal_2;
                    break;
                case "Normal_Wood":
                    clip1 = FS_Wood_1;
                    clip2 = FS_Wood_2;
                    break;
                case "Normal_Sand":
                case "Normal_Snow":
                    clip1 = FS_Sand_1;
                    clip2 = FS_Sand_2;
                    break;
                case "Default":
                case "Normal_Mud":
                case "Generic_MediumSlippy":
                case "Lava":
                case "Slippy_Rock":
                case "Sticky_Snow":
                case "Ice":
                case "Ice_LowSlippy":
                case "Generic_MediumSlippy_RigidOnly":
                    clip1 = FS_Dirt_1;
                    clip2 = FS_Dirt_2;
                    break;
                case "Normal_Water":
                    clip1 = FS_Water_1;
                    clip2 = FS_Water_2;
                    break;
                case "Normal_StoneTiles":
                    clip1 = FS_Tile_1;
                    clip2 = FS_Tile_2;
                    break;
                case "Generic_SlightlySlippy":
                case "HackRail":
                    clip1 = FS_Slippy;
                    clip2 = FS_Slippy;
                    break;
                default:
                    clip1 = FS_Stone_1;
                    clip2 = FS_Stone_2;
                    break;
            }

            if (footsteplast)
                clip1 = clip2;
            DoSoundStream(clip1, 1.0f, -5.0f);
            if (ActiveAnim == 10)
                footsteptimer = 0.5f;
            else
                footsteptimer = 0.25f;
        }
           
    }

    void XR_Setup()
    {
        RehabScene.Root.XR_Origin.XR_Camera.Position = Vector3.Zero;
        RehabScene.Root.XR_Origin.GlobalPosition = GlobalPosition + (Vector3.Up * 0.75f);
        RehabScene.Root.XR_Origin.GlobalRotationDegrees = new Vector3(GlobalRotationDegrees.X, GlobalRotationDegrees.Y + 180f, GlobalRotationDegrees.Z);
        XRServer.CenterOnHmd(XRServer.RotationMode.ResetButKeepTilt, true);
        Visible = false;
        string RHandPath = "res://assets/scenes/xr/XRHand_Crash.tscn";
        string LHandPath = "res://assets/scenes/xr/XRHand_Crash.tscn";
        switch (RegInt[(int)CharISlot.AgentType])
        {
            default: break;
            case 1:
                RHandPath = "res://assets/scenes/xr/XRHand_Cortex.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Cortex.tscn";
            break;
            case 2:
                RHandPath = "res://assets/scenes/xr/XRHand_Crunch_Metal.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Crunch.tscn";
            break;
            case 3:
                RHandPath = "res://assets/scenes/xr/XRHand_Nina.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Nina.tscn";
            break;
            case 4:
                RHandPath = "res://assets/scenes/xr/XRHand_EvilCrash.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_EvilCrash.tscn";
            break;
            case 5:
                RHandPath = "res://assets/scenes/xr/XRHand_Mecha_Chainsaw.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Mecha_Rocket.tscn";
            break;
        }
        RehabScene.Root.XR_Origin.XR_HandL.SpawnHand(LHandPath);
        RehabScene.Root.XR_Origin.XR_HandR.SpawnHand(RHandPath);
    }

}