using Godot;
using System;
using System.Collections.Generic;
namespace Rehab;
public partial class CutsceneInteractable : Node3D
{
    [Export]
    bool LockHand;
    [Export]
    bool LockGrip;
    List<RehabXRController> HandInRange = new();

    [Signal]
    public delegate void OnGripEventHandler();
    [Signal]
    public delegate void OnTouchEventHandler();
    [Signal]
    public delegate void OnHandEnterEventHandler();
    [Signal]
    public delegate void OnHandExitEventHandler();

    public override void _Ready()
    {
        Connect("body_entered", Callable.From<Node3D>(OnEnter));
        Connect("body_exited", Callable.From<Node3D>(OnExit));
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (var i in HandInRange)
        {
            if ((LockHand || LockGrip) && !i.HandModel.IsDetached && !i.IsGripping)
            {
                i.HandModel.AttachPos(this);
            }
            EmitSignal(SignalName.OnTouch);
            i.Vibrate(0.1d, 0.2d);
            if (i.IsGripping && (!LockGrip || i.HandModel.IsDetached))
            {
                EmitSignal(SignalName.OnGrip);
                i.Vibrate(1.0d, 1.0d);
            }
        }
    }

    public void OnEnter(Node3D body)
    {
        if (body is RigidBody3D c)
        {
            var parent = c.GetParent();
            while (parent != null)
            {
                if (parent is RehabXRController hand)
                {
                    HandInRange.Add(hand);
                    EmitSignal(SignalName.OnHandEnter);
                    if (LockHand && !hand.HandModel.IsDetached && !hand.IsGripping)
                    {
                        hand.HandModel.AttachPos(this);
                    }
                    break;
                }
                parent = parent.GetParent();
            }
        }
    }

    public void OnExit(Node3D body)
    {
        if (body is RigidBody3D c)
        {
            var parent = c.GetParent();
            while (parent != null)
            {
                if (parent is RehabXRController hand)
                {
                    HandInRange.Remove(hand);
                    EmitSignal(SignalName.OnHandExit);
                    if (hand.HandModel.IsDetached && (!LockGrip || !hand.IsGripping))
                    {
                        hand.HandModel.Reattach();
                    }
                    break;
                }
                parent = parent.GetParent();
            }
        }
    }

}