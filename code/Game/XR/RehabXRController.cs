using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabXRController : XRController3D
{
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
            var MEvent = new InputEventScreenTouch();
            MEvent.Position = RehabScene.Root.FE_XR_Viewport.GetMousePosition() * 2f;
            MEvent.Pressed = true;
            Input.ParseInputEvent(MEvent);
            break;
            case "ax_button":
            var XEvent = new InputEventJoypadButton();
            XEvent.ButtonIndex = JoyButton.A;
            XEvent.Pressed = true;
            Input.ParseInputEvent(XEvent);
            break;
            case "by_button":
            var YEvent = new InputEventJoypadButton();
            YEvent.ButtonIndex = JoyButton.Y;
            YEvent.Pressed = true;
            Input.ParseInputEvent(YEvent);
            break;
            case "menu_button":
            var SEvent = new InputEventJoypadButton();
            SEvent.ButtonIndex = JoyButton.Start;
            SEvent.Pressed = true;
            Input.ParseInputEvent(SEvent);
            break;
        }
    }

    void OnButtonRelease(string name)
    {
        switch (name)
        {
            default: break;
            case "trigger_click":
            var MEvent = new InputEventScreenTouch();
            MEvent.Pressed = false;
            Input.ParseInputEvent(MEvent);
            break;
            case "ax_button":
            var XEvent = new InputEventJoypadButton();
            XEvent.ButtonIndex = JoyButton.A;
            XEvent.Pressed = false;
            Input.ParseInputEvent(XEvent);
            break;
            case "by_button":
            var YEvent = new InputEventJoypadButton();
            YEvent.ButtonIndex = JoyButton.Y;
            YEvent.Pressed = false;
            Input.ParseInputEvent(YEvent);
            break;
            case "menu_button":
            var SEvent = new InputEventJoypadButton();
            SEvent.ButtonIndex = JoyButton.Start;
            SEvent.Pressed = false;
            Input.ParseInputEvent(SEvent);
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
            XEvent.Axis = JoyAxis.LeftX;
            XEvent.AxisValue = pos.X;
            var YEvent = new InputEventJoypadMotion();
            YEvent.Axis = JoyAxis.LeftY;
            YEvent.AxisValue = -pos.Y;
            Input.ParseInputEvent(XEvent);
            Input.ParseInputEvent(YEvent);
            break;
            case "secondary":
            var RXEvent = new InputEventJoypadMotion();
            RXEvent.Axis = JoyAxis.RightX;
            RXEvent.AxisValue = pos.X;
            var RYEvent = new InputEventJoypadMotion();
            RYEvent.Axis = JoyAxis.RightY;
            RYEvent.AxisValue = -pos.Y;
            Input.ParseInputEvent(RXEvent);
            Input.ParseInputEvent(RYEvent);
            break;
        }
    }
}