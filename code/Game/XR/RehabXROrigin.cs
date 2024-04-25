using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabXROrigin : XROrigin3D
{
    
    public MeshInstance3D FE_XR_Mesh;
    public XRCamera3D XR_Camera;
    public RehabXRController XR_HandL;
    public RehabXRController XR_HandR;
    public Sprite3D XR_CursorL;
    public Sprite3D XR_CursorR;
    public bool FE_XR_Active = false;
    public Node3D FE_XR_Pivot;
    double TurnCooldown = 0f;
    StaticBody3D FE_Collider;

    public override void _Ready()
    {
        XR_Camera = GetNode<XRCamera3D>("XRCamera3D");
        FE_XR_Pivot = GetNode<Node3D>("FE_Pivot");
        FE_XR_Mesh =FE_XR_Pivot.GetNode<MeshInstance3D>("FE_Display");
        XR_CursorL = FE_XR_Mesh.GetNode<Sprite3D>("CursorL");
        XR_CursorR = FE_XR_Mesh.GetNode<Sprite3D>("CursorR");
        XR_HandL = GetNode<RehabXRController>("HandL");
        XR_HandR = GetNode<RehabXRController>("HandR");
        FE_Collider = GetNode<StaticBody3D>("FE_Pivot/FE_Display/FE_Collider");
        XR_HandL.Origin = this;
        XR_HandR.Origin = this;
        XR_HandR.ActiveCursor = true;
        XR_HandL.Visible = false;
        XR_HandR.Visible = false;
        FE_XR_Mesh.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (!RehabScene.Root.XR_Enabled) return;

        UpdateCamera(delta);
        UpdateFE(delta);
    }

    void UpdateFE(double delta)
    {
        if (!FE_XR_Active) return;
        FE_XR_Mesh.MaterialOverride.Set("albedo_texture", RehabScene.Root.FE_XR_Viewport.GetTexture());
        var spaceState = RehabScene.Root.GetWorld3D().DirectSpaceState;
        Vector3 pos = XR_HandL.GlobalPosition + (-XR_HandL.GlobalTransform.Basis.Z * 400f);
        var query = PhysicsRayQueryParameters3D.Create(XR_HandL.GlobalPosition, pos);
        var result = spaceState.IntersectRay(query);
        if (result.ContainsKey("collider"))
        {
            XR_CursorL.Visible = true;
            var rPos = (Vector3)result["position"];
            XR_CursorL.GlobalPosition = rPos;
            XR_CursorL.Position = new Vector3(XR_CursorL.Position.X, 0.1f, XR_CursorL.Position.Z);
            if (XR_HandL.ActiveCursor)
            {
                var mousePos = new InputEventMouseMotion();
                mousePos.Position = new Vector2(XR_CursorL.Position.X * 80f + 640f, XR_CursorL.Position.Z * 80f + 360f);
                _Input(mousePos);
            }
        }
        else
        {
            XR_CursorL.Visible = false;
        }
        spaceState = RehabScene.Root.GetWorld3D().DirectSpaceState;
        pos = XR_HandR.GlobalPosition + (-XR_HandR.GlobalTransform.Basis.Z * 400f);
        query = PhysicsRayQueryParameters3D.Create(XR_HandR.GlobalPosition, pos);
        result = spaceState.IntersectRay(query);
        if (result.ContainsKey("collider"))
        {
            XR_CursorR.Visible = true;
            var rPos = (Vector3)result["position"];
            XR_CursorR.GlobalPosition = rPos;
            XR_CursorR.Position = new Vector3(XR_CursorR.Position.X, 0.1f, XR_CursorR.Position.Z);
            if (XR_HandR.ActiveCursor)
            {
                var mousePos = new InputEventMouseMotion();
                mousePos.Position = new Vector2(XR_CursorR.Position.X * 80f + 640f, XR_CursorR.Position.Z * 80f + 360f);
                _Input(mousePos);
            }
        }
        else
        {
            XR_CursorR.Visible = false;
        }
    }
    void UpdateCamera(double delta)
    {
        if (FE_XR_Active) return;
        float camdirX = 0f;

        if (Input.IsActionPressed(RehabGame.Pad_RStick_Left))
        {
            if (!RehabGame.InvertCameraX)
                camdirX -= Input.GetActionStrength(RehabGame.Pad_RStick_Left);
            else
                camdirX += Input.GetActionStrength(RehabGame.Pad_RStick_Left);
        }
        if (Input.IsActionPressed(RehabGame.Pad_RStick_Right))
        {
            if (!RehabGame.InvertCameraX)
                camdirX += Input.GetActionStrength(RehabGame.Pad_RStick_Right);
            else
                camdirX -= Input.GetActionStrength(RehabGame.Pad_RStick_Right);
        }

        if (TurnCooldown > 0f) TurnCooldown -= delta;
        if (Math.Abs(camdirX) > 0.45f && TurnCooldown <= 0f)
        {
            var camPos = XR_Camera.GlobalPosition;
            if (camdirX > 0f)
                Transform = Transform.RotatedLocal(Vector3.Up, -45f);
            else
                Transform = Transform.RotatedLocal(Vector3.Up, 45f);
            XR_Camera.GlobalPosition = camPos;
            TurnCooldown = 0.5f;
        }
    }

    public override void _Input(InputEvent input)
    {
        if (!RehabScene.Root.XR_Enabled) return;
        RehabScene.Root.FE_XR_Viewport.PushInput(input);
        if (!FE_XR_Active) return;
        if (input is InputEventMouseButton m)
        {
            if (m.ButtonIndex == MouseButton.Left)
            {
                if (m.Pressed)
                {
                    if (XR_HandR.ActiveCursor)
                        XR_CursorR.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                    else
                        XR_CursorL.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else
                {
                    if (XR_HandR.ActiveCursor)
                        XR_CursorR.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                    else
                        XR_CursorL.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                }
            }
        }
    }

    public void FE_Active()
    {
        FE_XR_Active = true;
        XR_CursorL.Visible = true;
        XR_CursorR.Visible = true;
        FE_XR_Pivot.Transform = XR_Camera.Transform;
        FE_Collider.ProcessMode = ProcessModeEnum.Inherit;
        XR_HandL.FE_Active();
        XR_HandR.FE_Active();
    }

    public void FE_Inactive()
    {
        FE_Collider.ProcessMode = ProcessModeEnum.Disabled;
        FE_XR_Active = false;
        XR_CursorL.Visible = false;
        XR_CursorR.Visible = false;
        XR_HandL.FE_Inactive();
        XR_HandR.FE_Inactive();
    }

    public void ResetOrientation()
    {
        FE_XR_Pivot.Transform = XR_Camera.Transform;
    }

    public void ClearHands()
    {
        XR_HandL.ClearHand();
        XR_HandR.ClearHand();
    }

    public void ToggleHands(bool val)
    {
        XR_HandL.Visible = val;
        XR_HandR.Visible = val;
    }
}