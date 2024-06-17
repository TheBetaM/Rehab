using Godot;
using System.Collections.Generic;
using System.Linq;
namespace Rehab;
public partial class Agent : Node3D
{

    [Export] public int UnkTypeValue;
    [Export] public int JointIDCount;
    [Export] public int ExitPointCount;
    [Export] public Godot.Collections.Dictionary Messages;
    [Export] public Godot.Collections.Array Scripts;
    [Export] public Godot.Collections.Array<Godot.Collections.Dictionary> ModelActions;
    [Export] public Godot.Collections.Array<Resource> Sounds;
    [Export] public Godot.Collections.Array<PackedScene> SubActorsScenes;
    public List<Agent> SubActors = new();
    public List<Node3D> SubModels = new();
    public AudioStreamPlayer3D AudioSource;
    public ChunkScene ParentScene;
    public int ActiveModel;
    public int ActiveAnim = -1;
    public Skeleton3D ActiveSkeleton;
    public List<int> JointsConst = new(); //Joint-ID ones
    public List<Node3D> ExitPoints = new();
    public Dictionary<int, List<CollisionShape3D>> ColShapes = new();
    bool FirstSetup = true;
    public Area3D NonSolidCollisionArea;

    // Instance data
    [Export] public Godot.Collections.Array<Node3D> LinkInstance;
    [Export] public Godot.Collections.Array<Path3D> LinkPath;
    [Export] public Godot.Collections.Array<Marker3D> LinkPoint;
    [Export] public Godot.Collections.Array<int> RegAngle;
    [Export] public Godot.Collections.Array<float> RegFloat;
    [Export] public Godot.Collections.Array<int> RegInt;
    [Export] public bool HasFlags = false;
    [Export] public ActorInstance.IFlags Flags;
    [Export] public bool OutlineCrate;
    [Export] public int RefList;

    string[] ShadowPaths = [
        "res://assets/textures/shadow/clin.png",
        "res://assets/textures/shadow/cube.png",
        "res://assets/textures/shadow/rcub.png",
        "res://assets/textures/shadow/octo.png",
    ];

    public override void _Ready()
    {
        AudioSource = new AudioStreamPlayer3D();
        AddChild(AudioSource);
        AudioSource.Bus = "SFX";
        var ShadowHolderNode = new Node3D();
        ShadowHolderNode.Name = "Shadows";
        AddChild(ShadowHolderNode);
        
        if (GetNodeOrNull("Children") != null)
        {
            foreach (var i in GetNode("Children").GetChildren())
                if (i is Agent a)
                    SubActors.Add(a);
        }
        if (GetNodeOrNull("Models") != null)
        {
            foreach (var i in GetNode("Models").GetChildren())
            {
                var body = (RigidBody3D)i.GetChild(0);
                foreach (var a in body.GetChildren())
                {
                    if (a is CollisionShape3D col)
                    {
                        if (SubModels.Count != 0 || this is AgentCharacter)
                        {
                            col.Disabled = true;
                            col.ProcessMode = ProcessModeEnum.Disabled;
                        }
                        if (ColShapes.ContainsKey(SubModels.Count))
                            ColShapes[SubModels.Count].Add(col);
                        else
                            ColShapes.Add(SubModels.Count, new() {col});
                        a.Reparent(this);
                    }
                }
                SubModels.Add((Node3D)i);
            }
        }
        for (int i = 0; i < JointIDCount; i++)
            JointsConst.Add(-1);
        for (int i = 0; i < ExitPointCount; i++)
            ExitPoints.Add(null);
        if (SubModels.Count > 0)
            ActiveSkeleton = (Skeleton3D)SubModels[ActiveModel].GetChild(0).GetChild(0);
        
        NonSolidCollisionArea = new Area3D();
        NonSolidCollisionArea.Name = "NonSolid";
        NonSolidCollisionArea.CollisionMask = 0;
        NonSolidCollisionArea.CollisionLayer = 0;
        NonSolidCollisionArea.Connect("body_entered", Callable.From<Node3D>(OnNonSolidCollisionEnter));
        var ColShape = new CollisionShape3D();
        if (ColShapes.ContainsKey(ActiveModel))
        {
            ColShape.Shape = ColShapes[ActiveModel][0].Shape;
            ColShape.Transform = ColShapes[ActiveModel][0].Transform;
        }
        else
        {
            ColShape.Shape = new BoxShape3D();
            ColShape.Position = new Vector3(0f, 0.5f, 0f);
        }
        NonSolidCollisionArea.AddChild(ColShape);
        NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Disabled;
        AddChild(NonSolidCollisionArea);
        
        var parent = GetParent();
        while (ParentScene == null && parent != null)
        {
            if (parent is ChunkScene chunk)
                ParentScene = chunk;
            else
                parent = parent.GetParent();
        }
        if (ParentScene != null)
        {
            UpdateLayers(ParentScene.ChunkLayer);
        }
        UpdateActiveModel();
        UpdateFlags();
    }

    public void DoAnimation(int slot, bool loop)
    {
        if (ModelActions == null) return;
        if (slot >= ModelActions.Count) return;
        if (ModelActions[slot] == null) return;
        var pair = ModelActions[slot];
        int ogi = (int)pair.Keys.First();
        string animName = (string)pair.Values.First();
        if (ActiveAnim == slot && ogi == ActiveModel) return;
        if (ogi != ActiveModel)
        {
            foreach (var i in SubModels)
            {
                i.Visible = false;
                i.ProcessMode = ProcessModeEnum.Disabled;
            }
            var animPlayer = (AnimationPlayer)SubModels[ogi].GetNode("AnimationPlayer");
            if (animPlayer.HasAnimation("RESET"))
            {
                animPlayer.Play("RESET");
                var oMode = animPlayer.CallbackModeProcess;
                animPlayer.CallbackModeProcess = AnimationPlayer.AnimationCallbackModeProcess.Manual; 
                animPlayer.Advance(0.1);
                animPlayer.Stop();
                animPlayer.CallbackModeProcess = oMode;
                animPlayer.Stop();
            }
            ActiveAnim = -1;
            SubModels[ogi].Visible = true;
            SubModels[ogi].ProcessMode = ProcessModeEnum.Inherit;
            if (ColShapes.ContainsKey(ogi))
            {
                if (this is not AgentCharacter && (!HasFlags || Flags.HasFlag(ActorInstance.IFlags.Collidable)))
                {
                    foreach (var i in ColShapes[ogi])
                    {
                        i.ProcessMode = ProcessModeEnum.Inherit;
                        i.Disabled = false;
                    }
                }
                else
                {
                    foreach (var i in ColShapes[ogi])
                    {
                        i.ProcessMode = ProcessModeEnum.Disabled;
                        i.Disabled = true;
                    }
                }
            }
            if (ColShapes.ContainsKey(ActiveModel))
            {
                foreach (var i in ColShapes[ActiveModel])
                {
                    i.Disabled = true;
                    i.ProcessMode = ProcessModeEnum.Disabled;
                }
            }
            ActiveModel = ogi;
            ActiveSkeleton = (Skeleton3D)SubModels[ogi].GetChild(0).GetChild(0);
            UpdateActiveModel();
        }
        if (animName != null && animName != "")
        {
            var animPlayer = (AnimationPlayer)SubModels[ogi].GetNode("AnimationPlayer");
            //animPlayer.PlaybackDefaultBlendTime = 0.25f;
            if (loop)
                animPlayer.GetAnimation(animName).LoopMode = Animation.LoopModeEnum.Linear;
            else
                animPlayer.GetAnimation(animName).LoopMode = Animation.LoopModeEnum.None;
            if (ActiveAnim == -1)
                animPlayer.Play(animName);
            else
                animPlayer.Play(animName, 0.25);
            ActiveAnim = slot;
        }
    }

    public void DoSound(int slot, float pitch, float volume, bool loop = false)
    {
        if (slot >= Sounds.Count || slot < 0)
            return;
        if (Sounds[slot] != null)
        {
            //AudioSource.Reparent(SubModels[ActiveModel].GetChild(0));
            AudioSource.VolumeDb = volume;
            AudioSource.Position = Vector3.Zero;
            AudioSource.ProcessMode = ProcessModeEnum.Pausable;
            AudioSource.Stream = (AudioStream)Sounds[slot];
            if (loop)
            {
                var stream = (AudioStreamWav)Sounds[slot];
                stream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
                if (stream.Stereo)
                {
                    stream.LoopBegin = 16;
                    stream.LoopEnd = stream.Data.Length / 4;
                }
                else
                {
                    stream.LoopEnd = 32;
                    stream.LoopEnd = stream.Data.Length / 2;
                }
            }
            AudioSource.PitchScale = pitch;
            AudioSource.Play();
        }
    }

    public void DoSoundPath(string path, float pitch, float volume, bool loop = false)
    {
        if (!ResourceLoader.Exists(path))
            return;
        //AudioSource.Reparent(SubModels[ActiveModel].GetChild(0));
        AudioSource.VolumeDb = volume;
        AudioSource.Position = Vector3.Zero;
        AudioSource.ProcessMode = ProcessModeEnum.Pausable;
        AudioSource.Stream = (AudioStream)ResourceLoader.Load(path);
        AudioSource.PitchScale = pitch;
        if (loop)
        {
            var stream = (AudioStreamWav)AudioSource.Stream;
            stream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
            if (stream.Stereo)
            {
                stream.LoopBegin = 16;
                stream.LoopEnd = stream.Data.Length / 4;
            }
            else
            {
                stream.LoopEnd = 32;
                stream.LoopEnd = stream.Data.Length / 2;
            }
        }
        AudioSource.Play();
    }

    public void DoSoundStream(AudioStream s, float pitch, float volume, bool loop = false)
    {
        //AudioSource.Reparent(SubModels[ActiveModel].GetChild(0));
        AudioSource.VolumeDb = volume;
        AudioSource.Position = Vector3.Zero;
        AudioSource.ProcessMode = ProcessModeEnum.Pausable;
        AudioSource.Stream = s;
        AudioSource.PitchScale = pitch;
        if (loop)
        {
            var stream = (AudioStreamWav)AudioSource.Stream;
            stream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
            if (stream.Stereo)
            {
                stream.LoopBegin = 16;
                stream.LoopEnd = stream.Data.Length / 4;
            }
            else
            {
                stream.LoopEnd = 32;
                stream.LoopEnd = stream.Data.Length / 2;
            }
        }
        AudioSource.Play();
    }

    public void UpdateLayers(int layer)
    {
        //Updating collision and light layers in child nodes
        var agentcol = Call("get_collision_layer_value", 1);
        if (!FirstSetup || (bool)agentcol == true)
        {
            for (int a = 1; a < 15; a++)
            {
                Call("set_collision_mask_value", a, false);
                Call("set_collision_layer_value", a, false);
            }
            Call("set_collision_mask_value", layer, true);
            if (this is AgentCharacter)
            {
                Call("set_collision_layer_value", layer, true);
            }
            if (HasFlags)
            {
                if (Flags.HasFlag(ActorInstance.IFlags.Collidable))
                {
                    Call("set_collision_layer_value", 9, true);
                    Call("set_collision_mask_value", 9, true);
                    Call("set_collision_mask_value", 10, true);
                    Call("set_collision_mask_value", 11, true);
                    Call("set_collision_mask_value", 12, true);
                    Call("set_collision_mask_value", 13, true);
                    Call("set_collision_mask_value", 14, true);
                }
                if (Flags.HasFlag(ActorInstance.IFlags.SolidToBodyslam))
                {
                    Call("set_collision_layer_value", 10, true);
                }
                if (Flags.HasFlag(ActorInstance.IFlags.SolidToSlide))
                {
                    Call("set_collision_layer_value", 11, true);
                }
                if (Flags.HasFlag(ActorInstance.IFlags.SolidToSpin))
                {
                    Call("set_collision_layer_value", 12, true);
                }
                if (Flags.HasFlag(ActorInstance.IFlags.SolidToTwinSlam))
                {
                    Call("set_collision_layer_value", 13, true);
                }
                if (Flags.HasFlag(ActorInstance.IFlags.SolidToTwinThrow))
                {
                    Call("set_collision_layer_value", 14, true);
                }
            }
        }
        bool LimitDrawDistance = false;
        if (OS.GetName() == "Android")
        {
            if (this is AgentPickup || this is AgentCrate || this is AgentCreature)
            {
                LimitDrawDistance = true;
            }
        }
	    UpdateLayersNested(this, layer, LimitDrawDistance);
	    FirstSetup = false;
        if (HasFlags)
        {
            for (int a = 9; a < 15; a++)
            {
                NonSolidCollisionArea.SetCollisionLayerValue(a, false);
                NonSolidCollisionArea.SetCollisionMaskValue(a, true);
            }
            if (Flags.HasFlag(ActorInstance.IFlags.Collidable))
            {
                NonSolidCollisionArea.SetCollisionLayerValue(9, true);
                if (!Flags.HasFlag(ActorInstance.IFlags.SolidToBodyslam))
                {
                    NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Inherit;
                    NonSolidCollisionArea.SetCollisionLayerValue(10, true);
                }
                if (!Flags.HasFlag(ActorInstance.IFlags.SolidToSlide))
                {
                    NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Inherit;
                    NonSolidCollisionArea.SetCollisionLayerValue(11, true);
                }
                if (!Flags.HasFlag(ActorInstance.IFlags.SolidToSpin))
                {
                    NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Inherit;
                    NonSolidCollisionArea.SetCollisionLayerValue(12, true);
                }
                if (!Flags.HasFlag(ActorInstance.IFlags.SolidToTwinSlam))
                {
                    NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Inherit;
                    NonSolidCollisionArea.SetCollisionLayerValue(13, true);
                }
                if (!Flags.HasFlag(ActorInstance.IFlags.SolidToTwinThrow))
                {
                    NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Inherit;
                    NonSolidCollisionArea.SetCollisionLayerValue(14, true);
                }
            }
            else
            {
                NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Disabled;
            }
        }
        else
        {
            NonSolidCollisionArea.ProcessMode = ProcessModeEnum.Disabled;
        }
    }

    void UpdateLayersNested(Node i, int layer, bool LimitDrawDistance)
    {
        if (i is VisualInstance3D vis)
        {
            for (int a = 1; a < 9; a++)
            {
                vis.SetLayerMaskValue(a, false);
            }
            vis.SetLayerMaskValue(layer, true);
            if (i is Light3D light)
            {
                var mask = (int)light.LightCullMask | (1 << (layer - 1));
                light.LightCullMask = (uint)mask;
            }
            if (vis is MeshInstance3D geom && LimitDrawDistance)
            {
                geom.VisibilityRangeEnd = 60f;
            }
        }
        else if (i is CollisionObject3D col)
        {
            if (!FirstSetup || col.GetCollisionLayerValue(1) != false)
            {
                for (int a = 1; a < 14; a++)
                {
                    col.SetCollisionMaskValue(a, false);
                    col.SetCollisionLayerValue(a, false);
                }
                col.SetCollisionMaskValue(layer, true);
                if (this is AgentCharacter)
                {
                    col.SetCollisionLayerValue(layer, true);
                }
                if (HasFlags)
                {
                    if (Flags.HasFlag(ActorInstance.IFlags.Collidable))
                    {
                        col.SetCollisionLayerValue(9, true);
                        col.SetCollisionMaskValue(9, false);
                        col.SetCollisionMaskValue(10, false);
                        col.SetCollisionMaskValue(11, false);
                        col.SetCollisionMaskValue(12, false);
                        col.SetCollisionMaskValue(13, false);
                    }
                    if (Flags.HasFlag(ActorInstance.IFlags.SolidToBodyslam))
                    {
                        col.SetCollisionLayerValue(10, true);
                    }
                    if (Flags.HasFlag(ActorInstance.IFlags.SolidToSlide))
                    {
                        col.SetCollisionLayerValue(11, true);
                    }
                    if (Flags.HasFlag(ActorInstance.IFlags.SolidToSpin))
                    {
                        col.SetCollisionLayerValue(12, true);
                    }
                    if (Flags.HasFlag(ActorInstance.IFlags.SolidToTwinSlam))
                    {
                        col.SetCollisionLayerValue(13, true);
                    }
                    if (Flags.HasFlag(ActorInstance.IFlags.SolidToTwinThrow))
                    {
                        col.SetCollisionLayerValue(14, true);
                    }
                }
            }
        }
        foreach (var id in i.GetChildren())
            UpdateLayersNested(id, layer, LimitDrawDistance);
    }

    void UpdateLayersChunk(object a, System.EventArgs e)
    {
        UpdateLayers(ParentScene.ChunkLayer);
    }

    public void UpdateActiveModel()
    {
        for (int i = 0; i < JointIDCount; i++)
            JointsConst[i] = -1;
        for (int i = 0; i < JointIDCount; i++)
        {
            var JointID = SubModels[ActiveModel].FindChild($"JointID-{i}", true);
            if (JointID != null)
            {
                var attach = (BoneAttachment3D)JointID.GetParent();
                JointsConst[i] = attach.BoneIdx;
                var skeleton = attach.GetParent();
                ActiveSkeleton = (Skeleton3D)skeleton;
            }
        }
        for (int i = 0; i < ExitPointCount; i++)
            ExitPoints[i] = (Node3D)SubModels[ActiveModel].FindChild($"ExitPoint{i}", true);
    }

    public virtual void OnChunkEnter(object a, System.EventArgs e)
    {

    }

    public void CreateShadow(int type, Vector2 dsize, int boneAttach)
    {
        if (SubModels.Count == 0) return;
        var shadow = new Decal();
        shadow.Size = new Vector3(dsize.X, 10f, dsize.Y);
        shadow.TextureAlbedo = (Texture2D)ResourceLoader.Load(ShadowPaths[type]);
        shadow.UpperFade = 0f;
        shadow.LowerFade = 0.5f;
        shadow.DistanceFadeEnabled = true;
        shadow.DistanceFadeBegin = 40;
        shadow.Layers = 1;
        shadow.Modulate = new Color(1f, 1f, 1f, 0.5f);
        shadow.SortingOffset = -100f;
        //SubModels[ActiveModel].GetChild(0).AddChild(shad);
        GetNode("Shadows").AddChild(shadow);
        shadow.Position = new Vector3(shadow.Position.X, -4.99f, shadow.Position.Z);
    }

    void UpdateFlags()
    {
        if (!HasFlags) return;
        if (Flags.HasFlag(ActorInstance.IFlags.Inactive))
        {
            ProcessMode = ProcessModeEnum.Disabled;
            Visible = false;
            return;
        }
        Visible = Flags.HasFlag(ActorInstance.IFlags.Visible);
        GetNode<Node3D>("Shadows").Visible = Flags.HasFlag(ActorInstance.IFlags.Shadow);
        GetNode<Node3D>("Shadows").ProcessMode = Flags.HasFlag(ActorInstance.IFlags.Shadow) ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
        if (ColShapes.ContainsKey(ActiveModel))
        {
            if (!Flags.HasFlag(ActorInstance.IFlags.Collidable))
            {
                foreach (var i in ColShapes[ActiveModel])
                {
                    i.ProcessMode = ProcessModeEnum.Disabled;
                    i.Disabled = true;
                }
            }
            else if (this is not AgentCharacter)
            {
                foreach (var i in ColShapes[ActiveModel])
                {
                    i.ProcessMode = ProcessModeEnum.Inherit;
                    i.Disabled = false;
                }
            }
        }
    }

    public virtual void OnNonSolidCollisionEnter(Node3D body)
    {

    }

    public virtual void OnMessage(int id)
    {

    }
}