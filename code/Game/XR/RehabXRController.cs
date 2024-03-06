using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabXRController : XRController3D
{
    public bool HasHand = false;
    public Node3D HandModel;

    public override void _Ready()
    {
        ButtonPressed += OnButtonDown;
        ButtonReleased += OnButtonRelease;
        InputVector2Changed += OnStick;
    }

    void OnButtonDown(string name)
    {
        switch (name)
        {
            default: break;
            case "trigger_click":
                if (Tracker == "right_hand")
                {
                    var MEvent = new InputEventScreenTouch();
                    MEvent.Position = RehabScene.Root.FE_XR_Viewport.GetMousePosition() * 2.64f;
                    MEvent.Pressed = true;
                    Input.ParseInputEvent(MEvent);
                }
            break;
            case "primary_click":
                var StickClickEvent = new InputEventJoypadButton();
                if (Tracker == "right_hand")
                    StickClickEvent.ButtonIndex = JoyButton.RightShoulder;
                else
                    StickClickEvent.ButtonIndex = JoyButton.LeftShoulder;
                StickClickEvent.Pressed = true;
                Input.ParseInputEvent(StickClickEvent);
            break;
            case "ax_button":
                var XEvent = new InputEventJoypadButton();
                if (Tracker == "right_hand")
                    XEvent.ButtonIndex = JoyButton.A;
                else
                    XEvent.ButtonIndex = JoyButton.X;
                XEvent.Pressed = true;
                Input.ParseInputEvent(XEvent);
            break;
            case "by_button":
                var YEvent = new InputEventJoypadButton();
                if (Tracker == "right_hand")
                    YEvent.ButtonIndex = JoyButton.B;
                else
                    YEvent.ButtonIndex = JoyButton.Y;
                YEvent.Pressed = true;
                Input.ParseInputEvent(YEvent);
            break;
            case "menu_button":
                if (Tracker == "right_hand")
                {
                    RehabScene.Root.XR_Origin.ResetOrientation();
                }
                else
                {
                    RehabScene.Root.XR_Origin.ResetOrientation();
                    var SEvent = new InputEventJoypadButton();
                    SEvent.ButtonIndex = JoyButton.Start;
                    SEvent.Pressed = true;
                    Input.ParseInputEvent(SEvent);
                }
            break;
        }
    }

    void OnButtonRelease(string name)
    {
        switch (name)
        {
            default: break;
            case "trigger_click":
                if (Tracker == "right_hand")
                {
                    var MEvent = new InputEventScreenTouch();
                    MEvent.Pressed = false;
                    Input.ParseInputEvent(MEvent);
                }
            break;
            case "primary_click":
                var StickClickEvent = new InputEventJoypadButton();
                if (Tracker == "right_hand")
                    StickClickEvent.ButtonIndex = JoyButton.RightShoulder;
                else
                    StickClickEvent.ButtonIndex = JoyButton.LeftShoulder;
                StickClickEvent.Pressed = false;
                Input.ParseInputEvent(StickClickEvent);
            break;
            case "ax_button":
                var XEvent = new InputEventJoypadButton();
                if (Tracker == "right_hand")
                    XEvent.ButtonIndex = JoyButton.A;
                else
                    XEvent.ButtonIndex = JoyButton.X;
                XEvent.Pressed = false;
                Input.ParseInputEvent(XEvent);
            break;
            case "by_button":
                var YEvent = new InputEventJoypadButton();
                if (Tracker == "right_hand")
                    YEvent.ButtonIndex = JoyButton.B;
                else
                    YEvent.ButtonIndex = JoyButton.Y;
                YEvent.Pressed = false;
                Input.ParseInputEvent(YEvent);
            break;
            case "menu_button":
                if (Tracker == "right_hand")
                {
                    RehabScene.Root.XR_Origin.ResetOrientation();
                }
                else
                {
                    RehabScene.Root.XR_Origin.ResetOrientation();
                    var SEvent = new InputEventJoypadButton();
                    SEvent.ButtonIndex = JoyButton.Start;
                    SEvent.Pressed = false;
                    Input.ParseInputEvent(SEvent);
                }
            break;
        }
    }

    void OnStick(string name, Vector2 pos)
    {
        switch (name)
        {
            default: break;
            case "primary":
                var XEvent = new InputEventJoypadMotion();
                if (Tracker == "right_hand")
                    XEvent.Axis = JoyAxis.RightX;
                else
                    XEvent.Axis = JoyAxis.LeftX;
                XEvent.AxisValue = pos.X;
                var YEvent = new InputEventJoypadMotion();
                if (Tracker == "right_hand")
                    YEvent.Axis = JoyAxis.RightY;
                else
                    YEvent.Axis = JoyAxis.LeftY;
                YEvent.AxisValue = -pos.Y;
                Input.ParseInputEvent(XEvent);
                Input.ParseInputEvent(YEvent);
            break;
        }
    }

    public void SpawnHand(string path)
    {
        if (HasHand)
        {
            HandModel.QueueFree();
            HasHand = false;
        }
        if (!ResourceLoader.Exists(path)) return;
        var scene = (PackedScene)ResourceLoader.Load(path);
        var hand = (Node3D)scene.Instantiate();
        if (Tracker != "right_hand")
        {
            hand.Scale = new Vector3(-hand.Scale.X, hand.Scale.Y, hand.Scale.Z);
        }
        hand.Scale *= 0.65f;
        HandModel = hand;
        HasHand = true;
        AddChild(hand);
    }

    public void ClearHand()
    {
        if (HasHand)
        {
            HandModel.QueueFree();
            HasHand = false;
        }
    }
}