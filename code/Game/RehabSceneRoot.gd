extends Node3D
class_name RehabSceneRoot

var ActiveChunk : ChunkScene
var Skydome : Node3D
var SkydomePath : String
static var Game : RehabGame = RehabGame.new()
static var Root : RehabSceneRoot
var Chunks : Array[ChunkScene]
var ChunkNames : Array[StringName]
var ChunkLayers : Array[int]
var LoadingChunkName : String

func _init():
	Root = self

func _ready():
	$LevelSelect.visible = true
	$LevelSelect.process_mode = Node.PROCESS_MODE_INHERIT

func LoadScene(path : String):
	UnloadAllChunks()
	LoadingChunkName = path.split("/")[-1].trim_suffix(".tscn")
	$Loading.UpdateVisuals()
	$Loading.visible = true
	$Loading.process_mode = Node.PROCESS_MODE_INHERIT
	ResourceLoader.load_threaded_request(path)
	while ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
		await get_tree().process_frame
	if ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_LOADED:
		var loadedPack = ResourceLoader.load_threaded_get(path)
		var loadedScene = loadedPack.instantiate()
		loadedScene.ActiveScene = true
		ActiveChunk = loadedScene
		Chunks.append(loadedScene)
		ChunkNames.append(loadedScene.name)
		for i in range(1, 20):
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
		$Loading.visible = false
		$Loading.process_mode = Node.PROCESS_MODE_DISABLED
		await get_tree().create_timer(5.0).timeout
		if (AgentCharacter.activeCharacter == null):
			printerr("LEVEL LOADED WITH NO CHARACTER")
			ExitLevel()
	else:
		printerr("FAILED TO LOAD SCENE AT " + path)

func LoadChunk(chunk : PackedScene, chunkName : String, holder : Node3D):
	if (ChunkNames.find(chunkName) == -1):
		print("[ROOT] Spawning " + chunkName)
		var LoadedChunk = chunk.instantiate()
		holder.add_child(LoadedChunk)
		LoadedChunk.reparent(self)
		Chunks.append(LoadedChunk)
		ChunkNames.append(chunkName)
		for i in range(1, 20):
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
	
	# Disabling links of chunk that we're exiting
	for i  in OldChunk.Links:
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
	$LevelSelect.visible = true
	$LevelSelect.process_mode = Node.PROCESS_MODE_INHERIT
	$LevelSelect.ResetMenu()
