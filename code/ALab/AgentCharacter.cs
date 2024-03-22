using System.Collections.Generic;
using System;
using System.Linq;
using Godot;
namespace Rehab;
public partial class AgentCharacter : Agent
{
    public enum CharacterType {
        Crash = 0,
        Cortex = 1,
        Coco = 2,
        Nina = 3,
        EvilCrash = 4,
        MechaBandicoot = 5,
    }

    [Export]
    public CharacterType CharType = CharacterType.Crash;
    public Vector3 char_velocity = Vector3.Zero;
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
    public float slideTimer;
    public float coyoteTimer;
    public bool BlockMovement;
    public Vector3 SpawnPos;
    public Vector3 SpawnRot;
    CollisionShape3D DynamicCol;
    //MeshInstance3D DynamicColVis;

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

    public const float AirGravity = 50f;
    public static float[] WalkSpeed = [2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 12f];
    public static float[] RunSpeed = [9f, 7f, 9f, 7f, 7f, 12f];
    public static float[] SpinLength = [0.4f, 0f, 0f, 0.7f, 0f, 0f];
    public static float[] CrawlSpeed = [1.75f, 1.75f, 0f, 0f, 0f, 0f];
    public static float[] SlideSpeed = [18f, 10f, 0f, 0f, 0f, 0f];
    public static float[] SlideTime = [0.4f, 0.4f, 0f, 0f, 0f, 0f];

    public static AgentCharacter activeCharacter;
    public static Dictionary<int, string> ActiveActorTypes = new();

    public override void _Ready()
    {
        base._Ready();

        if (!ActiveActorTypes.ContainsKey((int)CharType))
        {
            ActiveActorTypes[(int)CharType] = GetPath();
        }
        else
        {
            Visible = false;
            ProcessMode = ProcessModeEnum.Disabled;
        }
        
        CreateShadow(0, Vector2.One, 0);

        DynamicCol = new CollisionShape3D();
        //var box = new BoxShape3D();
        var box = new ConvexPolygonShape3D();
        box.Points = [
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 1, 0.5f),
            new Vector3(-0.5f, 1, -0.5f),
            new Vector3(0.5f, 1, -0.5f),
            new Vector3(-0.5f, 1, 0.5f),
        ];
        DynamicCol.Shape = box;
        DynamicCol.Name = "DynamicCol";
        AddChild(DynamicCol);
        //DynamicColVis = new MeshInstance3D();
        //var boxmesh = new BoxMesh();
        //DynamicColVis.Mesh = boxmesh;
        //AddChild(DynamicColVis);
        
        SpawnPos = GlobalPosition;
        SpawnRot = GlobalRotationDegrees;
        if (activeCharacter != null)
            return;
        if (!ParentScene.ActiveScene)
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
        RehabGame.SetCheckPoint(SpawnPos, SpawnRot, ParentScene.Name, true);
    }

    public override void _ExitTree()
    {
        if (isReparenting)
        {
            isReparenting = false;
            return;
        }
        if (ActiveActorTypes.ContainsKey((int)CharType))
        {
            if (ActiveActorTypes[(int)CharType] == GetPath())
                ActiveActorTypes.Remove((int)CharType);
        }
        if (activeCharacter == this)
            activeCharacter = null;
    }

    public override void _PhysicsProcess(double delt)
    {
        ActiveActorTypes[(int)CharType] = GetPath();
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        float delta = (float)delt;
        UpdateDynamicCollision(delta);
        if (activeCharacter != this)
        {
            UpdateNPC(delta);
            return;
        }
        if (Input.IsActionJustPressed("pad1_start"))
        {
            RehabScene.Root.StartPauseMenu(false);
            return;
        }
        
        UpdateMovement(delta);
        UpdateHeadAnim(delta);
        UpdateFootStep(delta);
        if (RehabScene.Root.XR_Enabled)
        {
            RehabScene.Root.XR_Origin.GlobalPosition = GlobalPosition + (Vector3.Up * RehabGame.XR_Height);
        }
    }

    void UpdateMovement(float delta)
    {
        if (BlockMovement) return;
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
        {
            spinTimer -= delta;
            if (spinTimer <= 0f)
            {                
                Call("set_collision_mask_value", 9, true);
                Call("set_collision_mask_value", 10, true);
                Call("set_collision_mask_value", 11, true);
                Call("set_collision_mask_value", 12, true);
                Call("set_collision_mask_value", 13, true);
                Call("set_collision_mask_value", 14, true);
            }
        }
        if (slideTimer > 0f)
        {
            slideTimer -= delta;
            if (slideTimer <= 0f)
            {
                Call("set_collision_mask_value", 9, true);
                Call("set_collision_mask_value", 10, true);
                Call("set_collision_mask_value", 11, true);
                Call("set_collision_mask_value", 12, true);
                Call("set_collision_mask_value", 13, true);
                Call("set_collision_mask_value", 14, true);
            }
        }
        
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
        if (Input.IsActionJustPressed("pad1_square") && spinTimer <= 0f && SpinLength[(int)CharType] > 0f && !isCrouched)
        {
            spinTimer = SpinLength[(int)CharType];
            DoAnimation(14, true);
            if (System.Random.Shared.Next(0, 2) == 0)
                DoSound(2, 1f, 0f);
            else
                DoSound(3, 1f, 0f);
            Call("set_collision_mask_value", 12, true);
            Call("set_collision_mask_value", 9, false);
            Call("set_collision_mask_value", 10, false);
            Call("set_collision_mask_value", 11, false);
            Call("set_collision_mask_value", 13, false);
            Call("set_collision_mask_value", 14, false);
        }
        if (Input.IsActionJustPressed("pad1_circle"))
        {
            if (pressed && slideTimer <= 0f && onFloor && SlideSpeed[(int)CharType] > 0f)
            {
                slideTimer = SlideTime[(int)CharType];
                DoAnimation(36, true);
                DoSound(6, 1f, 0f);
                Call("set_collision_mask_value", 11, true);
                Call("set_collision_mask_value", 9, false);
                Call("set_collision_mask_value", 10, false);
                Call("set_collision_mask_value", 12, false);
                Call("set_collision_mask_value", 13, false);
                Call("set_collision_mask_value", 14, false);
            }
        }
        if (Input.IsActionPressed("pad1_circle"))
        {
            if (!isCrouched && CrawlSpeed[(int)CharType] > 0f && onFloor && slideTimer <= 0f)
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
        
        var speed = RunSpeed[(int)CharType];
        if (dirLength < 0.3f)
            speed = 0;
        else if (dirLength < 0.8f)
            speed = WalkSpeed[(int)CharType];
        if (isCrouched && pressed)
            speed = CrawlSpeed[(int)CharType];
        if (slideTimer > 0f)
            speed = SlideSpeed[(int)CharType];
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
                else if (speed == WalkSpeed[(int)CharType])
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
        var colCount1 = (int)Call("get_slide_collision_count");
        if (colCount1 == 0) return;
        for (int i = 0; i < colCount1; i++)
        {
            var colData = (KinematicCollision3D)Call("get_slide_collision", i);
            if (colData == null) continue;
            var colCount = colData.GetCollisionCount();
            if (colCount == 0) continue;
            for (int a = 0; a < colCount; a++)
            {
                var hit = colData.GetCollider(a);
                if (hit is Area3D area && area.GetParent() is Agent agent)
                {
                    hit = agent;
                }
                if (hit is AgentCrate crate)
                {
                    crate.OnBodyEntered(this);
                }
                else if (hit is AgentFurniture furn)
                {
                    furn.OnDoorTouch(this);
                }
            }
        }
        
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
    
    public void UpdateFootStep(float delta)
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

    public void UpdateDynamicCollision(float delta)
    {
        //Aabb box = new Aabb();
        //UpdateDynamicColNested(SubModels[ActiveModel], ref box);
        
        float MinX = 10000f, MinZ = 10000f, MaxX = -10000f, MaxY = -10000f, MaxZ = -10000f;;
        for (int i = 0; i < ActiveSkeleton.GetBoneCount(); i++)
        {
            var pos = ActiveSkeleton.GetBoneGlobalPose(i);
            if (pos.Origin.X < MinX) MinX = pos.Origin.X;
            if (pos.Origin.Z < MinZ) MinZ = pos.Origin.Z;
            if (pos.Origin.X > MaxX) MaxX = pos.Origin.X;
            if (pos.Origin.Y > MaxY) MaxY = pos.Origin.Y;
            if (pos.Origin.Z > MaxZ) MaxZ = pos.Origin.Z;
        }
        if (spinTimer > 0f)
        {
            MaxY = 1.8f;
            MinX = -1.25f;
            MaxX = 1.25f;
            MinZ = -1.25f;
            MaxZ = 1.25f;
        }
        /*
        Vector3 center = new Vector3(0f, MaxY / 2f, 0f);
        Vector3 size = new Vector3((MaxX - MinX) + 0.1f, MaxY, (MaxZ - MinZ) + 0.1f);
        DynamicCol.Position = DynamicCol.Position.MoveToward(center, delta * 10f);
        var oldSize = (Vector3)DynamicCol.Shape.Get("size");
        DynamicCol.Shape.Set("size", oldSize.MoveToward(size, delta * 10f));
        */
        Vector3 size = new Vector3((MaxX - MinX) + 0.1f, MaxY, (MaxZ - MinZ) + 0.1f);
        var oldPts = (Vector3[])DynamicCol.Shape.Get("points");
        Vector3[] Points = [
            oldPts[0].MoveToward(new Vector3(0.5f, 0, 0.5f) * size, 10f * delta),
            oldPts[1].MoveToward(new Vector3(-0.5f, 0, -0.5f) * size, 10f * delta),
            oldPts[2].MoveToward(new Vector3(0.5f, 0, -0.5f) * size, 10f * delta),
            oldPts[3].MoveToward(new Vector3(-0.5f, 0, 0.5f) * size, 10f * delta),
            oldPts[4].MoveToward(new Vector3(0.5f, 1, 0.5f) * size, 10f * delta),
            oldPts[5].MoveToward(new Vector3(-0.5f, 1, -0.5f) * size, 10f * delta),
            oldPts[6].MoveToward(new Vector3(0.5f, 1, -0.5f) * size, 10f * delta),
            oldPts[7].MoveToward(new Vector3(0.5f, 1, 0.5f) * size, 10f * delta),
        ];
        DynamicCol.Shape.Set("points", Points);
        
        //var oldSize = (Vector3)DynamicCol.Shape.Get("size");
        //DynamicCol.Position = box.Position + (box.Size / 2f);
        //DynamicCol.Shape.Set("size", oldSize.MoveToward(box.Size, delta * 10f));
        //DynamicCol.Shape.Set("size", box.Size);
        //DynamicColVis.Position = DynamicCol.Position;
        //DynamicColVis.Mesh.Set("size", size);
    }

    void UpdateDynamicColNested(Node parent, ref Aabb inBox)
    {
        if (parent is MeshInstance3D mesh)
        {
            inBox = inBox.Merge(mesh.Mesh.GetAabb());
        }
        foreach (var i in parent.GetChildren())
        {
            UpdateDynamicColNested(i, ref inBox);
        }
    }

    void XR_Setup()
    {
        RehabScene.Root.XR_Origin.XR_Camera.Position = Vector3.Zero;
        RehabScene.Root.XR_Origin.GlobalPosition = GlobalPosition + (Vector3.Up * RehabGame.XR_Height);
        RehabScene.Root.XR_Origin.GlobalRotationDegrees = new Vector3(GlobalRotationDegrees.X, GlobalRotationDegrees.Y + 180f, GlobalRotationDegrees.Z);
        XRServer.CenterOnHmd(XRServer.RotationMode.ResetButKeepTilt, true);
        Visible = false;
        string RHandPath = "res://assets/scenes/xr/XRHand_Crash.tscn";
        string LHandPath = "res://assets/scenes/xr/XRHand_Crash.tscn";
        switch (CharType)
        {
            default: break;
            case CharacterType.Cortex:
                RHandPath = "res://assets/scenes/xr/XRHand_Cortex.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Cortex.tscn";
            break;
            case CharacterType.Coco:
                RHandPath = "res://assets/scenes/xr/XRHand_Crunch_Metal.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Crunch.tscn";
            break;
            case CharacterType.Nina:
                RHandPath = "res://assets/scenes/xr/XRHand_Nina.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Nina.tscn";
            break;
            case CharacterType.EvilCrash:
                RHandPath = "res://assets/scenes/xr/XRHand_EvilCrash.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_EvilCrash.tscn";
            break;
            case CharacterType.MechaBandicoot:
                RHandPath = "res://assets/scenes/xr/XRHand_Mecha_Chainsaw.tscn";
                LHandPath = "res://assets/scenes/xr/XRHand_Mecha_Rocket.tscn";
            break;
        }
        RehabScene.Root.XR_Origin.XR_HandL.SpawnHand(LHandPath, this);
        RehabScene.Root.XR_Origin.XR_HandR.SpawnHand(RHandPath, this);
        //AudioSource.AttenuationModel = AudioStreamPlayer3D.AttenuationModelEnum.Disabled;
    }

    void UpdateNPC(float delta)
    {
        DoAnimation(8, true);
    }
}