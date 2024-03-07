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
    public ChunkScene LoadedChunk;
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
        if (ParentScene.ActiveScene && LoadedChunk != null)
        {
            LoadedChunk.Visible = !SpawnInvisible;
            if (SpawnInvisible)
                LoadedChunk.ProcessMode = ProcessModeEnum.Disabled;
            else
                LoadedChunk.ProcessMode = ProcessModeEnum.Inherit;
        }
        if (ParentScene.ActiveScene && LoadTriggers == null)
            SpawnChunk();
    }

    public void TrigEnter(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (LoadedChunk == null) return;
        if (!ParentScene.ActiveScene) return;
        if (IsBuffered) return;
    }

    public void TrigExit(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (LoadedChunk == null) return;
        if (!ParentScene.ActiveScene) return;
        if (body is AgentCharacter agent)
        {
            if (agent.GetParent() is Agent) return; // attached co-op character
            DisableLinkNow(); // todo remove this?
            agent.isReparenting = true;
            agent.Reparent(LoadedChunk);
            agent.ParentScene = LoadedChunk;
            agent.UpdateLayers(LoadedChunk.ChunkLayer);
            if (AgentCharacter.activeCharacter == agent)
            {
                GD.Print("---");
                GD.Print("[LINK] Entered from " + ParentScene.Name);
                SwitchToChunk(LoadedChunk);
            }
        }
    }

    public void LoadTrigEnter(Node3D body)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;
        if (LoadedChunk != null) return;
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
        if (LoadedChunk == null) return;
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
        if (OS.GetName() == "Android")
        {
            // todo Android only streaming crash issue
            return;
        }
        if (LoadedChunk != null || IsLoading) return;
        IsLoading = true;
        if (LoadedScene == null)
        {
            string FullChunkPath = RehabGame.AssetsPath + ChunkPath;
            if (FileAccess.FileExists(FullChunkPath))
            {
                ResourceLoader.LoadThreadedRequest(FullChunkPath);
                while (ResourceLoader.LoadThreadedGetStatus(FullChunkPath) == ResourceLoader.ThreadLoadStatus.InProgress)
                    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                if (ResourceLoader.LoadThreadedGetStatus(FullChunkPath) == ResourceLoader.ThreadLoadStatus.Loaded)
                    LoadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(FullChunkPath);
                else
                    GD.PrintErr("[ChunkLink] FAILED TO LOAD SCENE AT " + FullChunkPath);
            }
            else
            {
                GD.PrintErr("[ChunkLink] FAILED TO FIND SCENE AT " + FullChunkPath);
                return;
            }
        }
        LoadedChunk = (ChunkScene)RehabScene.Root.LoadChunk(LoadedScene, ChunkName, GetNode<Node3D>("ChunkHolder"));
        if (LoadedChunk != null && SpawnInvisible)
        {
            LoadedChunk.Visible = false;
            LoadedChunk.ProcessMode = ProcessModeEnum.Disabled;
        }
        IsLoading = false;
    }

    public void DespawnChunk()
    {
        if (LoadedChunk == null) return;
        RehabScene.Root.UnloadChunk(LoadedChunk.Name);
        LoadedChunk = null;
    }

    public void SwitchToChunk(ChunkScene chunk)
    {
        RehabScene.Root.SwitchToChunk(chunk);
    }
}