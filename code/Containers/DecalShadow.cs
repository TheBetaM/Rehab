using Godot;
namespace Rehab;
public partial class DecalShadow : Decal
{
    [Export]
    public int bone = 0;
    [Export]
    public int model_id = 0;
    public Skeleton3D skeleton;
    public Node3D parentNode;

    public override void _Ready()
    {
        parentNode = GetParentNode3D();
        skeleton = GetParent().GetParent().GetNode("Models").GetChild(model_id).GetNode("RigidBody").GetNode<Skeleton3D>("Armature");
        if (bone == 255) bone = 0;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Visible || !parentNode.Visible) return;
        //Transform = skeleton.GetBonePose(bone);
    }
}