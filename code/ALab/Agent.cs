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

    // Instance data
    [Export] public bool OutlineCrate;
    [Export] public int RefList;
    [Export] public Godot.Collections.Array<Marker3D> LinkInstance;
    [Export] public Godot.Collections.Array<Path3D> LinkPath;
    [Export] public Godot.Collections.Array<Marker3D> LinkPoint;
    [Export] public Godot.Collections.Array<int> RegAngle;
    [Export] public Godot.Collections.Array<float> RegFloat;
    [Export] public Godot.Collections.Array<int> RegInt;

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
                foreach (var a in i.GetChild(0).GetChildren())
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
            animPlayer.Play("RESET");
            var oMode = animPlayer.CallbackModeProcess;
            animPlayer.CallbackModeProcess = AnimationPlayer.AnimationCallbackModeProcess.Manual; 
            animPlayer.Advance(0.1);
            animPlayer.Stop();
            animPlayer.CallbackModeProcess = oMode;
            animPlayer.Stop();
            ActiveAnim = -1;
            SubModels[ogi].Visible = true;
            SubModels[ogi].ProcessMode = ProcessModeEnum.Inherit;
            if (this is not AgentCharacter && ColShapes.ContainsKey(ogi))
            {
                foreach (var i in ColShapes[ogi])
                {
                    i.ProcessMode = ProcessModeEnum.Inherit;
                    i.Disabled = false;
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

    public void DoSound(int slot, float pitch, float volume)
    {
        if (slot >= Sounds.Count)
            return;
        if (Sounds[slot] != null)
        {
            //AudioSource.Reparent(SubModels[ActiveModel].GetChild(0));
            AudioSource.VolumeDb = volume;
            AudioSource.Position = Vector3.Zero;
            AudioSource.ProcessMode = ProcessModeEnum.Always;
            AudioSource.Stream = (AudioStream)Sounds[slot];
            AudioSource.PitchScale = pitch;
            AudioSource.Play();
        }
    }

    public void DoSoundPath(string path, float pitch, float volume)
    {
        if (!ResourceLoader.Exists(path))
            return;
        //AudioSource.Reparent(SubModels[ActiveModel].GetChild(0));
        AudioSource.VolumeDb = volume;
        AudioSource.Position = Vector3.Zero;
        AudioSource.ProcessMode = ProcessModeEnum.Always;
        AudioSource.Stream = (AudioStream)ResourceLoader.Load(path);
        AudioSource.PitchScale = pitch;
        AudioSource.Play();
    }

    public void DoSoundStream(AudioStream stream, float pitch, float volume)
    {
        //AudioSource.Reparent(SubModels[ActiveModel].GetChild(0));
        AudioSource.VolumeDb = volume;
        AudioSource.Position = Vector3.Zero;
        AudioSource.ProcessMode = ProcessModeEnum.Always;
        AudioSource.Stream = stream;
        AudioSource.PitchScale = pitch;
        AudioSource.Play();
    }

    public void UpdateLayers(int layer)
    {
        //Updating collision and light layers in child nodes
        var agentcol = Call("get_collision_layer_value", 1);
        if (!FirstSetup || (bool)agentcol == true)
        {
            for (int a = 1; a < 9; a++)
            {
                Call("set_collision_mask_value", a, false);
                Call("set_collision_layer_value", a, false);
            }
            Call("set_collision_mask_value", layer, true);
            Call("set_collision_layer_value", layer, true);
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
                for (int a = 1; a < 9; a++)
                {
                    col.SetCollisionMaskValue(a, false);
                    col.SetCollisionLayerValue(a, false);
                }
                col.SetCollisionMaskValue(layer, true);
                col.SetCollisionLayerValue(layer, true);
            }
        }
        foreach (var id in i.GetChildren())
            UpdateLayersNested(id, layer, LimitDrawDistance);
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
        var shad = new Decal();
        shad.Size = new Vector3(dsize.X, 10f, dsize.Y);
        shad.TextureAlbedo = (Texture2D)ResourceLoader.Load(ShadowPaths[type]);
        shad.UpperFade = 0f;
        shad.LowerFade = 0.5f;
        shad.DistanceFadeEnabled= true;
        shad.DistanceFadeBegin= 40;
        shad.Layers = 1;
        shad.Modulate = new Color(1f, 1f, 1f, 0.5f);
        //SubModels[ActiveModel].GetChild(0).AddChild(shad);
        GetNode("Shadows").AddChild(shad);
        shad.Position = new Vector3(shad.Position.X, -4.99f, shad.Position.Z);
    }
}