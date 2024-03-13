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

    [Signal]
    public delegate void OnBodyEnteredEventHandler(Node body);
    [Signal]
    public delegate void OnBodyExitedEventHandler(Node body);

    public override void _Ready()
    {
        if (GetNodeOrNull("HandAnim") != null)
        {
            HasAnim = true;
            HandAnim = GetNode<AnimationPlayer>("HandAnim");
        }
        HandCont = (RehabXRController)GetParent();
    }

    public void OnButtonDown(string name, bool isRightHand)
    {
        if (!HasAnim) return;
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        switch (name)
        {
            default: break;
            case "grip_click":
                HandAnim.Play(name);
            break;
        }
    }

    public void OnButtonRelease(string name, bool isRightHand)
    {
        if (!HasAnim) return;
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
        }
    }

    public void OnVector(string name, Vector2 pos, bool isRightHand)
    {
        
    }

    public void OnFloat(string name, double pos, bool isRightHand)
    {

    }

    public void Attach(Node3D target)
    {
        OrigScale = Scale;
        IsDetached = true;
        Reparent(target);
        Position = Vector3.Zero;
        RotationDegrees = Vector3.Zero;
        Scale = Scale * OrigScale;
    }
    public void AttachPos(Node3D target)
    {
        OrigScale = Scale;
        IsDetached = true;
        Reparent(RehabScene.Root);
        GlobalPosition = target.GlobalPosition;
        GlobalRotationDegrees = target.GlobalRotationDegrees;
        Scale = Scale *  OrigScale;
    }

    public void Reattach()
    {
        if (!IsDetached) return;
        IsDetached = false;
        Reparent(HandCont);
        Position = Vector3.Zero;
        RotationDegrees = Vector3.Zero;
        Scale = OrigScale;
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

}