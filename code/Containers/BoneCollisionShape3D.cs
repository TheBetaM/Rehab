using Godot;
namespace Rehab;
public partial class BoneCollisionShape3D : CollisionShape3D{
    //This is needed because physics objects can only have collision shapes as direct children
    [Export]
    public int bone =  0;
    public Skeleton3D skeleton;
    public bool noParent = false;

    public override void _Ready()
    {
        skeleton = GetParent().GetNode<Skeleton3D>("Armature");
        if (bone == 255) bone = 0;
        UpdateBone();
    }

    async void UpdateBone()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Transform = skeleton.GetBonePose(bone);
    }

    /*
    public override void _PhysicsProcess(double delta)
    {
        Transform = skeleton.GetBonePose(bone);
    }
    */
}