using Godot;
using System;
using System.Collections.Generic;
namespace Rehab;
public partial class CutsceneInteractable : Node3D
{
    [Export]
    bool LockHand;
    List<RehabXRHand> HandInRange = new();

    [Signal]
    public delegate void OnGripEventHandler();
    [Signal]
    public delegate void OnTouchEventHandler();

    public override void _Ready()
    {
        Connect("body_entered", Callable.From<Node3D>(OnEnter));
        Connect("body_exited", Callable.From<Node3D>(OnExit));
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (var i in HandInRange)
        {
            EmitSignal(SignalName.OnTouch);
            if (i.HandCont.IsGripping)
            {
                EmitSignal(SignalName.OnGrip);
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
                if (parent is RehabXRHand hand)
                {
                    HandInRange.Add(hand);
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
                if (parent is RehabXRHand hand)
                {
                    HandInRange.Remove(hand);
                    break;
                }
                parent = parent.GetParent();
            }
        }
    }

}