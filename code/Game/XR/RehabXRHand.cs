using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabXRHand : Node3D
{
    AnimationPlayer HandAnim;
    public RehabXRController HandCont;
    public PhysicsBody3D PlayerBody;
    bool HasAnim;
    public bool IsDetached;
    Vector3 OrigScale;
    
    [Export]
    public bool IsNinaHand;
    public bool IsRestricted;
    public bool IsFired;

    [Signal]
    public delegate void OnBodyEnteredEventHandler(Node body);
    [Signal]
    public delegate void OnBodyExitedEventHandler(Node body);

    Node3D GrabTarget;
    Node3D MultiToolTarget;
    Node3D NinaHandFired;
    Node3D NinaHandSpring;
    AudioStreamPlayer3D HandAudio;
    Node3D HandTarget;
    Node3D NinaHandMain;
    AnimationPlayer NinaHandFiredAnim;
    RigidBody3D NinaHandFiredBody;
    bool IsNinaHandFired;
    bool IsNinaHandRetracting;
    bool NinaHandHasTarget;
    Vector3 NinaHandLastTargetPoint;
    bool IsNinaHandGrappling;
    double ShakyHandTimer;
    bool NinaHandColBuffer;

    public override void _Ready()
    {
        if (GetNodeOrNull("HandAnim") != null)
        {
            HasAnim = true;
            HandAnim = GetNode<AnimationPlayer>("HandAnim");
        }
        HandCont = (RehabXRController)GetParent();
        if (IsNinaHand)
        {
            GrabTarget = GetNode<Node3D>("NinaGrabTarget");
            NinaHandFired = GetNode<Node3D>("NinaHandFired");
            MultiToolTarget = GetNode<Node3D>("MultiToolTarget");
            NinaHandSpring = GetNode<Node3D>("NinaHandSpring");
            HandAudio = GetNode<AudioStreamPlayer3D>("Audio");
            NinaHandMain = GetNode<Node3D>("Rig_NinaHand");
            NinaHandFiredAnim = GetNode<AnimationPlayer>("NinaHandFired/AnimationPlayer");
            NinaHandFiredBody = GetNode<RigidBody3D>("NinaHandFired/RigidBody");
            NinaHandFiredBody.Connect("body_entered", Callable.From<Node3D>(NinaFiredHand_BodyEntered));
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsNinaHand) return;
        if (IsNinaHandFired) 
        {
            UpdateFiredNinaHand((float)delta);
            return;
        }
        if (HandCont.IsGripping) return;
        Nina_UpdateTarget(delta);
    }

    public void OnButtonDown(string name, bool isRightHand)
    {
        if (!HasAnim || IsFired) return;
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        switch (name)
        {
            default: break;
            case "grip_click":
                HandAnim.Play(name);
                if (IsNinaHand && !IsNinaHandFired)
                {
                    if (GrabTarget.Visible)
                    {
                        Nina_RemoveTarget();
                    }
                }
            break;
            case "trigger_touch":
                if (IsDetached || HandCont.IsGripping || IsNinaHandFired) return;
                HandAnim.Play(name);
            break;
            case "ax_touch":
                if (IsDetached || HandCont.IsGripping || IsNinaHandFired) return;
                HandAnim.Play("primary_touch");
            break;
            case "trigger_click":
                if (IsDetached || IsRestricted || IsNinaHandFired) return;
                if (IsNinaHand)
                    Nina_FireFist();
            break;
        }
    }

    public void OnButtonRelease(string name, bool isRightHand)
    {
        if (!HasAnim || IsFired) return;
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        switch (name)
        {
            default: break;
            case "grip_click":
                HandAnim.Play(name + "_release");
                if (IsDetached)
                {
                    Reattach();
                }
            break;
            case "trigger_touch":
                if (IsDetached || HandCont.IsGripping || IsNinaHandFired) return;
                HandAnim.Play(name + "_release");
            break;
            case "ax_touch":
                if (IsDetached || HandCont.IsGripping || IsNinaHandFired) return;
                HandAnim.Play("primary_touch_release");
            break;
        }
    }

    public void OnVector(string name, Vector2 pos, bool isRightHand)
    {
        
    }

    public void OnFloat(string name, double pos, bool isRightHand)
    {
        if (!HasAnim || IsFired) return;
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        switch (name)
        {
            default: break;
            case "trigger":
                if (IsDetached || IsNinaHandFired) return;
                if (Math.Abs(pos) > 0.05f && !HandCont.IsGripping)
                {
                    HandAnim.Play("trigger_touch_release");
                }
            break;
        }
    }

    public void Attach(Node3D target)
    {
        OrigScale = Scale;
        IsDetached = true;
        Reparent(target);
        Position = Vector3.Zero;
        RotationDegrees = Vector3.Zero;
        Scale = Scale * OrigScale;
        HandAnim.Play("RESET");
    }
    public void AttachPos(Node3D target)
    {
        OrigScale = Scale;
        IsDetached = true;
        Reparent(RehabScene.Root);
        GlobalPosition = target.GlobalPosition;
        GlobalRotationDegrees = target.GlobalRotationDegrees;
        Scale = Scale *  OrigScale;
        HandAnim.Play("RESET");
    }

    public void Reattach()
    {
        if (!IsDetached) return;
        IsDetached = false;
        Reparent(HandCont);
        Position = Vector3.Zero;
        RotationDegrees = Vector3.Zero;
        Scale = OrigScale;
        HandAnim.Play("RESET");
    }

    public void UpdateLayers(int layer)
    {
	    UpdateLayersNested(this, layer);
    }

    void UpdateLayersNested(Node i, int layer)
    {
        if (i is VisualInstance3D vis)
        {
            for (int a = 1; a < 9; a++)
            {
                vis.SetLayerMaskValue(a, false);
            }
            vis.SetLayerMaskValue(layer, true);
        }
        foreach (var id in i.GetChildren())
            UpdateLayersNested(id, layer);
    }

    void Nina_UpdateTarget(double delta)
    {
        var spaceState = RehabScene.Root.GetWorld3D().DirectSpaceState;
        Vector3 pos = HandCont.GlobalPosition;
        Vector3 target = HandCont.GlobalPosition + (-HandCont.GlobalTransform.Basis.Z * 20f);
        Vector3[] QueryPos = [
            pos,
            pos + (HandCont.GlobalTransform.Basis.X * 1f) + (HandCont.GlobalTransform.Basis.Y * 1f),
            pos + (HandCont.GlobalTransform.Basis.X * -1f) + (HandCont.GlobalTransform.Basis.Y * -1f),
            pos + (HandCont.GlobalTransform.Basis.X * 1f) + (HandCont.GlobalTransform.Basis.Y * -1f),
            pos + (HandCont.GlobalTransform.Basis.X * -1f) + (HandCont.GlobalTransform.Basis.Y * 1f),

            pos + (HandCont.GlobalTransform.Basis.X * 0.5f) + (HandCont.GlobalTransform.Basis.Y * 0.5f),
            pos + (HandCont.GlobalTransform.Basis.X * -0.5f) + (HandCont.GlobalTransform.Basis.Y * -0.5f),
            pos + (HandCont.GlobalTransform.Basis.X * 0.5f) + (HandCont.GlobalTransform.Basis.Y * -0.5f),
            pos + (HandCont.GlobalTransform.Basis.X * -0.5f) + (HandCont.GlobalTransform.Basis.Y * 0.5f),

            pos + (HandCont.GlobalTransform.Basis.X * 1.5f) + (HandCont.GlobalTransform.Basis.Y * 1.5f),
            pos + (HandCont.GlobalTransform.Basis.X * -1.5f) + (HandCont.GlobalTransform.Basis.Y * -1.5f),
            pos + (HandCont.GlobalTransform.Basis.X * 1.5f) + (HandCont.GlobalTransform.Basis.Y * -1.5f),
            pos + (HandCont.GlobalTransform.Basis.X * -1.5f) + (HandCont.GlobalTransform.Basis.Y * 1.5f),
        ];
        Vector3[] QueryPoints = [
            target,
            target + (HandCont.GlobalTransform.Basis.X * 1f) + (HandCont.GlobalTransform.Basis.Y * 1f),
            target + (HandCont.GlobalTransform.Basis.X * -1f) + (HandCont.GlobalTransform.Basis.Y * -1f),
            target + (HandCont.GlobalTransform.Basis.X * 1f) + (HandCont.GlobalTransform.Basis.Y * -1f),
            target + (HandCont.GlobalTransform.Basis.X * -1f) + (HandCont.GlobalTransform.Basis.Y * 1f),

            target + (HandCont.GlobalTransform.Basis.X * 0.5f) + (HandCont.GlobalTransform.Basis.Y * 0.5f),
            target + (HandCont.GlobalTransform.Basis.X * -0.5f) + (HandCont.GlobalTransform.Basis.Y * -0.5f),
            target + (HandCont.GlobalTransform.Basis.X * 0.5f) + (HandCont.GlobalTransform.Basis.Y * -0.5f),
            target + (HandCont.GlobalTransform.Basis.X * -0.5f) + (HandCont.GlobalTransform.Basis.Y * 0.5f),

            target + (HandCont.GlobalTransform.Basis.X * 1.5f) + (HandCont.GlobalTransform.Basis.Y * 1.5f),
            target + (HandCont.GlobalTransform.Basis.X * -1.5f) + (HandCont.GlobalTransform.Basis.Y * -1.5f),
            target + (HandCont.GlobalTransform.Basis.X * 1.5f) + (HandCont.GlobalTransform.Basis.Y * -1.5f),
            target + (HandCont.GlobalTransform.Basis.X * -1.5f) + (HandCont.GlobalTransform.Basis.Y * 1.5f),
        ];
        for (int i = 0; i < QueryPoints.Length; i++)
        {
            var query = PhysicsRayQueryParameters3D.Create(QueryPos[i], QueryPoints[i], (uint)RehabScene.Root.ActiveChunk.ChunkLayer);
            var result = spaceState.IntersectRay(query);
            if (!result.ContainsKey("collider")) continue;
            var hit = (GodotObject)result["collider"];
            if (hit is Node3D node)
            {
                if (node == HandTarget) return;
                if (node is AgentCreature || node is AgentChiChiGrass)
                {
                    var rPos = (Vector3)result["position"];
                    Nina_AttachTarget(node, rPos);
                    return;
                }
            }
        }
        if (GrabTarget.Visible)
        {
            if (ShakyHandTimer <= 0f) 
            {
                ShakyHandTimer = 0.2f;
                return;
            }
            ShakyHandTimer -= delta;
            if (ShakyHandTimer <= 0f)
            {
                Nina_RemoveTarget();
            }
        }
    }

    void Nina_AttachTarget(Node3D node, Vector3 pos)
    {
        HandTarget = node;
        GrabTarget.Reparent(RehabScene.Root);
        GrabTarget.GlobalPosition = pos;
        GrabTarget.LookAt(GlobalPosition);
        GrabTarget.Visible = true;
        NinaHandHasTarget = true;
        NinaHandLastTargetPoint = pos;
    }

    void Nina_RemoveTarget()
    {
        HandTarget = null;
        GrabTarget.Visible = false;
        GrabTarget.Reparent(this);
        GrabTarget.Position = Vector3.Zero;
        NinaHandHasTarget = false;
    }

    void Nina_FireFist()
    {
        NinaHandFired.GlobalPosition = NinaHandMain.GlobalPosition;
        NinaHandFired.GlobalRotationDegrees = NinaHandMain.GlobalRotationDegrees;
        if (HandCont.IsGripping)
            NinaHandFiredAnim.Play("FullGrip");
        else
            NinaHandFiredAnim.PlayBackwards("FullGrip");
        NinaHandFiredAnim.Advance(0.25f);
        NinaHandMain.Visible = false;
        NinaHandFired.Visible = true;
        string SoundPath = RehabGame.AssetsPath + "Sounds/Nina_SFX_2.res";
        if (HandCont.Tracker == "right_hand") SoundPath = RehabGame.AssetsPath + "Sounds/Nina_SFX_3.res";
        HandAudio.Stream = (AudioStream)ResourceLoader.Load(SoundPath);
        HandAudio.Play();
        NinaHandFired.Reparent(RehabScene.Root);
        IsNinaHandFired = true;
        IsNinaHandRetracting = false;
        IsNinaHandGrappling = !HandCont.IsGripping;
        AgentCharacter.activeCharacter.Call("add_collision_exception_with", NinaHandFiredBody);
        NinaHandFiredBody.AddCollisionExceptionWith(AgentCharacter.activeCharacter);
        Nina_FireFistBody();
    }
    async void Nina_FireFistBody()
    {
        NinaHandColBuffer = true;
        NinaHandFiredBody.ProcessMode = ProcessModeEnum.Inherit;
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        NinaHandColBuffer = false;
    }

    public void Nina_ResetFist()
    {
        IsNinaHandFired = false;
        IsNinaHandRetracting = false;
        NinaHandMain.Visible = true;
        NinaHandFired.Visible = false;
        NinaHandFired.Reparent(this);
        NinaHandFired.GlobalPosition = NinaHandMain.GlobalPosition;
        NinaHandFired.GlobalRotationDegrees = NinaHandMain.GlobalRotationDegrees;
    }

    public void NinaFiredHand_BodyEntered(Node3D body)
    {
        if (NinaHandColBuffer) return;
        if (NinaHandHasTarget)
        {
            if (body != HandTarget)
            {
                if (body is AgentPickup pickup)
                {
                    if (IsNinaHandGrappling)
                    {
                        pickup.ForcePickup(this);
                    }
                    else
                    {
                        pickup.ForceSpun(this);
                    }
                    return;
                }
                NinaFiredHand_TargetReached(body);
            }
        }
        else
        {
            NinaFiredHand_TargetReached(body);
        }
    }
    public void NinaFiredHand_TargetReached(Node3D body)
    {
        IsNinaHandRetracting = true;
        if (body is AgentChiChiGrass hook && IsNinaHandGrappling)
        {
            string SoundPath = RehabGame.AssetsPath + "Sounds/Nina_SFX_4.res";
            HandAudio.Stream = (AudioStream)ResourceLoader.Load(SoundPath);
            HandAudio.Play();
            string name = (string)hook.Name;
            if (name.Contains("Ceiling") && hook.LinkPoint != null && hook.LinkPoint.Count != 0)
            {
                Nina_CeilingHookTravel(hook);
            }
            else
            {
                Nina_WallHookTravel(hook);
            }
        }
        else if (body is AgentCreature enemy)
        {
            if (IsNinaHandGrappling)
            {
                enemy.ForcePanic();
            }
            else
            {
                enemy.ForceDeath();
            }
        }
        else if (body is AgentPickup pickup)
        {
            if (IsNinaHandGrappling)
            {
                pickup.ForcePickup(this);
            }
            else
            {
                pickup.ForceSpun(this);
            }
        }
        Nina_RemoveTarget();
        CallDeferred("DisableFiredHandCol");
    }

    public void DisableFiredHandCol()
    {
        NinaHandFiredBody.ProcessMode = ProcessModeEnum.Disabled;
    }

    void UpdateFiredNinaHand(float delta)
    {
        if (NinaHandFired.GlobalPosition.DistanceTo(GlobalPosition) > 20f)
        {
            IsNinaHandRetracting = true;
        }
        if (IsNinaHandRetracting)
        {
            NinaHandFired.GlobalPosition = NinaHandFired.GlobalPosition.MoveToward(GlobalPosition, delta * 40f);
            if (NinaHandFired.GlobalPosition.DistanceTo(GlobalPosition) < 0.1f)
            {
                Nina_ResetFist();
            }
        }
        else
        {
            if (NinaHandHasTarget)
            {
                NinaHandFired.GlobalPosition = NinaHandFired.GlobalPosition.MoveToward(NinaHandLastTargetPoint, delta * 20f);
                if (NinaHandFired.GlobalPosition.DistanceTo(NinaHandLastTargetPoint) < 0.1f)
                {
                    NinaFiredHand_TargetReached(HandTarget);
                    IsNinaHandRetracting = true;
                }
            }
            else
            {
                NinaHandFired.Translate(-GlobalTransform.Basis.Z * delta * 20f);
            }
        }
    }

    async void Nina_CeilingHookTravel(AgentChiChiGrass hook)
    {
        AgentCharacter.activeCharacter.BlockMovement = true;
        AgentCharacter.activeCharacter.ProcessMode = ProcessModeEnum.Disabled;
        AgentCharacter.activeCharacter.GlobalPosition = hook.LinkPoint[0].GlobalPosition;
        AgentCharacter.activeCharacter.GlobalRotation = hook.LinkPoint[0].GlobalRotation;
        RehabScene.Root.XR_CameraCut(0.85f);
        CallDeferred("Nina_ResetFist");
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        string SoundPath = RehabGame.AssetsPath + "Sounds/Nina_SFX_1.res";
        HandAudio.Stream = (AudioStream)ResourceLoader.Load(SoundPath);
        HandAudio.Play();
        AgentCharacter.activeCharacter.ProcessMode = ProcessModeEnum.Inherit;
        AgentCharacter.activeCharacter.BlockMovement = false;
    }

    void Nina_WallHookTravel(AgentChiChiGrass hook)
    {
        AgentCharacter.activeCharacter.BlockMovement = true;
        AgentCharacter.activeCharacter.GlobalPosition = hook.GlobalPosition + (hook.GlobalTransform.Basis.Z * 2f);
        RehabScene.Root.XR_CameraCut(0.25f);
        CallDeferred("Nina_ResetFist");
    }

}