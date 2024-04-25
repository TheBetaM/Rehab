using Godot;
using System.Collections.Generic;
namespace Rehab;
public partial class ChunkLink : Node3D
{
    [Export(PropertyHint.File, "*.tscn")]
    public string ChunkPath;
    [Export]
    public string ChunkName;
    [Export]
    public bool IsDisabled;
    [Export]
    public bool VisibleAlways;
    [Export]
    public bool VisibleInFrustum;
    [Export]
    public bool UnkFlag;
    [Export]
    public bool CustomLink;

    ChunkScene ParentScene;
    PackedScene LoadedScene;
    Area3D EnterTrigger;
    Area3D LoadTriggers; //todo array
    VisibleOnScreenNotifier3D EnterNotifier;
    bool IsLoading;
    bool InUse;
    bool AllowSpawn;
    public static bool AndroidStall = false;
    public static List<object> AndroidQueue = new();

    public override void _Ready()
    {
        ParentScene = (ChunkScene)GetParent().GetParent();
        ParentScene.Links.Add(this);

        if (string.IsNullOrWhiteSpace(ChunkPath)) return;

        if (GetNodeOrNull("EnterTrigger") != null && !IsDisabled)
        {
            EnterTrigger = GetNode<Area3D>("EnterTrigger");
            EnterTrigger.BodyEntered += TrigEnter;
            EnterTrigger.BodyExited += TrigExit;
        }
        if (GetNodeOrNull("EnterNotifier") != null && !IsDisabled)
        {
            EnterNotifier = GetNode<VisibleOnScreenNotifier3D>("EnterNotifier");
            EnterNotifier.ScreenEntered += LinkInView;
            EnterNotifier.ScreenExited += LinkExitView;
        }

        if (GetNodeOrNull("LoadTriggers") != null)
        {
            LoadTriggers = GetNode<Area3D>("LoadTriggers/LoadTrigger_0");
            LoadTriggers.BodyEntered += LoadTrigEnter;
            LoadTriggers.BodyExited += LoadTrigExit;
        }
        else if (ParentScene.ActiveScene)
        {
            AllowSpawn = true;
        }

        if (!ParentScene.ActiveScene)
            DisableLinkNow();
    }

    public override void _Process(double delta)
    {
        // Android crashing bug workaround Part 1
        if (AllowSpawn && !IsLoading && !InUse && !RehabScene.Root.ChunkNames.Contains(ChunkName))
        {
            SpawnChunk();
        }
    }

    public void DisableLink()
    {
        IsLoading = true;
    }

    public void DisableLinkNow()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public void ActivateLink()
    {
        if (string.IsNullOrWhiteSpace(ChunkPath)) return;
        ProcessMode = ProcessModeEnum.Inherit;
        IsLoading = false;
        InUse = false;
        if (ParentScene.ActiveScene && RehabScene.Root.ChunkNames.Contains(ChunkName))
        {
            int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
            if (!VisibleAlways)
            {
                if (VisibleInFrustum && EnterNotifier != null && EnterNotifier.IsOnScreen())
                {
                    RehabScene.Root.Chunks[ind].Visible = true;
                    RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
                }
                else
                {
                    RehabScene.Root.Chunks[ind].Visible = false;
                    RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Disabled;
                }
            }
            else
            {
                RehabScene.Root.Chunks[ind].Visible = true;
                RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
            }
        }
        if (ParentScene.ActiveScene && LoadTriggers == null)
            SpawnChunk();
    }

    public void TrigEnter(Node3D body)
    {

    }

    public void TrigExit(Node3D body)
    {
        if (InUse || ProcessMode == ProcessModeEnum.Disabled) return;
        if (!RehabScene.Root.ChunkNames.Contains(ChunkName)) return;
        if (!ParentScene.ActiveScene || IsLoading || InUse) return;
        if (body is AgentCharacter agent)
        {
            if (agent.GetParent() is Agent) return; // attached co-op character
            InUse = true;
            agent.isSwitchingChunks = true;
            agent.isReparenting = true;
            int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
            agent.Reparent(RehabScene.Root.Chunks[ind]);
            agent.ParentScene = RehabScene.Root.Chunks[ind];
            agent.UpdateLayers(RehabScene.Root.Chunks[ind].ChunkLayer);
            if (AgentCharacter.activeCharacter == agent)
            {
                GD.Print("---");
                GD.Print("[LINK] Entered from " + ParentScene.Name);
                SwitchToChunk(RehabScene.Root.Chunks[ind]);
                agent.isSwitchingChunks = false;
            }
        }
    }

    public void LoadTrigEnter(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (RehabScene.Root.ChunkNames.Contains(ChunkName)) return;
        if (!ParentScene.ActiveScene || IsLoading || InUse) return;
        if (body is AgentCharacter agent && AgentCharacter.activeCharacter == agent)
            AllowSpawn = true;
    }

    public void LoadTrigExit(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (!RehabScene.Root.ChunkNames.Contains(ChunkName)) return;
        if (!ParentScene.ActiveScene || IsLoading || InUse) return;
        if (body is AgentCharacter agent && RehabScene.PlayerCam.Current && !RehabScene.GameMenu.Visible && !agent.isSwitchingChunks)
        {
            if (AgentCharacter.activeCharacter == agent)
            {
                AllowSpawn = false;
                DespawnChunk();
            }
        }
    }

    public async void SpawnChunk()
    {
        if (RehabScene.Root.ChunkNames.Contains(ChunkName) || IsLoading) return;
        IsLoading = true;
        if (LoadedScene == null)
        {
            string FullChunkPath = RehabGame.AssetsPath + ChunkPath;
            if (CustomLink)
            {
                FullChunkPath = ChunkPath;
            }
            if (FileAccess.FileExists(FullChunkPath))
            {
                if (OS.GetName() == "Android")
                {
                    // Android crashing bug workaround Part 2
                    lock (AndroidQueue)
                    {
                        AndroidQueue.Add(this);
                    }
                    while (AndroidStall || AndroidQueue[0] != this)
                        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                    AndroidStall = true;
                    lock (AndroidQueue)
                    {
                        AndroidQueue.RemoveAt(0);
                    }
                }
                ResourceLoader.LoadThreadedRequest(FullChunkPath);
                while (ResourceLoader.LoadThreadedGetStatus(FullChunkPath) == ResourceLoader.ThreadLoadStatus.InProgress)
                    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                if (ResourceLoader.LoadThreadedGetStatus(FullChunkPath) == ResourceLoader.ThreadLoadStatus.Loaded)
                    LoadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(FullChunkPath);
                else
                    GD.PrintErr("[ChunkLink] FAILED TO LOAD SCENE AT " + FullChunkPath);
                AndroidStall = false;
            }
            else
            {
                GD.PrintErr("[ChunkLink] FAILED TO FIND SCENE AT " + FullChunkPath);
                return;
            }
        }
        RehabScene.Root.LoadChunk(LoadedScene, ChunkName, GetNode<Node3D>("ChunkHolder"));
        if (RehabScene.Root.ChunkNames.Contains(ChunkName))
        {
            int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
            if (RehabScene.Root.IsLoadingXR)
            {
                RehabScene.Root.Chunks[ind].Visible = false;
                RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Disabled;
            }
            else if (!VisibleAlways)
            {
                if (VisibleInFrustum && EnterNotifier != null && EnterNotifier.IsOnScreen())
                {
                    RehabScene.Root.Chunks[ind].Visible = true;
                    RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
                }
                else
                {
                    RehabScene.Root.Chunks[ind].Visible = false;
                    RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Disabled;
                }
            }
            else
            {
                RehabScene.Root.Chunks[ind].Visible = true;
                RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
            }
        }
        IsLoading = false;
    }

    public void DespawnChunk()
    {
        RehabScene.Root.UnloadChunk(ChunkName);
    }

    public void SwitchToChunk(ChunkScene chunk)
    {
        RehabScene.Root.SwitchToChunk(chunk);
    }

    public void UpdateVisibility()
    {
        if (RehabScene.Root.ChunkNames.Contains(ChunkName))
        {
            int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
            if (!VisibleAlways)
            {
                if (VisibleInFrustum && EnterNotifier != null && EnterNotifier.IsOnScreen())
                {
                    RehabScene.Root.Chunks[ind].Visible = true;
                    RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
                }
                else
                {
                    RehabScene.Root.Chunks[ind].Visible = false;
                    RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Disabled;
                }
            }
            else
            {
                RehabScene.Root.Chunks[ind].Visible = true;
                RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
            }
        }
    }

    public void LinkInView()
    {
        if (!VisibleInFrustum || !RehabScene.Root.ChunkNames.Contains(ChunkName) || !ParentScene.ActiveScene || ProcessMode == ProcessModeEnum.Disabled) return;
        int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
        RehabScene.Root.Chunks[ind].Visible = true;
        RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
    }

    public void LinkExitView()
    {
        if (!VisibleInFrustum || !RehabScene.Root.ChunkNames.Contains(ChunkName) || !ParentScene.ActiveScene || ProcessMode == ProcessModeEnum.Disabled) return;
        int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
        RehabScene.Root.Chunks[ind].Visible = false;
        RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Disabled;
    }
}