# Godot data container class for ChunkLink
extends Node3D
class_name ChunkLink
@export_file("*.tscn") var ChunkPath : String
@export var ChunkName : String
@export var Type : int
@export var Flags : int

var ParentScene : ChunkScene
var LoadedScene : PackedScene
var LoadedChunk : Node3D
var EnterTrigger : Area3D
var LoadTriggers : Area3D #todo array
#todo load area : cases where the load area only loads and not instantiates

func _ready():
	
	ParentScene = get_parent().get_parent()
	
	if (ChunkPath.is_empty()):
		return
	
	if (get_node_or_null("EnterTrigger") != null):
		EnterTrigger = get_node("EnterTrigger")
		EnterTrigger.connect("body_entered", TrigEnter)
		EnterTrigger.connect("body_exited", TrigExit)
	
	if (get_node_or_null("LoadTriggers") != null):
		LoadTriggers = $LoadTriggers/LoadTrigger_0
		LoadTriggers.connect("body_entered", LoadTrigEnter)
		LoadTriggers.connect("body_exited", LoadTrigExit)
	elif (ParentScene.ActiveScene):
		SpawnChunk()

func TrigEnter(body):
	if (LoadedChunk == null):
		return
	if (!ParentScene.ActiveScene):
		return
	if (body is CharacterBody3D):
		var agent = body.get_parent().get_parent().get_parent()
		if (agent is AgentCharacter and AgentCharacter.activeCharacter == agent):
			pass

func TrigExit(body):
	pass
	
func LoadTrigEnter(body):
	if (LoadedChunk != null):
		return
	if (!ParentScene.ActiveScene):
		return
	if (body is CharacterBody3D):
		var agent = body.get_parent().get_parent().get_parent()
		if (agent is AgentCharacter and AgentCharacter.activeCharacter == agent):
			SpawnChunk()

func LoadTrigExit(body):
	if (LoadedChunk == null):
		return
	if (!ParentScene.ActiveScene):
		return
	if (body is CharacterBody3D):
		var agent = body.get_parent().get_parent().get_parent()
		if (agent is AgentCharacter and AgentCharacter.activeCharacter == agent):
			DespawnChunk()

func SpawnChunk():
	if (LoadedChunk != null):
		return
	if (LoadedScene == null):
		var FullChunkPath = RehabGame.AssetsPath + ChunkPath
		if (FileAccess.file_exists(FullChunkPath)):
			ResourceLoader.load_threaded_request(FullChunkPath)
			while ResourceLoader.load_threaded_get_status(FullChunkPath) == ResourceLoader.THREAD_LOAD_IN_PROGRESS:
				await get_tree().process_frame
			if ResourceLoader.load_threaded_get_status(FullChunkPath) == ResourceLoader.THREAD_LOAD_LOADED:
				LoadedScene = ResourceLoader.load_threaded_get(FullChunkPath)
			else:
				printerr("FAILED TO LOAD SCENE AT " + FullChunkPath)
		else:
			printerr("FAILED TO FIND SCENE AT " + FullChunkPath)
			return
	LoadedChunk = RehabSceneRoot.Root.LoadChunk(LoadedScene, ChunkName, $ChunkHolder)

func DespawnChunk():
	if (LoadedChunk == null):
		return
	RehabSceneRoot.Root.UnloadChunk(LoadedChunk.name)
	LoadedChunk = null

