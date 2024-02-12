using Godot;
namespace Rehab;
public partial class ChunkScene : Node3D
{
    [Export(PropertyHint.File, "*.tscn")]
    public string SkydomePath;
    [Export]
    public Environment WorldEnv;
    [Export]
    public bool ActiveScene;
    [Export]
    public Godot.Collections.Array<ChunkLink> Links = new();
    public int ChunkLayer = 1;
    public int DirShadowCount = 0;

    public void UpdateLayers(int layer)
    {
        ChunkLayer = layer;
        UpdateLayersNested(this);
    }

    void UpdateLayersNested(Node parent)
    {
        foreach (var i in parent.GetChildren())
        {
            UpdateLayersNested(i);
            if (i is VisualInstance3D vis)
            {
                vis.SetLayerMaskValue(1, false);
                vis.SetLayerMaskValue(ChunkLayer, true);
                if (i is Light3D light)
                {
                    var mask = (int)light.LightCullMask | (1 << (ChunkLayer - 1));
                    light.LightCullMask = (uint)mask;
                    if (!ActiveScene)
                        light.ShadowEnabled = false;
                }
            }
            if (i is CollisionObject3D col)
            {
                if (col.GetCollisionLayerValue(1) == false) return;
                col.SetCollisionLayerValue(1, false);
                col.SetCollisionMaskValue(1, false);
                col.SetCollisionMaskValue(ChunkLayer, true);
                col.SetCollisionLayerValue(ChunkLayer, true);
            }
        }
    }

    public void ShadowToggle(bool val)
    {
        DirShadowCount = 0;
        ShadowToggleNested(this, val);
    }

    void ShadowToggleNested(Node parent, bool val)
    {
        foreach (var i in parent.GetChildren())
        {
            ShadowToggleNested(i, val);
            if (i is DirectionalLight3D dir)
            {
                if (!val || DirShadowCount < 4)
                {
                    dir.ShadowEnabled = val;
                    if (val)
                    {
                        DirShadowCount++;
                    }
                }
            }
            if (i is SpotLight3D spot)
            {
                spot.ShadowEnabled = val;
            }
            if (i is OmniLight3D omni)
            {
                omni.ShadowEnabled = val;
            }
        }
    }

    public void OnChunkEnter()
    {
        AgentOnChunkEnter(this);
    }

    public void OnChunkExit()
    {

    }

    void AgentOnChunkEnter(Node parent)
    {
        foreach (var i in parent.GetChildren())
        {
            AgentOnChunkEnter(i);
            if (i is Agent agent)
            {
                agent.OnChunkEnter();
            }
        }
    }
}