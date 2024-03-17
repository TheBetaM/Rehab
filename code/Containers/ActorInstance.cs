using Godot;
using Godot.Collections;
namespace Rehab;
[Tool]
public partial class ActorInstance : Marker3D
{
    [Export] PackedScene Prefab;
    [Export] bool OutlineCrate;
    [Export] int RefList = -1;
    [Export] Array<NodePath> LinkInstance;
    [Export] Array<NodePath> LinkPath;
    [Export] Array<NodePath> LinkPoint;
    [Export] Array<int> RegAngle;
    [Export] Array<float> RegFloat;
    [Export] Array<int> RegInt;
    public Agent Actor;

    public override async void _Ready()
    {
        //Preventing load stutter and prioritizing current scene
        int delay = GetIndex() % 30;
        delay++;

        var parent = GetParent();
        ChunkScene ParentScene = null;
        while (ParentScene == null && parent != null)
        {
            if (parent is ChunkScene chunk)
                ParentScene = chunk;
            else
                parent = parent.GetParent();
        }
        if (ParentScene != null && !ParentScene.ActiveScene)
        {
            delay += 31;
        }

        for (int i = 0; i < delay; i++)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        
        var act = Prefab.Instantiate();
        if (Engine.IsEditorHint())
            act.Set("metadata/_edit_lock_", true);
        if (act is not Agent a)
        {
            AddChild(act);
            return;
        } 
        Actor = a;
        Actor.OutlineCrate = OutlineCrate;
        Actor.RefList = RefList;
        if (RegAngle != null)
        {
            Actor.RegAngle = RegAngle;
        }
        if (RegFloat != null)
        {
            Actor.RegFloat = RegFloat;
        }
        if (RegInt != null)
        {
            Actor.RegInt = RegInt;
        }
        if (LinkInstance != null)
        {
            Actor.LinkInstance = new();
            foreach (var i in LinkInstance)
            {
                if (GetNodeOrNull(i) != null)
                    Actor.LinkInstance.Add((Node3D)GetNodeOrNull(i));
            }
        }
        if (LinkPath != null)
        {
            Actor.LinkPath = new();
            foreach (var i in LinkPath)
            {
                if (GetNodeOrNull(i) != null)
                    Actor.LinkPath.Add((Path3D)GetNodeOrNull(i));
            }
        }
        if (LinkPoint != null)
        {
            Actor.LinkPoint = new();
            foreach (var i in LinkPoint)
            {
                if (GetNodeOrNull(i) != null)
                    Actor.LinkPoint.Add((Marker3D)GetNodeOrNull(i));
            }
            
        }
        AddChild(Actor);
        
    }
}