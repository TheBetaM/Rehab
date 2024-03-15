using Godot;
using System.Collections.Generic;
using System;
namespace Rehab;
public partial class ChunkScene : Node3D
{
    [Export(PropertyHint.File, "*.tscn")]
    public string SkydomePath;
    [Export]
    public Godot.Environment WorldEnv;
    [Export]
    public bool ActiveScene;
    [Export]
    public Godot.Collections.Array<ChunkLink> Links = new();
    public int ChunkLayer = 1;
    public int DirShadowCount = 0;

    public event EventHandler OnChunkEnter;
    public event EventHandler OnChunkExit;

    public virtual void InitGame()
    {
        RehabScene.PlayerCam.Current = true;
        RehabScene.GameHUD.OnUnPause();
        ShadowToggle(true);
        PlayerCheck();
    }

    async void PlayerCheck()
    { 
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        ChunkEnter();
        await ToSignal(GetTree().CreateTimer(3.5f), SceneTreeTimer.SignalName.Timeout);
        if (AgentCharacter.activeCharacter == null && !RehabScene.FE.GetNode<Control>("LevelSelect").Visible &&
        !RehabScene.FE.GetNode<Control>("Loading").Visible && !RehabScene.FE.GetNode<Control>("FE_MainMenuDynamic").Visible && 
        !RehabScene.FE.GetNode<Control>("FE_Credits").Visible)
        {
            GD.PrintErr("[ROOT] LEVEL LOADED WITH NO CHARACTER");
            RehabScene.Root.StartMessage("#FE-NoActorError");
            await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
            while (RehabScene.GameMenu.Visible)
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            RehabScene.Root.ExitLevel(false);
        }
    }

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

    public async void ShadowToggle(bool val)
    {
        if (val)
            await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        else
            await ToSignal(GetTree().CreateTimer(0.15f), SceneTreeTimer.SignalName.Timeout);
        DirShadowCount = 0;
        ShadowToggleNested(GetNode("Lights"), val);
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

    public void ChunkEnter()
    {
        OnChunkEnter?.Invoke(this, null);
    }

    public void ChunkExit()
    {
        OnChunkExit?.Invoke(this, null);
    }
}