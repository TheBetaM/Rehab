using Godot;
namespace Rehab;
public partial class TriggerVolume : Area3D
{
    [Export]
    public Godot.Collections.Dictionary<int, bool> Messages;
    [Export]
    public Godot.Collections.Array<NodePath> InstanceRefs;
    [Export]
    public float SomeFloat;
    [Export]
    public int SectionHead;
    [Export]
    public Godot.Collections.Array<bool> Mask;

    public override void _Ready()
    {
        BodyEntered += OnTrigger;
    }

    void OnTrigger(Node3D body)
    {
        
    }

}