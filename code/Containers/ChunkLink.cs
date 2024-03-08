using Godot;
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
    public bool SpawnInvisible;

    ChunkScene ParentScene;
    PackedScene LoadedScene;
    Area3D EnterTrigger;
    Area3D LoadTriggers; //todo array
    public bool IsBuffered;
    bool IsLoading;

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

        if (GetNodeOrNull("LoadTriggers") != null)
        {
            LoadTriggers = GetNode<Area3D>("LoadTriggers/LoadTrigger_0");
            LoadTriggers.BodyEntered += LoadTrigEnter;
            LoadTriggers.BodyExited += LoadTrigExit;
        }
        else if (ParentScene.ActiveScene)
        {
            SpawnChunk();
        }

        if (!ParentScene.ActiveScene)
            DisableLinkNow();
    }

    public void DisableLink()
    {
        CallDeferred("DisableLinkNow");
    }

    public void DisableLinkNow()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public void ActivateLink()
    {
        if (string.IsNullOrWhiteSpace(ChunkPath)) return;
        ProcessMode = ProcessModeEnum.Inherit;
        if (ParentScene.ActiveScene && RehabScene.Root.ChunkNames.Contains(ChunkName))
        {
            int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
            RehabScene.Root.Chunks[ind].Visible = !SpawnInvisible;
            if (SpawnInvisible)
                RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Disabled;
            else
                RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Inherit;
        }
        if (ParentScene.ActiveScene && LoadTriggers == null)
            SpawnChunk();
    }

    public void TrigEnter(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (!ParentScene.ActiveScene) return;
        if (IsBuffered) return;
    }

    public void TrigExit(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (!RehabScene.Root.ChunkNames.Contains(ChunkName)) return;
        if (!ParentScene.ActiveScene) return;
        if (body is AgentCharacter agent)
        {
            if (agent.GetParent() is Agent) return; // attached co-op character
            DisableLinkNow(); // todo remove this?
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
            }
        }
    }

    public void LoadTrigEnter(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (RehabScene.Root.ChunkNames.Contains(ChunkName)) return;
        if (!ParentScene.ActiveScene) return;
        if (body is CharacterBody3D cbody)
        {
            if (body is AgentCharacter agent && AgentCharacter.activeCharacter == agent)
                SpawnChunk();
        }
    }

    public void LoadTrigExit(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (!RehabScene.Root.ChunkNames.Contains(ChunkName)) return;
        if (!ParentScene.ActiveScene) return;
        if (body is CharacterBody3D && RehabScene.PlayerCam.Current && !RehabScene.GameMenu.Visible)
        {
            var agentb = body.GetParent().GetParent().GetParent();
            if (agentb is AgentCharacter agent && AgentCharacter.activeCharacter == agent)
            {
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
            if (FileAccess.FileExists(FullChunkPath))
            {
                if (OS.GetName() == "Android")
                {
                    // bug: Android crashes trying to stream too many levels from a pack
                    LoadedScene = (PackedScene)ResourceLoader.Load(FullChunkPath);
                }
                else
                {
                    ResourceLoader.LoadThreadedRequest(FullChunkPath);
                    while (ResourceLoader.LoadThreadedGetStatus(FullChunkPath) == ResourceLoader.ThreadLoadStatus.InProgress)
                        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                    if (ResourceLoader.LoadThreadedGetStatus(FullChunkPath) == ResourceLoader.ThreadLoadStatus.Loaded)
                        LoadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(FullChunkPath);
                    else
                        GD.PrintErr("[ChunkLink] FAILED TO LOAD SCENE AT " + FullChunkPath);
                }
            }
            else
            {
                GD.PrintErr("[ChunkLink] FAILED TO FIND SCENE AT " + FullChunkPath);
                return;
            }
        }
        RehabScene.Root.LoadChunk(LoadedScene, ChunkName, GetNode<Node3D>("ChunkHolder"));
        if (RehabScene.Root.ChunkNames.Contains(ChunkName) && SpawnInvisible)
        {
            int ind = RehabScene.Root.ChunkNames.IndexOf(ChunkName);
            RehabScene.Root.Chunks[ind].Visible = false;
            RehabScene.Root.Chunks[ind].ProcessMode = ProcessModeEnum.Disabled;
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
}