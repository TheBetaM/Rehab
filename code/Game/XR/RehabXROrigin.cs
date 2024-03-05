using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabXROrigin : XROrigin3D
{
    
    public MeshInstance3D FE_XR_Mesh;
    public XRController3D XR_HandL;
    public XRController3D XR_HandR;
    public Sprite3D XR_Cursor;

    public override void _Ready()
    {
        FE_XR_Mesh = GetNode<MeshInstance3D>("FE_Display");
        XR_Cursor = FE_XR_Mesh.GetNode<Sprite3D>("Cursor");
        XR_HandL = GetNode<XRController3D>("HandL");
        XR_HandR = GetNode<XRController3D>("HandR");
        FE_XR_Mesh.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (!RehabScene.Root.XR_Enabled) return;
        FE_XR_Mesh.MaterialOverride.Set("albedo_texture", RehabScene.Root.FE_XR_Viewport.GetTexture());
        var spaceState = RehabScene.Root.GetWorld3D().DirectSpaceState;
        Vector3 pos = XR_HandL.GlobalPosition + (-XR_HandL.Transform.Basis.Z * 100f);
        var query = PhysicsRayQueryParameters3D.Create(XR_HandL.GlobalPosition, pos);
        var result = spaceState.IntersectRay(query);
        if (result.ContainsKey("collider"))
        {
            var rPos = (Vector3)result["position"];
            XR_Cursor.GlobalPosition = new Vector3(rPos.X, rPos.Y, XR_Cursor.GlobalPosition.Z);
            if (XR_Cursor.Scale.X > 0.7f)
            {
                XR_Cursor.Scale = new Vector3(0.3f, 0.3f, 0.3f);
            }
            var mousePos = new InputEventMouseMotion();
            mousePos.Position = new Vector2(XR_Cursor.Position.X * 160f + 640f, XR_Cursor.Position.Z * 80f + 360f);
            _Input(mousePos);
        }
        else
        {
            XR_Cursor.Scale = new Vector3(0.75f, 0.75f, 0.75f);
        }
    }

    public override void _Input(InputEvent input)
    {
        if (!RehabScene.Root.XR_Enabled) return;
        RehabScene.Root.FE_XR_Viewport.PushInput(input);
        if (input is InputEventJoypadButton b)
        {
            if (b.ButtonIndex == JoyButton.A)
            {
                if (b.Pressed)
                {
                    XR_Cursor.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else
                {
                    XR_Cursor.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                }
            }
        }
        else if (input is InputEventMouseButton m)
        {
            if (m.ButtonIndex == MouseButton.Left)
            {
                if (m.Pressed)
                {
                    XR_Cursor.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else
                {
                    XR_Cursor.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                }
            }
        }
    }
}