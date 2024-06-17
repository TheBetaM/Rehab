using System;
using Godot;
using Godot.Collections;
namespace Rehab;
[Tool]
public partial class ActorInstance : Marker3D
{
    [Export] PackedScene Prefab;
    [Export] Array<NodePath> LinkInstance;
    [Export] Array<NodePath> LinkPath;
    [Export] Array<NodePath> LinkPoint;
    [Export] Array<int> RegAngle;
    [Export] Array<float> RegFloat;
    [Export] Array<int> RegInt;
    [Export] IFlags Flags;
    [Export] bool OutlineCrate;
    [Export] int RefList = -1;
    public Agent Actor;

    public override async void _Ready()
    {
        if (Prefab == null) return;
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
        Actor.HasFlags = true;
        Actor.Flags = Flags;
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

    [Flags]
    public enum IFlags : uint
    {
        Inactive = 1 << 0,
        Collidable = 1 << 1,
        Visible = 1 << 2,
        Shadow = 1 << 3,
        Flag4 = 1 << 4,
        Flag5 = 1 << 5,
        HasPersistentState = 1 << 6,
        Flag7 = 1 << 7,
        Flag8 = 1 << 8,
        Harmful = 1 << 9,
        SolidToBodyslam = 1 << 10,
        SolidToSlide = 1 << 11,
        SolidToSpin = 1 << 12,
        SolidToTwinSlam = 1 << 13,
        SolidToTwinThrow = 1 << 14,
        Targetable = 1 << 15,
        Flag16 = 1 << 16,
        ReflectRegularProjectiles = 1 << 17,
        ScriptFlag18 = 1 << 18,
        ScriptFlag19 = 1 << 19,
        ScriptFlag20 = 1 << 20,
        ScriptFlag21 = 1 << 21,
        ScriptFlag22 = 1 << 22,
        ScriptFlag23 = 1 << 23,
        ScriptFlag24 = 1 << 24,
        ScriptFlag25 = 1 << 25,
        ScriptFlag26 = 1 << 26,
        ScriptFlag27 = 1 << 27,
        ScriptFlag28 = 1 << 28,
        ScriptFlag29 = 1 << 29,
        ScriptFlag30 = 1 << 30,
        ScriptFlag31 = (uint)1 << 31,
        
    }
}