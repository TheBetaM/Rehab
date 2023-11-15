extends Node3D
class_name RehabSceneRoot

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
var Chunks : Array[ChunkScene]
var ChunkNames : Array[StringName]
var ChunkLayers : Array[int]
var LoadingChunkName : String
const MaxChunksLoaded : int = 8 # at the same time
var ActiveMusic : String
var MusicIsChanging : bool
var ActiveAmbience : String
var AmbienceIsChanging : bool

func _init():
	Root = self

func _ready():
	PlayerCam = $PlayerCam
	FreeLookCam = $FreeLookCam
	GameHUD = $FE_HUD
	GameMenu = $FE_Menu
	AudioMusic = $AudioMusic
	AudioAmbience = $AudioAmb
	if (Game.Dev):
		StartLevelSelect()
	else:
		StartLevelSelect()

func LoadScene(path : String):
	UnloadAllChunks()
	LoadingChunkName = path.split("/")[-1].trim_suffix(".tscn")
	PlayerCam.FullReset()
	FreeLookCam.FullReset()
	GameHUD.Clear()
	$Loading.AnimIn()
	$Loading.process_mode = Node.PROCESS_MODE_INHERIT
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
		$Loading.AnimOut()
		await get_tree().create_timer(0.5).timeout
		$Loading.visible = false
		$Loading.process_mode = Node.PROCESS_MODE_DISABLED
		await get_tree().create_timer(3.5).timeout
		if (AgentCharacter.activeCharacter == null):
			printerr("[ROOT] LEVEL LOADED WITH NO CHARACTER")
			ExitLevel()
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
	print("[ROOT] Entering " + chunk.name)
	
	# Updating World Environment and Lights
	$WorldEnv.environment = chunk.WorldEnv
	
	# Updating Skydome
	var skypath : String = RehabGame.AssetsPath + chunk.SkydomePath
	if (SkydomePath != skypath and !chunk.SkydomePath.is_empty()):
		if (Skydome):
			Skydome.queue_free()
		var sky = ResourceLoader.load(skypath)
		SkydomePath = skypath
		Skydome = sky.instantiate()
		add_child(Skydome)
	elif chunk.SkydomePath.is_empty():
		if (Skydome):
			Skydome.queue_free()
		SkydomePath = ""
	
	# Centering active chunk to (0,0,0) and others around it
	var ChunkOffset = chunk.global_position;
	for c in Chunks:
		c.global_position += -ChunkOffset
	PlayerCam.global_position += -ChunkOffset
	
	# Disabling links of chunk that we're exiting
	for i in OldChunk.Links:
		i.DisableLink()
	
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
	chunk.ActiveScene = true
	chunk.visible = true
	chunk.process_mode = Node.PROCESS_MODE_INHERIT
	chunk.OnChunkEnter()
	
	# Activating and starting links of entered chunk
	for i  in chunk.Links:
		if (i.ChunkName == OldChunk.name):
			i.IsBufferred = true
		i.ActivateLink()
	

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

func ExitLevel():
	UnloadAllChunks()
	GameHUD.Clear()
	StartLevelSelect()
	AudioAmbience.stop()
	ActiveAmbience = ""
	AudioMusic.stop()
	ActiveMusic = ""
	$LevelSelect.ResetMenu()

func StartLevelSelect():
	$LevelSelect.visible = true
	$LevelSelect.process_mode = Node.PROCESS_MODE_INHERIT

func StartPauseMenu():
	process_mode = Node.PROCESS_MODE_DISABLED
	GameHUD.ForceAnimOut()
	GameMenu.Start_PauseMenu()

var MusicPaths : Dictionary = {
	0 : "undefined",
	7 : "4_6_Twisted_Docamok",
	8 : "3_4B_Cortex_Amberley",
	9 : "2_8_Embryo_Boss_Fight",
	10 : "1_7_Worm_Chase",
	27 : "1_1_Nsanity_Island",
	28 : "1_2_Cavern_Catastrophe",
	29 : "1_3_Totem_Hokem",
	30 : "1_4_Mechabandicoo",
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
	if (MusicIsChanging):
		return
	var path = ""
	if (MusicPaths.has(id)):
		path = RehabGame.AssetsPath + "Sounds/Music/" + MusicPaths[id] + ".tres"
	if (path == "" or !ResourceLoader.exists(path)):
		return
	if (ActiveMusic == path):
		return
	MusicIsChanging = true
	ActiveMusic = path
	ResourceLoader.load_threaded_request(path)
	var volumeTween = create_tween()
	volumeTween.tween_property(AudioMusic, "volume_db", -30.0, 2.0)
	while ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
		await get_tree().process_frame
	if ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_LOADED:
		var loadedTrack = ResourceLoader.load_threaded_get(path)
		AudioMusic.stream = loadedTrack
		AudioMusic.volume_db = 0.0
		AudioMusic.play()
	MusicIsChanging = false

func PlayAmbience(id: int):
	if (AmbienceIsChanging):
		return
	var path = ""
	if (MusicPaths.has(id)):
		path = RehabGame.AssetsPath + "Sounds/Music/" + MusicPaths[id] + ".tres"
	if (path == "" or !ResourceLoader.exists(path)):
		return
	if (ActiveAmbience == path):
		return
	AmbienceIsChanging = true
	ActiveAmbience = path
	ResourceLoader.load_threaded_request(path)
	var volumeTween = create_tween()
	volumeTween.tween_property(AudioAmbience, "volume_db", -30.0, 2.0)
	while ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
		await get_tree().process_frame
	if ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_LOADED:
		var loadedTrack = ResourceLoader.load_threaded_get(path)
		AudioAmbience.stream = loadedTrack
		AudioAmbience.volume_db = 0.0
		AudioAmbience.play()
	AmbienceIsChanging = false
