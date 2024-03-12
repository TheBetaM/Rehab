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
    List<PhysicsBody3D> HandCol = new();

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
        NestedCollisionStart(this);
        HandCont = (RehabXRController)GetParent();
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (var i in HandCol)
        {
            i.Position = Vector3.Zero;
            i.RotationDegrees = Vector3.Zero;
        }
    }

    void NestedCollisionStart(Node parent)
    {
        foreach (var i in parent.GetChildren())
        {
            if (i is RigidBody3D body)
            {
                //body.AddCollisionExceptionWith(PlayerBody);
                //PlayerBody.AddCollisionExceptionWith(body);
                string name = (string)body.Name;
                if (name.Contains("Hand"))
                {
                    body.BodyEntered += CollisionEnter;
                    body.BodyExited += CollisionExit;
                }
                if (body.Name != "RigidBody")
                {
                    HandCol.Add(body);
                }
            }
            else if (i is StaticBody3D sbody)
            {
                HandCol.Add(sbody);
            }
            NestedCollisionStart(i);
        }
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
            break;
        }
    }

    public void OnVector(string name, Vector2 pos, bool isRightHand)
    {
        
    }

    public void OnFloat(string name, double pos, bool isRightHand)
    {

    }

    void CollisionEnter(Node body)
    {
        EmitSignal(SignalName.OnBodyEntered, body);
    }

    void CollisionExit(Node body)
    {
        EmitSignal(SignalName.OnBodyExited, body);
    }

    public void ToggleHandCollisions(bool val)
    {
        foreach (var i in HandCol)
        {
            i.ProcessMode = val ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
        }
    }
}