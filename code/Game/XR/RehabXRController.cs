using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabXRController : XRController3D
{
    public bool HasHand = false;
    public RehabXRHand HandModel;
    public bool ActiveCursor = false;
    public RehabXROrigin Origin;
    public RehabXRHand Hand;
    public bool HandModelDynamic;
    public bool IsGripping;
    public RigidBody3D HandCol;
    bool IsFEActive;

    public override void _Ready()
    {
        ButtonPressed += OnButtonDown;
        ButtonReleased += OnButtonRelease;
        InputVector2Changed += OnVector;
        InputFloatChanged += OnFloat;
        HandCol = GetChild<RigidBody3D>(0);
    }

    public override void _PhysicsProcess(double delta)
    {
        HandCol.Position = Vector3.Zero;
        HandCol.RotationDegrees = Vector3.Zero;
    }

    void OnButtonDown(string name)
    {
        switch (name)
        {
            default: break;
            case "trigger_click":
                if (!ActiveCursor)
                    return;
                var MEvent = new InputEventScreenTouch();
                MEvent.Position = RehabScene.Root.FE_XR_Viewport.GetMousePosition() * 2.64f;
                MEvent.Pressed = true;
                Input.ParseInputEvent(MEvent);
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
            case "grip_click":
                IsGripping = true;
            break;
        }
        if (HandModelDynamic && !IsFEActive)
            Hand.OnButtonDown(name, Tracker == "right_hand");
    }

    void OnButtonRelease(string name)
    {
        switch (name)
        {
            default: break;
            case "trigger_click":
                if (!ActiveCursor)
                {
                    Origin.XR_HandL.ActiveCursor = false;
                    Origin.XR_HandR.ActiveCursor = false;
                    ActiveCursor = true;
                    return;
                }
                var MEvent = new InputEventScreenTouch();
                MEvent.Pressed = false;
                Input.ParseInputEvent(MEvent);
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
            case "grip_click":
                IsGripping = false;
            break;
        }
        if (HandModelDynamic && !IsFEActive)
            Hand.OnButtonRelease(name, Tracker == "right_hand");
    }

    void OnVector(string name, Vector2 pos)
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
        if (HandModelDynamic && !IsFEActive)
            Hand.OnVector(name, pos, Tracker == "right_hand");
    }

    void OnFloat(string name, double pos)
    {
        if (HandModelDynamic && !IsFEActive)
            Hand.OnFloat(name, pos, Tracker == "right_hand");
    }

    public void SpawnHand(string path, Node body)
    {
        if (HasHand)
        {
            HandModelDynamic = false;
            HandModel.QueueFree();
            HasHand = false;
        }
        if (!ResourceLoader.Exists(path)) return;
        HandModelDynamic = false;
        var scene = (PackedScene)ResourceLoader.Load(path);
        var hand = (Node3D)scene.Instantiate();
        if (Tracker != "right_hand")
        {
            hand.Scale = new Vector3(-hand.Scale.X, hand.Scale.Y, hand.Scale.Z);
        }
        if (hand is RehabXRHand ahand)
        {
            Hand = ahand;
            HandModelDynamic = true;
            if (body is PhysicsBody3D phys)
            {
                ahand.PlayerBody = phys;
            }
        }
        HandModel = (RehabXRHand)hand;
        HasHand = true;
        AddChild(hand);
    }

    public void ClearHand()
    {
        HandModelDynamic = false;
        if (HasHand)
        {
            HandModel.QueueFree();
            HasHand = false;
        }
    }

    public void FE_Active()
    {
        if (HasHand)
        {
            HandModel.ProcessMode = ProcessModeEnum.Disabled;
        }
        IsFEActive = true;
    }

    public void FE_Inactive()
    {
        if (HasHand)
        {
            HandModel.ProcessMode = ProcessModeEnum.Inherit;
        }
        IsFEActive = false;
    }

    public void ToggleHandCollision(bool val)
    {
        HandCol.ProcessMode = val ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }

    public void Vibrate(double power, double duration, double delay = 0)
    {
        TriggerHapticPulse("haptic", 150, power, duration, delay);
    }

}