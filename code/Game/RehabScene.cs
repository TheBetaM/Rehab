using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Rehab;
public partial class RehabScene : Node3D
{
    [Export]
    public Environment DefaultEnv;
    public ChunkScene ActiveChunk;
    public Node3D Skydome;
    public string SkydomePath;
    public static RehabScene Root;
    public static PlayerCamera PlayerCam;
    public static FreeLookCamera FreeLookCam;
    public static FrontendHUD GameHUD;
    public static FrontendMenu GameMenu;
    public static AudioStreamPlayer AudioMusic;
    public static AudioStreamPlayer AudioAmbience;
    public static AudioStreamPlayer AudioMenu;
    public static AudioStreamPlayer AudioVoice;
    public List<ChunkScene> Chunks = new();
    public List<StringName> ChunkNames = new();
    public List<int> ChunkLayers = new();
    public string LoadingChunkName;
    const int MaxChunksLoaded = 8; // at the same time
    public string ActiveMusic;
    public string ActiveAmbience;
    public AudioStream SoundFE_Back;
    public AudioStream SoundFE_Click;
    public AudioStream SoundFE_Select;
    bool MusicSwitching;
    bool AmbSwitching;
    public static Control FE;
    public XRInterface XR_Interface;
    public bool XR_Enabled = false;
    public RehabXROrigin XR_Origin;
    public SubViewport FE_XR_Viewport;
    public bool IsLoadingXR;
    public bool IsLoadingScene;
    

    public RehabScene()
    {
        Root = this;
    }

    public override void _Ready()
    {
        FE = GetNode<Control>("FE");
        PlayerCam = GetNode<PlayerCamera>("PlayerCam");
        FreeLookCam = GetNode<FreeLookCamera>("FreeLookCam");
        GameHUD = FE.GetNode<FrontendHUD>("FE_HUD");
        GameMenu = FE.GetNode<FrontendMenu>("FE_Menu");
        AudioMusic = GetNode<AudioStreamPlayer>("Audio/AudioMusic1");
        AudioAmbience = GetNode<AudioStreamPlayer>("Audio/AudioAmb1");
        AudioMenu = GetNode<AudioStreamPlayer>("Audio/AudioMenu");
        var env = GetNode<WorldEnvironment>("WorldEnv");
        env.Environment = DefaultEnv;
        XR_Interface = XRServer.FindInterface("OpenXR");
        XR_Origin = GetNode<RehabXROrigin>("XROrigin3D");
        FE_XR_Viewport = GetNode<SubViewport>("XR_FE");
        if (XR_Interface != null && XR_Interface.IsInitialized())
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            GetViewport().UseXR = true;
            FE.Reparent(FE_XR_Viewport);
            XR_Origin.Visible = true;
            XR_Enabled = true;
        }
        else
        {
            XR_Origin.QueueFree();
            FE_XR_Viewport.QueueFree();
        }
        if (OS.HasFeature("mobile") ||
            (string)ProjectSettings.GetSetting("rendering/renderer/rendering_method") == "mobile")
        {
            FixMobileFE(FE);
        }
        RehabGame.Init();
        var conf = GetNode<ConfigHandler>("ConfigHandler");
        conf.Load();
        conf.Setup();
        GameInit();
    }

    public async void GameInit()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        LoadPacks();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        if (ResourceLoader.Exists(RehabGame.AssetsPath + "Sounds/Menu/FE_BACK.res"))
            SoundFE_Back = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Menu/FE_BACK.res");
	    if (ResourceLoader.Exists(RehabGame.AssetsPath + "Sounds/Menu/FE_CLICK.res"))
            SoundFE_Click = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Menu/FE_CLICK.res");
	    if (ResourceLoader.Exists(RehabGame.AssetsPath + "Sounds/Menu/FE_SELECT.res"))
            SoundFE_Select = (AudioStream)ResourceLoader.Load(RehabGame.AssetsPath + "Sounds/Menu/FE_SELECT.res");
	
        var dir = DirAccess.Open(RehabGame.AssetsPath + "Levels/");
        if (dir != null)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            GameHUD.Setup();
            RehabGame.SetupMods();
            if (RehabGame.Dev)
            {
                StartLevelSelect();
            }
            else
            {
                StartMessage($"#FE-Explorer-Disclaimer-{System.Random.Shared.Next(0, 11)}");
                await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
                while (GameMenu.Visible)
                    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                StartMainMenu();
            }
        }
        else
        {
            GD.Print("[ROOT] Cannot open " + RehabGame.AssetsPath + "Levels/");
            FE.GetNode<FrontendInstaller>("FE_Installer").Activate();
            if (XR_Enabled)
            {
                XR_Origin.FE_Active();
            }
        }
    }

    void LoadPacks()
    {
        string PacksPath = RehabGame.DataPath;
        var pdir = DirAccess.Open(PacksPath);
        if (pdir != null)
        {
            foreach (var i in pdir.GetFiles())
            {
                var success = ProjectSettings.LoadResourcePack(PacksPath + i);
                if (success)
                    GD.Print("[ROOT] Pack loaded from " + i);
                else
                    GD.PrintErr("[ROOT] Pack FAILED from " + i);
            }
        }
        else
        {
            GD.Print("[ROOT] Data directory failed to open! " + PacksPath);
        }
        if (RehabGame.PacksPath == RehabGame.DataPath || string.IsNullOrWhiteSpace(RehabGame.PacksPath)) return;
        PacksPath = RehabGame.PacksPath;
        pdir = DirAccess.Open(PacksPath);
        if (pdir != null)
        {
            foreach (var i in pdir.GetFiles())
            {
                var success = ProjectSettings.LoadResourcePack(PacksPath + i);
                if (success)
                    GD.Print("[ROOT] Pack loaded from " + i);
                else
                    GD.PrintErr("[ROOT] Pack FAILED from " + i);
            }
        }
        else
        {
            GD.Print("[ROOT] Packs directory failed to open! " + PacksPath);
        }
    }

    public async void LoadScene(string path)
    {
        GD.Print($"[ROOT] Loading {path}");
        IsLoadingScene = true;
        if (XR_Enabled) IsLoadingXR = true;
        UnloadAllChunks();
        LoadingChunkName = path.Split("/").Last().Replace(".tscn","");
        PlayerCam.FullReset();
        FreeLookCam.FullReset();
        GameHUD.Clear();
        FE.GetNode<LoadingVisuals>("Loading").AnimIn();
        FE.GetNode<LoadingVisuals>("Loading").ProcessMode = ProcessModeEnum.Inherit;
        ResourceLoader.LoadThreadedRequest(path);
        while (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.InProgress)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        if (ResourceLoader.LoadThreadedGetStatus(path) != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            GD.PrintErr("[ROOT] FAILED TO LOAD SCENE AT " + path);
            GetTree().Quit();
            return;
        }
        var loadedPack = (PackedScene)ResourceLoader.LoadThreadedGet(path);
        var loadedScene = (ChunkScene)loadedPack.Instantiate();
        loadedScene.ActiveScene = true;
        loadedScene.ProcessMode = ProcessModeEnum.Disabled;
        if (XR_Enabled) loadedScene.Visible = false;
        ActiveChunk = loadedScene;
        Chunks.Add(loadedScene);
        ChunkNames.Add(loadedScene.Name);
        for (int i = 1; i <= MaxChunksLoaded; i++)
        {
            if (!ChunkLayers.Contains(i))
            {
                ChunkLayers.Add(i);
                loadedScene.UpdateLayers(i);
                break;
            }
        }
        loadedScene.WorldEnv.TonemapMode = Environment.ToneMapper.Reinhardt;
        GetNode<WorldEnvironment>("WorldEnv").Environment = loadedScene.WorldEnv;
        AddChild(loadedScene);
        string skypath = RehabGame.AssetsPath + loadedScene.SkydomePath;
        if (!string.IsNullOrWhiteSpace(loadedScene.SkydomePath))
        {
            if (!string.IsNullOrWhiteSpace(SkydomePath))
                Skydome.QueueFree();
            var sky = (PackedScene)ResourceLoader.Load(skypath);
            SkydomePath = skypath;
            Skydome = (Node3D)sky.Instantiate();
            if (XR_Enabled) Skydome.Visible = false;
            AddChild(Skydome);
        }
        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        loadedScene.ProcessMode = ProcessModeEnum.Inherit;
        loadedScene.InitGame();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        FE.GetNode<LoadingVisuals>("Loading").AnimOut();
        if (RehabGame.UseMouseCamera) Input.MouseMode = Input.MouseModeEnum.Captured;
        if (XR_Enabled) XR_Origin.FE_Inactive();
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        FE.GetNode<LoadingVisuals>("Loading").Visible = false;
        FE.GetNode<LoadingVisuals>("Loading").ProcessMode = ProcessModeEnum.Disabled;
        if (XR_Enabled) 
        {
            loadedScene.Visible = true;
            foreach (var link in loadedScene.Links)
            {
                link.UpdateVisibility();
            }
            if (!string.IsNullOrWhiteSpace(SkydomePath))
                Skydome.Visible = true;
            XR_Origin.ToggleHands(true);
            if (XR_Origin.XR_HandL.HasHand)
                XR_Origin.XR_HandL.HandModel.UpdateLayers(loadedScene.ChunkLayer);
            if (XR_Origin.XR_HandR.HasHand) 
                XR_Origin.XR_HandR.HandModel.UpdateLayers(loadedScene.ChunkLayer);
        }
        IsLoadingXR = false;
        IsLoadingScene = false;
    }

    public Node3D LoadChunk(PackedScene chunk, string chunkName, Node3D holder)
    {
        if (ChunkNames.Contains(chunkName)) return null;
        lock (ChunkNames)
        {
		    ChunkNames.Add(chunkName);
        }
        GD.Print("[ROOT] Spawning " + chunkName);
		var LoadedChunk = chunk.Instantiate();
		holder.AddChild(LoadedChunk);
		LoadedChunk.Reparent(this);
        var scene = (ChunkScene)LoadedChunk;
        lock (Chunks)
        {
            Chunks.Add(scene);
        }
        lock (ChunkLayers)
        {
            for (int i = 1; i <= MaxChunksLoaded; i++)
            {
                if (!ChunkLayers.Contains(i))
                {
                    ChunkLayers.Add(i);
                    scene.UpdateLayers(i);
                    break;
                }
            }
        }
		return scene;
    }

    public void SwitchToChunk(ChunkScene chunk)
    {
        if (chunk == ActiveChunk) return;
        
        var OldChunk = ActiveChunk;
        // Disabling links of chunk that we're exiting
        foreach (var i in OldChunk.Links)
            i.DisableLink();
        OldChunk.ActiveScene = false;
        OldChunk.ChunkExit();
        OldChunk.ShadowToggle(false);
        chunk.ActiveScene = true;
        GD.Print("[ROOT] Entering " + chunk.Name);
        
        // Updating World Environment and Lights
        chunk.ShadowToggle(true);
        UpdateWorldEnv(chunk.WorldEnv);
        if (XR_Enabled)
        {
            if (XR_Origin.XR_HandL.HasHand)
                XR_Origin.XR_HandL.HandModel.UpdateLayers(chunk.ChunkLayer);
            if (XR_Origin.XR_HandR.HasHand)
                XR_Origin.XR_HandR.HandModel.UpdateLayers(chunk.ChunkLayer);
        }

        // Updating Skydome
        string skypath = RehabGame.AssetsPath + chunk.SkydomePath;
        if (SkydomePath != skypath && !string.IsNullOrWhiteSpace(chunk.SkydomePath))
        {
            if (!string.IsNullOrWhiteSpace(SkydomePath))
                Skydome.QueueFree();
            SkydomePath = "";
            SpawnSkydome(skypath);
        }
        else if (string.IsNullOrWhiteSpace(chunk.SkydomePath))
        {
            if (!string.IsNullOrWhiteSpace(SkydomePath))
                Skydome.QueueFree();
            SkydomePath = "";
        }
        
        // Centering active chunk to (0,0,0) and others around it
        var ChunkOffset = chunk.GlobalPosition;
        foreach (var c in Chunks)
            c.GlobalPosition += -ChunkOffset;
        if (XR_Enabled)
        {
            XR_Origin.GlobalPosition += -ChunkOffset;
        }
        PlayerCam.pivot += -ChunkOffset;
        PlayerCam.GlobalPosition += -ChunkOffset;
        
        // Activating and starting links of entered chunk
        foreach (var i in chunk.Links)
        {
            if (i.ChunkName == OldChunk.Name)
            {
                int ind = ChunkNames.IndexOf(i.ChunkName);
                Chunks[ind].Position = i.GetNode<Node3D>("ChunkHolder").GlobalPosition;
                Chunks[ind].Rotation = i.GetNode<Node3D>("ChunkHolder").GlobalRotation;
            }
            foreach (var a in OldChunk.Links)
            {
                if (a.ChunkName == i.ChunkName && ChunkNames.Contains(a.ChunkName))
                {
                    int ind = ChunkNames.IndexOf(i.ChunkName);
                    Chunks[ind].Position = i.GetNode<Node3D>("ChunkHolder").GlobalPosition;
                    Chunks[ind].Rotation = i.GetNode<Node3D>("ChunkHolder").GlobalRotation;
                }
            }
        }
        ActivateLinks(chunk);
        
        // Disposing of unlinked chunks
        var NameCopy = new List<StringName>(ChunkNames);
        foreach (var cn in NameCopy)
        {
            var found = false;
            foreach (var c in chunk.Links)
            {
                if (c.ChunkName == cn)
                {
                    found = true;
                    break;
                }
            }
            if (!found && cn != chunk.Name)
            {
                UnloadChunk(cn);
            }
        }
        
        ActiveChunk = chunk;
        chunk.Visible = true;
        chunk.ProcessMode = ProcessModeEnum.Inherit;
        chunk.ChunkEnter();
    }

    async void UpdateWorldEnv(Environment Env)
    {
        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
        Env.TonemapMode = Environment.ToneMapper.Reinhardt;
        GetNode<WorldEnvironment>("WorldEnv").Environment = Env;
    }

    async void QueueUnload(List<StringName> chunks)
    {
        await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
        foreach (var a in chunks)
        {
            UnloadChunk(a);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
    }

    async void SpawnSkydome(string path)
    {
        await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);
        if (OS.GetName() == "Android")
        {
            lock (ChunkLink.AndroidQueue)
            {
                ChunkLink.AndroidQueue.Add(this);
            }
            while (ChunkLink.AndroidStall || ChunkLink.AndroidQueue[0] != this)
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            ChunkLink.AndroidStall = true;
            lock (ChunkLink.AndroidQueue)
            {
                ChunkLink.AndroidQueue.RemoveAt(0);
            }
        }
        ResourceLoader.LoadThreadedRequest(path);
        while (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.InProgress)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        var sky = (PackedScene)ResourceLoader.LoadThreadedGet(path);
        Skydome = (Node3D)sky.Instantiate();
        AddChild(Skydome);
        SkydomePath = path;
        ChunkLink.AndroidStall = false;
    }

    async void ActivateLinks(ChunkScene chunk)
    {
        await ToSignal(GetTree().CreateTimer(0.6f), SceneTreeTimer.SignalName.Timeout);
        foreach (var i in chunk.Links)
        {
            i.ActivateLink();
            await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);
        }
    }

    async void CenterWorld(Vector3 ChunkOffset)
    {
        await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
        foreach (var c in Chunks)
            c.GlobalPosition += -ChunkOffset;
        if (XR_Enabled)
        {
            XR_Origin.GlobalPosition += -ChunkOffset;
        }
        PlayerCam.pivot += -ChunkOffset;
        PlayerCam.GlobalPosition += -ChunkOffset;
    }

    public void UnloadChunk(string chunk)
    {
        var pos = ChunkNames.IndexOf(chunk);
        if (pos == -1) return;
        GD.Print("[ROOT] Unloading " + chunk);
        lock (ChunkNames)
        {
            Chunks[pos].QueueFree();
            Chunks.RemoveAt(pos);
            ChunkNames.RemoveAt(pos);
            ChunkLayers.RemoveAt(pos);
        }
    }

    void UnloadAllChunks()
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            Chunks[i].QueueFree();
        }
        Chunks.Clear();
        ChunkNames.Clear();
        ChunkLayers.Clear();
        AgentCharacter.ActiveActorTypes.Clear();
        AgentCharacter.activeCharacter = null;
        PlayerCam.camTarget = null;
        if (!string.IsNullOrWhiteSpace(SkydomePath))
            Skydome.QueueFree();
        SkydomePath = "";
        ChunkLink.AndroidStall = false;
        ChunkLink.AndroidQueue.Clear();
        if (XR_Enabled)
        {
            XR_Origin.ClearHands();
            XR_Origin.ToggleHands(false);
        }
    }

    public void ExitLevel(bool toMain)
    {
        UnloadAllChunks();
        GameHUD.Clear();
        AudioAmbience.Stop();
        ActiveAmbience = "";
        AudioMusic.Stop();
        ActiveMusic = "";
        GetNode<WorldEnvironment>("WorldEnv").Environment = DefaultEnv;
        if (OS.GetName() == "Android")
        {
            DisplayServer.ScreenSetOrientation(DisplayServer.ScreenOrientation.SensorLandscape);
        }
        if (!toMain)
        {
            StartLevelSelect();
        }
        else
        {
            StartMainMenu();
        }
    }

    public void StartLevelSelect()
    {
        if (XR_Enabled) XR_Origin.FE_Active();
        Input.MouseMode = Input.MouseModeEnum.Visible;
	    FE.GetNode<LevelSelectList>("LevelSelect").Activate();
    }

    public void StartMainMenu()
    {
        if (XR_Enabled) XR_Origin.FE_Active();
        Input.MouseMode = Input.MouseModeEnum.Visible;
	    FE.GetNode<MainMenuDynamic>("FE_MainMenuDynamic").Activate();
    }

    public void StartPauseMenu(bool optionsOnly)
    {
        ProcessMode = ProcessModeEnum.Disabled;
        if (XR_Enabled) XR_Origin.FE_Active();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GameHUD.OnPause();
        GameMenu.Start_PauseMenu(optionsOnly);
    }

    public void StartMessage(string text)
    {
        ProcessMode = ProcessModeEnum.Disabled;
        if (XR_Enabled) XR_Origin.FE_Active();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GameHUD.OnPause();
        GameMenu.Start_Message(text);
    }

    public async void PlayMusic(int id)
    {
        if (MusicSwitching) return;
        var path = "";
        if (RehabGame.MusicPaths.ContainsKey(id))
            path = RehabGame.AssetsPath + "Sounds/Music/" + RehabGame.MusicPaths[id] + ".res";
        if (path == "" || !ResourceLoader.Exists(path)) return;
        if (ActiveMusic == path) return;
        ActiveMusic = path;
        MusicSwitching = true;
        if (AudioMusic.Playing)
        {
            var musicFader = (MusicFader)AudioMusic;
            musicFader.IsFadingOut = true;
            if (AudioMusic == GetNode<AudioStreamPlayer>("Audio/AudioMusic1"))
                AudioMusic = GetNode<AudioStreamPlayer>("Audio/AudioMusic2");
            else
                AudioMusic = GetNode<AudioStreamPlayer>("Audio/AudioMusic1");
            musicFader = (MusicFader)AudioMusic;
            musicFader.IsFadingOut = false;
        }
        await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);
        if (OS.GetName() == "Android")
        {
            var loadedTrack = (AudioStream)ResourceLoader.Load(path);
            AudioMusic.Stream = loadedTrack;
            AudioMusic.VolumeDb = 0.0f;
            AudioMusic.Play();
        }
        else
        {
            ResourceLoader.LoadThreadedRequest(path);
            while (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.InProgress)
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            if (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.Loaded)
            {
                var loadedTrack = (AudioStream)ResourceLoader.LoadThreadedGet(path);
                AudioMusic.Stream = loadedTrack;
                AudioMusic.VolumeDb = 0.0f;
                AudioMusic.Play();
            }
        }
        MusicSwitching = false;
    }

    public async void PlayAmbience(int id)
    {
        if (AmbSwitching) return;
        var path = "";
        if (RehabGame.MusicPaths.ContainsKey(id))
            path = RehabGame.AssetsPath + "Sounds/Music/" + RehabGame.MusicPaths[id] + ".res";
        if (path == "" || !ResourceLoader.Exists(path)) return;
        if (ActiveAmbience == path) return;
        ActiveAmbience = path;
        AmbSwitching = true;
        if (AudioAmbience.Playing)
        {
            var musicFader = (MusicFader)AudioAmbience;
            musicFader.IsFadingOut = true;
            if (AudioAmbience == GetNode<AudioStreamPlayer>("Audio/AudioAmb1"))
                AudioAmbience = GetNode<AudioStreamPlayer>("Audio/AudioAmb2");
            else
                AudioAmbience = GetNode<AudioStreamPlayer>("Audio/AudioAmb1");
            musicFader = (MusicFader)AudioAmbience;
            musicFader.IsFadingOut = false;
        }
        await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);
        if (OS.GetName() == "Android")
        {
            var loadedTrack = (AudioStream)ResourceLoader.Load(path);
            AudioAmbience.Stream = loadedTrack;
            AudioAmbience.VolumeDb = -10.0f;
            AudioAmbience.Play();
        }
        else
        {
            ResourceLoader.LoadThreadedRequest(path);
            while (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.InProgress)
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            if (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.Loaded)
            {
                var loadedTrack = (AudioStream)ResourceLoader.LoadThreadedGet(path);
                AudioAmbience.Stream = loadedTrack;
                AudioAmbience.VolumeDb = -10.0f;
                AudioAmbience.Play();
            }
        }
        AmbSwitching = false;
    }

    public void PlayCredits()
    {
        AudioMusic.ProcessMode = ProcessModeEnum.Always;
        ProcessMode = ProcessModeEnum.Disabled;
        FE.GetNode<FrontendCredits>("FE_Credits").StartCredits();
    }

    public void PlayMenuSound_Back()
    {
        if (SoundFE_Back == null) return;
		AudioMenu.Stream = SoundFE_Back;
		AudioMenu.Play();
    }

    public void PlayMenuSound_Click()
    {
        if (SoundFE_Click == null) return;
		AudioMenu.Stream = SoundFE_Click;
		AudioMenu.Play();
    }

    public void PlayMenuSound_Select()
    {
        if (SoundFE_Select == null) return;
		if (AudioMenu.Playing && AudioMenu.Stream != SoundFE_Select) return;
		AudioMenu.Stream = SoundFE_Select;
		AudioMenu.Play();
    }

    public void ForceGameOver()
    {
        FE.GetNode<FrontendGameOver>("FE_GameOver").Activate();
    }

    public void MainMenu_UpdateViewport()
    {
        FE.GetNode<MainMenuDynamic>("FE_MainMenuDynamic").UpdateViewport();
    }

    public void ConfigSave()
    {
        GetNode<ConfigHandler>("ConfigHandler").UpdateAll();
	    GetNode<ConfigHandler>("ConfigHandler").Save();
    }

    public void StopMusic()
    {
        if (AudioMusic.Playing)
        {
            var musicFader = (MusicFader)AudioMusic;
            musicFader.IsFadingOut = true;
        }
    }

    void FixMobileFE(Node parent)
    {
        // vulkan mobile workaround (some bug with clipping canvas items)
        if (parent is TextureRect tex && tex.Name == "TextureRectMask")
        {
            tex.Visible = false;
        }
        foreach (var i in parent.GetChildren())
        {
            FixMobileFE(i);
        }
    }


}