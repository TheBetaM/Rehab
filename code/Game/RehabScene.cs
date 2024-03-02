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
    List<ChunkScene> Chunks = new();
    List<StringName> ChunkNames = new();
    List<int> ChunkLayers = new();
    public string LoadingChunkName;
    const int MaxChunksLoaded = 8; // at the same time
    public string ActiveMusic;
    public string ActiveAmbience;
    public AudioStream SoundFE_Back;
    public AudioStream SoundFE_Click;
    public AudioStream SoundFE_Select;
    bool MusicSwitching;
    bool AmbSwitching;

    public RehabScene()
    {
        Root = this;
    }

    public override void _Ready()
    {
        PlayerCam = GetNode<PlayerCamera>("PlayerCam");
        FreeLookCam = GetNode<FreeLookCamera>("FreeLookCam");
        GameHUD = GetNode<FrontendHUD>("FE/FE_HUD");
        GameMenu = GetNode<FrontendMenu>("FE/FE_Menu");
        AudioMusic = GetNode<AudioStreamPlayer>("Audio/AudioMusic1");
        AudioAmbience = GetNode<AudioStreamPlayer>("Audio/AudioAmb1");
        AudioMenu = GetNode<AudioStreamPlayer>("Audio/AudioMenu");
        var env = GetNode<WorldEnvironment>("WorldEnv");
        env.Environment = DefaultEnv;
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
            GetNode<LevelSelectList>("FE/LevelSelect").Generate();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            GameHUD.Setup();
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
            //StartMessage("#FE-NoGameData");
            //await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
            //while (GameMenu.Visible)
            //    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            //GetTree().Quit();
            GetNode<FrontendInstaller>("FE/FE_Installer").Activate();
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
            GD.Print("[ROOT] Packs directory failed to open! " + PacksPath);
        }
    }

    public async void LoadScene(string path)
    {
        UnloadAllChunks();
        LoadingChunkName = path.Split("/").Last().Replace(".tscn","");
        PlayerCam.FullReset();
        FreeLookCam.FullReset();
        GameHUD.Clear();
        GetNode<LoadingVisuals>("FE/Loading").AnimIn();
        GetNode<LoadingVisuals>("FE/Loading").ProcessMode = ProcessModeEnum.Inherit;
        ResourceLoader.LoadThreadedRequest(path);
        while (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.InProgress)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        if (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            var loadedPack = (PackedScene)ResourceLoader.LoadThreadedGet(path);
            var loadedScene = (ChunkScene)loadedPack.Instantiate();
            loadedScene.ActiveScene = true;
            loadedScene.ProcessMode = ProcessModeEnum.Disabled;
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
                AddChild(Skydome);
            }
            await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
            loadedScene.ProcessMode = ProcessModeEnum.Inherit;
            loadedScene.OnChunkEnter();
            loadedScene.ShadowToggle(true);
            GetNode<LoadingVisuals>("FE/Loading").AnimOut();
            if (RehabGame.UseMouseCamera)
                Input.MouseMode = Input.MouseModeEnum.Captured;
            await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
            GetNode<LoadingVisuals>("FE/Loading").Visible = false;
            GetNode<LoadingVisuals>("FE/Loading").ProcessMode = ProcessModeEnum.Disabled;
            GameHUD.OnUnPause();
            await ToSignal(GetTree().CreateTimer(3.5f), SceneTreeTimer.SignalName.Timeout);
            if (AgentCharacter.activeCharacter == null && !GetNode<Control>("FE/LevelSelect").Visible &&
             !GetNode<Control>("FE/Loading").Visible && !GetNode<Control>("FE/FE_MainMenuDynamic").Visible && 
             !GetNode<Control>("FE/FE_Credits").Visible)
            {
                GD.PrintErr("[ROOT] LEVEL LOADED WITH NO CHARACTER");
                StartMessage("#FE-NoActorError");
                await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
                while (GameMenu.Visible)
                    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                ExitLevel(false);
            }
        }
        else
        {
            GD.PrintErr("[ROOT] FAILED TO LOAD SCENE AT " + path);
        } 
    }

    public Node3D LoadChunk(PackedScene chunk, string chunkName, Node3D holder)
    {
        if (ChunkNames.Contains(chunkName)) return null;
        GD.Print("[ROOT] Spawning " + chunkName);
		var LoadedChunk = chunk.Instantiate();
		holder.AddChild(LoadedChunk);
		LoadedChunk.Reparent(this);
        var scene = (ChunkScene)LoadedChunk;
		Chunks.Add(scene);
		ChunkNames.Add(chunkName);
        for (int i = 1; i <= MaxChunksLoaded; i++)
        {
            if (!ChunkLayers.Contains(i))
            {
                ChunkLayers.Add(i);
				scene.UpdateLayers(i);
                break;
            }
        }
		return scene;
    }

    public void SwitchToChunk(ChunkScene chunk)
    {
        if (chunk == ActiveChunk) return;
        
        var OldChunk = ActiveChunk;
        OldChunk.ActiveScene = false;
        OldChunk.OnChunkExit();
        OldChunk.ShadowToggle(false);
        chunk.ActiveScene = true;
        GD.Print("[ROOT] Entering " + chunk.Name);
        
        // Updating World Environment and Lights
        chunk.WorldEnv.TonemapMode = Environment.ToneMapper.Reinhardt;
        chunk.ShadowToggle(true);
        GetNode<WorldEnvironment>("WorldEnv").Environment = chunk.WorldEnv;
        
        // Updating Skydome
        string skypath = RehabGame.AssetsPath + chunk.SkydomePath;
        if (SkydomePath != skypath && !string.IsNullOrWhiteSpace(chunk.SkydomePath))
        {
            if (!string.IsNullOrWhiteSpace(SkydomePath))
                Skydome.QueueFree();
            var sky = (PackedScene)ResourceLoader.Load(skypath);
            SkydomePath = skypath;
            Skydome = (Node3D)sky.Instantiate();
            AddChild(Skydome);
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
        PlayerCam.pivot += -ChunkOffset;
        PlayerCam.GlobalPosition += -ChunkOffset;
        
        // Disabling links of chunk that we're exiting
        foreach (var i in OldChunk.Links)
            i.DisableLink();
        
        // Activating and starting links of entered chunk
        foreach (var i in chunk.Links)
        {
            if (i.ChunkName == OldChunk.Name)
            {
                i.IsBuffered = true;
                i.LoadedChunk = OldChunk;
                i.LoadedChunk.Position = i.GetNode<Node3D>("ChunkHolder").GlobalPosition;
                i.LoadedChunk.Rotation = i.GetNode<Node3D>("ChunkHolder").GlobalRotation;
            }
            foreach (var a in OldChunk.Links)
            {
                if (a.ChunkName == i.ChunkName && a.LoadedChunk != null)
                {
                    i.LoadedChunk = a.LoadedChunk;
                    i.LoadedChunk.Position = i.GetNode<Node3D>("ChunkHolder").GlobalPosition;
                    i.LoadedChunk.Rotation = i.GetNode<Node3D>("ChunkHolder").GlobalRotation;
                }
            }
            i.ActivateLink();
        }
        
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
                UnloadChunk(cn);
        }
        
        ActiveChunk = chunk;
        chunk.Visible = true;
        chunk.ProcessMode = ProcessModeEnum.Inherit;
        chunk.OnChunkEnter();
    }

    public void UnloadChunk(string chunk)
    {
        var pos = ChunkNames.IndexOf(chunk);
        if (pos == -1) return;
        GD.Print("[ROOT] Unloading " + chunk);
        Chunks[pos].QueueFree();
        Chunks.RemoveAt(pos);
        ChunkNames.RemoveAt(pos);
        ChunkLayers.RemoveAt(pos);
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
        if (!toMain)
        {
            StartLevelSelect();
            GetNode<LevelSelectList>("FE/LevelSelect").ResetMenu();
        }
        else
        {
            StartMainMenu();
        }
    }

    public void StartLevelSelect()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
	    GetNode<LevelSelectList>("FE/LevelSelect").Activate();
    }

    public void StartMainMenu()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
	    GetNode<MainMenuDynamic>("FE/FE_MainMenuDynamic").Activate();
    }

    public void StartPauseMenu(bool optionsOnly)
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GameHUD.OnPause();
        GameMenu.Start_PauseMenu(optionsOnly);
    }

    void StartMessage(string text)
    {
        ProcessMode = ProcessModeEnum.Disabled;
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
        ResourceLoader.LoadThreadedRequest(path);
        while (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.InProgress)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        if (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            var loadedTrack = ResourceLoader.LoadThreadedGet(path);
            AudioAmbience.Stream = (AudioStream)loadedTrack;
            AudioAmbience.VolumeDb = -10.0f;
            AudioAmbience.Play();
        }
        AmbSwitching = false;
    }

    public void PlayCredits()
    {
        AudioMusic.ProcessMode = ProcessModeEnum.Always;
        ProcessMode = ProcessModeEnum.Disabled;
        GetNode<FrontendCredits>("FE/FE_Credits").StartCredits();
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
        GetNode<FrontendGameOver>("FE/FE_GameOver").Activate();
    }

    public void MainMenu_UpdateViewport()
    {
        GetNode<MainMenuDynamic>("FE/FE_MainMenuDynamic").UpdateViewport();
    }

    public void ConfigSave()
    {
        GetNode<ConfigHandler>("ConfigHandler").UpdateAll();
	    GetNode<ConfigHandler>("ConfigHandler").Save();
    }



}