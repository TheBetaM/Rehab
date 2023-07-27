extends Node3D
class_name RehabSceneRoot

var ActiveChunk : ChunkScene
var Skydome : Node3D
var SkydomePath : String
static var Game : RehabGame = RehabGame.new()
static var Root : RehabSceneRoot
var Chunks : Array[ChunkScene]
var ChunkNames : Array[StringName]

func _init():
	Root = self

func _ready():
	$LevelSelect.visible = true
	$LevelSelect.process_mode = Node.PROCESS_MODE_INHERIT

func LoadScene(path : String):
	UnloadAllChunks()
	$Loading.visible = true
	$Loading.process_mode = Node.PROCESS_MODE_INHERIT
	ResourceLoader.load_threaded_request(path)
	while ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
		await get_tree().process_frame
	if ResourceLoader.load_threaded_get_status(path) == ResourceLoader.THREAD_LOAD_LOADED:
		var loadedPack = ResourceLoader.load_threaded_get(path)
		var loadedScene = loadedPack.instantiate()
		loadedScene.ActiveScene = true
		$WorldEnv.environment = loadedScene.WorldEnv
		add_child(loadedScene)
		Chunks.append(loadedScene)
		ChunkNames.append(loadedScene.name)
		var skypath : String = RehabGame.AssetsPath + loadedScene.SkydomePath
		if (!skypath.is_empty()):
			var sky = ResourceLoader.load(skypath)
			SkydomePath = skypath
			Skydome = sky.instantiate()
			add_child(Skydome)
		$Loading.visible = false
		$Loading.process_mode = Node.PROCESS_MODE_DISABLED
		await get_tree().process_frame
		await get_tree().process_frame
		await get_tree().process_frame
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
		if (LoadedChunk.get_node_or_null("Lights") != null):
			LoadedChunk.get_node("Lights").visible = false
		#todo: disable collision?
		Chunks.append(LoadedChunk)
		ChunkNames.append(chunkName)
		return LoadedChunk
	else:
		return null

func UnloadChunk(chunk : String):
	var pos = ChunkNames.find(chunk)
	if (pos != -1):
		print("[ROOT] Unloading " + chunk)
		Chunks[pos].queue_free()
		Chunks.remove_at(pos)
		ChunkNames.remove_at(pos)
		return

func UnloadAllChunks():
	for i in range(Chunks.size()):
		Chunks[i].queue_free()
	for i in range(Chunks.size()):
		Chunks.remove_at(i)
		ChunkNames.remove_at(i)
	Chunks.clear()
	ChunkNames.clear()
	if (Skydome != null):
		Skydome.queue_free()

func ExitLevel():
	UnloadAllChunks()
	$LevelSelect.visible = true
	$LevelSelect.process_mode = Node.PROCESS_MODE_INHERIT
	$LevelSelect.ResetMenu()
