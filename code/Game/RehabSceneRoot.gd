extends Node3D
class_name RehabSceneRoot

@export var DefaultEnv : Environment
var ActiveChunk : ChunkScene
var Skydome : Node3D
var SkydomePath : String
static var Game : RehabGame = RehabGame.new()
static var Root : RehabSceneRoot
static var PlayerCam : Camera3D
static var FreeLookCam : Camera3D
static var GameHUD : Control
static var GameMenu : Control
static var AudioMusic : AudioStreamPlayer
static var AudioAmbience : AudioStreamPlayer
static var AudioMenu : AudioStreamPlayer
static var AudioVoice : AudioStreamPlayer
var Chunks : Array[ChunkScene]
var ChunkNames : Array[StringName]
var ChunkLayers : Array[int]
var LoadingChunkName : String
const MaxChunksLoaded : int = 8 # at the same time
var ActiveMusic : String
var ActiveAmbience : String
var SoundFE_Back : AudioStream
var SoundFE_Click : AudioStream
var SoundFE_Select : AudioStream
var MusicSwitching : bool
var AmbSwitching : bool

func _init():
	Root = self

func _ready():
	PlayerCam = $PlayerCam
	FreeLookCam = $FreeLookCam
	GameHUD = $FE/FE_HUD
	GameMenu = $FE/FE_Menu
	AudioMusic = $Audio/AudioMusic1
	AudioAmbience = $Audio/AudioAmb1
	AudioMenu = $Audio/AudioMenu
	$WorldEnv.environment = DefaultEnv
	$ConfigHandler.Load()
	$ConfigHandler.Setup()
	GameInit()

func GameInit():
	await get_tree().process_frame
	LoadPacks()
	await get_tree().process_frame
	
	if (ResourceLoader.exists(RehabGame.AssetsPath + "Sounds/Menu/FE_BACK.res")): SoundFE_Back = load(RehabGame.AssetsPath + "Sounds/Menu/FE_BACK.res")
	if (ResourceLoader.exists(RehabGame.AssetsPath + "Sounds/Menu/FE_CLICK.res")): SoundFE_Click = load(RehabGame.AssetsPath + "Sounds/Menu/FE_CLICK.res")
	if (ResourceLoader.exists(RehabGame.AssetsPath + "Sounds/Menu/FE_SELECT.res")): SoundFE_Select = load(RehabGame.AssetsPath + "Sounds/Menu/FE_SELECT.res")
	
	var dir = DirAccess.open(RehabGame.AssetsPath + "Levels/");
	if dir:
		$FE/LevelSelect.Generate()
		await get_tree().process_frame
		await get_tree().process_frame
		if (Game.Dev):
			StartLevelSelect()
		else:
			StartMessage("#FE-Explorer-Disclaimer-" + str(randi_range(0, 10)))
			await get_tree().create_timer(0.5).timeout
			while (GameMenu.visible):
				await get_tree().process_frame
			StartMainMenu()
	else:
		print("[ROOT] Cannot open " + RehabGame.AssetsPath + "Levels/")
		StartMessage("#FE-NoGameData")
		await get_tree().create_timer(0.5).timeout
		while (GameMenu.visible):
			await get_tree().process_frame
		get_tree().quit()

func LoadPacks():
	var PacksPathSplit = OS.get_executable_path().split("/")
	var PacksPath = ""
	var PathID = 0
	for i in PacksPathSplit:
		PathID += 1
		if (PathID < PacksPathSplit.size()):
			PacksPath += i
			PacksPath += "/"
	PacksPath += "Packs/"
	
	if (!DirAccess.dir_exists_absolute(PacksPath)):
		DirAccess.make_dir_absolute(PacksPath)
	
	var pdir = DirAccess.open(PacksPath)
	if (pdir):
		for i in pdir.get_files():
			var success = ProjectSettings.load_resource_pack(PacksPath + i)
			if (success):
				print("[ROOT] Pack loaded from " + i)
			else:
				printerr("[ROOT] Pack FAILED from " + i)
	else:
		print("[ROOT] Packs directory failed to open!")

func LoadScene(path : String):
	UnloadAllChunks()
	LoadingChunkName = path.split("/")[-1].trim_suffix(".tscn")
	PlayerCam.FullReset()
	FreeLookCam.FullReset()
	GameHUD.Clear()
	$FE/Loading.AnimIn()
	$FE/Loading.process_mode = Node.PROCESS_MODE_INHERIT
	ResourceLoader.load_threaded_request(path)
	while ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
		await get_tree().process_frame
	if ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_LOADED:
		var loadedPack = ResourceLoader.load_threaded_get(path)
		var loadedScene = loadedPack.instantiate()
		loadedScene.ActiveScene = true
		loadedScene.process_mode = Node.PROCESS_MODE_DISABLED
		ActiveChunk = loadedScene
		Chunks.append(loadedScene)
		ChunkNames.append(loadedScene.name)
		for i in range(1, MaxChunksLoaded):
			if (!ChunkLayers.has(i)):
				ChunkLayers.append(i)
				loadedScene.UpdateLayers(i)
				break
		loadedScene.WorldEnv.tonemap_mode = Environment.TONE_MAPPER_REINHARDT
		$WorldEnv.environment = loadedScene.WorldEnv
		add_child(loadedScene)
		var skypath : String = RehabGame.AssetsPath + loadedScene.SkydomePath
		if (!loadedScene.SkydomePath.is_empty()):
			var sky = ResourceLoader.load(skypath)
			SkydomePath = skypath
			Skydome = sky.instantiate()
			add_child(Skydome)
		await get_tree().create_timer(1.0).timeout
		loadedScene.process_mode = Node.PROCESS_MODE_INHERIT
		loadedScene.OnChunkEnter()
		$FE/Loading.AnimOut()
		if (RehabGame.UseMouseCamera):
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
		await get_tree().create_timer(0.5).timeout
		$FE/Loading.visible = false
		$FE/Loading.process_mode = Node.PROCESS_MODE_DISABLED
		await get_tree().create_timer(3.5).timeout
		if (AgentCharacter.activeCharacter == null and !$FE/LevelSelect.visible and !$FE/Loading.visible and !$FE/FE_MainMenuDynamic.visible and !$FE/FE_Credits.visible):
			printerr("[ROOT] LEVEL LOADED WITH NO CHARACTER")
			ExitLevel(false)
	else:
		printerr("[ROOT] FAILED TO LOAD SCENE AT " + path)

func LoadChunk(chunk : PackedScene, chunkName : String, holder : Node3D):
	if (ChunkNames.find(chunkName) == -1):
		print("[ROOT] Spawning " + chunkName)
		var LoadedChunk = chunk.instantiate()
		holder.add_child(LoadedChunk)
		LoadedChunk.reparent(self)
		Chunks.append(LoadedChunk)
		ChunkNames.append(chunkName)
		for i in range(1, MaxChunksLoaded):
			if (!ChunkLayers.has(i)):
				ChunkLayers.append(i)
				LoadedChunk.UpdateLayers(i)
				break
		return LoadedChunk
	else:
		return null

func SwitchToChunk(chunk : ChunkScene):
	
	if (chunk == ActiveChunk):
		return
	
	var OldChunk = ActiveChunk
	OldChunk.ActiveScene = false
	OldChunk.OnChunkExit()
	OldChunk.ShadowToggle(false)
	chunk.ActiveScene = true
	print("[ROOT] Entering " + chunk.name)
	
	# Updating World Environment and Lights
	chunk.WorldEnv.tonemap_mode = Environment.TONE_MAPPER_REINHARDT
	chunk.ShadowToggle(true)
	$WorldEnv.environment = chunk.WorldEnv
	
	# Updating Skydome
	var skypath : String = RehabGame.AssetsPath + chunk.SkydomePath
	if (SkydomePath != skypath and !chunk.SkydomePath.is_empty()):
		if (Skydome and SkydomePath != ""):
			Skydome.queue_free()
		var sky = ResourceLoader.load(skypath)
		SkydomePath = skypath
		Skydome = sky.instantiate()
		add_child(Skydome)
	elif chunk.SkydomePath.is_empty():
		if (Skydome and SkydomePath != ""):
			Skydome.queue_free()
		SkydomePath = ""
	
	# Centering active chunk to (0,0,0) and others around it
	var ChunkOffset = chunk.global_position;
	for c in Chunks:
		c.global_position += -ChunkOffset
	PlayerCam.pivot += -ChunkOffset
	PlayerCam.global_position += -ChunkOffset
	
	# Disabling links of chunk that we're exiting
	for i in OldChunk.Links:
		i.DisableLink()
	
	# Activating and starting links of entered chunk
	for i in chunk.Links:
		if (i.ChunkName == OldChunk.name):
			i.IsBufferred = true
			i.LoadedChunk = OldChunk
			i.LoadedChunk.position = i.get_node("ChunkHolder").global_position
			i.LoadedChunk.rotation = i.get_node("ChunkHolder").global_rotation
		for a in OldChunk.Links:
			if (a.ChunkName == i.ChunkName and a.LoadedChunk != null):
				i.LoadedChunk = a.LoadedChunk
				i.LoadedChunk.position = i.get_node("ChunkHolder").global_position
				i.LoadedChunk.rotation = i.get_node("ChunkHolder").global_rotation
		i.ActivateLink()
	
	# Disposing of unlinked chunks
	for cn in ChunkNames:
		var found = false
		for c in chunk.Links:
			if (c.ChunkName == cn):
				found = true
				break
		if (!found and cn != chunk.name):
			UnloadChunk(cn)
	
	ActiveChunk = chunk
	chunk.visible = true
	chunk.process_mode = Node.PROCESS_MODE_INHERIT
	chunk.OnChunkEnter()

func UnloadChunk(chunk : String):
	var pos = ChunkNames.find(chunk)
	if (pos != -1):
		print("[ROOT] Unloading " + chunk)
		Chunks[pos].queue_free()
		Chunks.remove_at(pos)
		ChunkNames.remove_at(pos)
		ChunkLayers.remove_at(pos)
		return

func UnloadAllChunks():
	for i in range(Chunks.size()):
		Chunks[i].queue_free()
	Chunks.clear()
	ChunkNames.clear()
	ChunkLayers.clear()
	AgentCharacter.ActiveActorTypes.clear()
	AgentCharacter.activeCharacter = null
	if (Skydome != null):
		Skydome.queue_free()
	SkydomePath = ""

func ExitLevel(toMain : bool):
	UnloadAllChunks()
	GameHUD.Clear()
	AudioAmbience.stop()
	ActiveAmbience = ""
	AudioMusic.stop()
	ActiveMusic = ""
	$WorldEnv.environment = DefaultEnv
	if (!toMain):
		StartLevelSelect()
		$FE/LevelSelect.ResetMenu()
	else:
		StartMainMenu()

func StartLevelSelect():
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	$FE/LevelSelect.Activate()

func StartMainMenu():
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	$FE/FE_MainMenuDynamic.Activate()

func StartPauseMenu(optionsOnly : bool):
	process_mode = Node.PROCESS_MODE_DISABLED
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	GameHUD.ForceAnimOut()
	GameMenu.Start_PauseMenu(optionsOnly)

func StartMessage(text : String):
	process_mode = Node.PROCESS_MODE_DISABLED
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	GameHUD.ForceAnimOut()
	GameMenu.Start_Message(text)

var MusicPaths : Dictionary = {
	0 : "undefined",
	7 : "4_6_Twisted_Docamok",
	8 : "3_4B_Cortex_Amberley",
	9 : "2_8_Embryo_Boss_Fight",
	10 : "1_7_Worm_Chase",
	27 : "1_1_Nsanity_Island",
	28 : "1_2_Cavern_Catastrophe",
	29 : "1_3_Totem_Hokem",
	30 : "1_4_Mechabandicoot",
	31 : "1_5_River_Boat_section",
	32 : "1_6_Totem_God_Boss_Fight",
	33 : "2_1_Ice_Lab_MT",
	34 : "2_2_Ice_Climb",
	35 : "2_3_Uka_Uka_Ice_Creature",
	36 : "2_5_Boat_Chase",
	37 : "3_1_Madame_Amberly_School",
	38 : "3_1_Madame_Amberly_nolaugh",
	40 : "3_3_Dingodile_Mini_Boss",
	41 : "3_5_Rooftop_Rampage",
	53 : "2_1A_Ice_Lab_MT_FASTER",
	54 : "2_4_Humiliskate",
	55 : "2_6_Ngin_Mini_Boss_Fight",
	56 : "2_7_Henchmania",
	57 : "3_2_Broiler_Room_Doom_2",
	58 : "3_4A_Crash_Amberley",
	59 : "3_6_Amberly_Boss_Fight",
	60 : "4_0_Level_4_Hub",
	61 : "4_1_Rockslide_Rumble",
	62 : "4_2_Twisted_Insanity",
	63 : "4_3_Twins_Compound",
	64 : "4_4_Twins_Boss_Fight",
	77 : "LO7_boiler_fan_room_bg_stereo",
	78 : "LO7_boiler_main_room_bg_stereo",
	79 : "LO7_boiler_met_spin_rm_bg_stereo",
	80 : "LO7_boiler_vent_room_bg_stereo",
	89 : "ocean_waves_stereo",
	90 : "Jungle_Ambience_Stereo_1",
	91 : "LO8_outside_ambience_stereo",
	92 : "LO8_nitro_flood_loop_stereo",
	103 : "L12_lava_pool_bg_stereo",
	104 : "L12_lava_cave_bg_stereo",
	105 : "L12_hanger_bg_stereo",
	106 : "LO7_tunnel_bg_stereo",
	107 : "LO7_Newboiler_main_room_bg_stereo",
	108 : "LO7_Newboiler_met_spin_rm_bg_stereo",
	109 : "LO7_Newboiler_vent_room_bg_stereo",
	110 : "LO7_Newboiler_fan_room_bg_stereo",
	111 : "L09_Amb_Rooftops_Stereo",
	112 : "H02_Amb_ColdWind_Stereo",
	113 : "H04_Amb_Wind_Stereo",
	114 : "L01_Amb_Cave_Stereo",
	115 : "L02_Amb_Cavern_Stereo",
	116 : "L04_Amb_IceCavern_Stereo",
	117 : "L06_Amb_CrowsNest_Stere",
	118 : "L06_Amb_Henchmania_Stereo",
	119 : "L06_Amb_ShipInt_Stereo",
	120 : "L07_Amb_FanRoom_Stereo",
	121 : "L07_Amb_MainRoom_Stereo",
	122 : "L07_Amb_MetRoom_Stereo",
	123 : "L07_Amb_VentRoom_Stereo",
	124 : "Gen_Amb_Airship_Int_Stereo",
	136 : "1_8NativeChase",
	137 : "LO8_UProomtone_stereo",
	139 : "L05_Chickens",
	140 : "B01_MechEndfall",
}

func PlayMusic(id : int):
	if (MusicSwitching):
		return;
	var path = ""
	if (MusicPaths.has(id)):
		path = RehabGame.AssetsPath + "Sounds/Music/" + MusicPaths[id] + ".res"
	if (path == "" or !ResourceLoader.exists(path)):
		return
	if (ActiveMusic == path):
		return
	ActiveMusic = path
	MusicSwitching = true
	if (AudioMusic.playing):
		AudioMusic.IsFadingOut = true
		if (AudioMusic == $Audio/AudioMusic1):
			AudioMusic = $Audio/AudioMusic2
		else:
			AudioMusic = $Audio/AudioMusic1
		AudioMusic.IsFadingOut = false
	ResourceLoader.load_threaded_request(path)
	while ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
		await get_tree().process_frame
	if ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_LOADED:
		var loadedTrack = ResourceLoader.load_threaded_get(path)
		AudioMusic.stream = loadedTrack
		AudioMusic.volume_db = 0.0
		AudioMusic.play()
	MusicSwitching = false

func PlayAmbience(id: int):
	if (AmbSwitching):
		return;
	var path = ""
	if (MusicPaths.has(id)):
		path = RehabGame.AssetsPath + "Sounds/Music/" + MusicPaths[id] + ".res"
	if (path == "" or !ResourceLoader.exists(path)):
		return
	if (ActiveAmbience == path):
		return
	ActiveAmbience = path
	AmbSwitching = true
	if (AudioAmbience.playing):
		AudioAmbience.IsFadingOut = true
		if (AudioAmbience == $Audio/AudioAmb1):
			AudioAmbience = $Audio/AudioAmb2
		else:
			AudioAmbience = $Audio/AudioAmb1
		AudioAmbience.IsFadingOut = false
	ResourceLoader.load_threaded_request(path)
	while ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
		await get_tree().process_frame
	if ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_LOADED:
		var loadedTrack = ResourceLoader.load_threaded_get(path)
		AudioAmbience.stream = loadedTrack
		AudioAmbience.volume_db = -10.0
		AudioAmbience.play()
	AmbSwitching = false

func PlayCredits():
	AudioMusic.process_mode = Node.PROCESS_MODE_ALWAYS
	process_mode = Node.PROCESS_MODE_DISABLED
	$FE/FE_Credits.StartCredits()

func PlayMenuSound_Back():
	if (SoundFE_Back):
		AudioMenu.stream = SoundFE_Back
		AudioMenu.play()

func PlayMenuSound_Click():
	if (SoundFE_Click):
		AudioMenu.stream = SoundFE_Click
		AudioMenu.play()

func PlayMenuSound_Select():
	if (SoundFE_Select):
		if (AudioMenu.playing and AudioMenu.stream != SoundFE_Select):
			return
		AudioMenu.stream = SoundFE_Select
		AudioMenu.play()

func ForceGameOver():
	$FE/FE_GameOver.Activate()

func MainMenu_UpdateViewport():
	$FE/FE_MainMenuDynamic.UpdateViewport()

func ConfigSave():
	$ConfigHandler.UpdateAll()
	$ConfigHandler.Save()
