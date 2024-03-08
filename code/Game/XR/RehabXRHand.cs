using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabXRHand : Node3D
{
    AnimationPlayer HandAnim;
    RehabXRController HandCont;

    public override void _Ready()
    {
        HandAnim = GetNode<AnimationPlayer>("HandAnim");
        HandCont = (RehabXRController)GetParent();
    }

    public void OnButtonDown(string name, bool isRightHand)
    {
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
}