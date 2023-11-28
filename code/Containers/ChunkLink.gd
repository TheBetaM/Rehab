# Godot data container class for ChunkLink
extends Node3D
class_name ChunkLink
@export_file("*.tscn") var ChunkPath : String
@export var ChunkName : String
@export var IsDisabled : bool
@export var SpawnInvisible : bool

var ParentScene : ChunkScene
var LoadedScene : PackedScene
var LoadedChunk : Node3D
var EnterTrigger : Area3D
var LoadTriggers : Area3D #todo array
var IsBufferred : bool
#var BufferAgent : Agent

func _ready():
	
	ParentScene = get_parent().get_parent()
	ParentScene.Links.append(self)
	
	if (ChunkPath.is_empty()):
		return
	
	if (get_node_or_null("EnterTrigger") != null and !IsDisabled):
		EnterTrigger = get_node("EnterTrigger")
		EnterTrigger.connect("body_entered", TrigEnter)
		EnterTrigger.connect("body_exited", TrigExit)
	
	if (get_node_or_null("LoadTriggers") != null):
		LoadTriggers = $LoadTriggers/LoadTrigger_0
		LoadTriggers.connect("body_entered", LoadTrigEnter)
		LoadTriggers.connect("body_exited", LoadTrigExit)
	elif (ParentScene.ActiveScene):
		SpawnChunk()
	
	if (!ParentScene.ActiveScene):
		DisableLink()

func DisableLink():
	process_mode = Node.PROCESS_MODE_DISABLED

func ActivateLink():
	if (ChunkPath.is_empty()):
		return
	process_mode = Node.PROCESS_MODE_INHERIT
	if (ParentScene.ActiveScene and LoadedChunk != null):
		LoadedChunk.visible = !SpawnInvisible
		if (SpawnInvisible):
			LoadedChunk.process_mode = Node.PROCESS_MODE_DISABLED
		else:
			LoadedChunk.process_mode = Node.PROCESS_MODE_INHERIT
	if (ParentScene.ActiveScene and LoadTriggers == null):
		SpawnChunk()

func TrigEnter(body):
	if (process_mode == Node.PROCESS_MODE_DISABLED):
		return
	if (LoadedChunk == null):
		return
	if (!ParentScene.ActiveScene):
		return
	if (IsBufferred):
		return
	if (body is CharacterBody3D):
		var agent = body#.get_parent().get_parent().get_parent()
		if (agent is AgentCharacter):
			DisableLink() #todo remove this
			agent.isReparenting = true
			agent.reparent(LoadedChunk)
			agent.ParentScene = LoadedChunk
			agent.UpdateLayers(LoadedChunk.ChunkLayer)
			if (AgentCharacter.activeCharacter == agent):
				SwitchToChunk(LoadedChunk)

func TrigExit(body):
	if (process_mode == Node.PROCESS_MODE_DISABLED):
		return
	IsBufferred = false
	
func LoadTrigEnter(body):
	if (process_mode == Node.PROCESS_MODE_DISABLED):
		return
	if (LoadedChunk != null):
		return
	if (!ParentScene.ActiveScene):
		return
	if (body is CharacterBody3D):
		var agent = body#.get_parent().get_parent().get_parent()
		if (agent is AgentCharacter and AgentCharacter.activeCharacter == agent):
			SpawnChunk()

func LoadTrigExit(body):
	if (process_mode == Node.PROCESS_MODE_DISABLED):
		return
	if (LoadedChunk == null):
		return
	if (!ParentScene.ActiveScene):
		return
	if (body is CharacterBody3D and RehabSceneRoot.Root.PlayerCam.current and !RehabSceneRoot.Root.GameMenu.visible):
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
				printerr("[ChunkLink] FAILED TO LOAD SCENE AT " + FullChunkPath)
		else:
			printerr("[ChunkLink] FAILED TO FIND SCENE AT " + FullChunkPath)
			return
	LoadedChunk = RehabSceneRoot.Root.LoadChunk(LoadedScene, ChunkName, $ChunkHolder)
	if (LoadedChunk != null and SpawnInvisible):
		LoadedChunk.visible = false
		LoadedChunk.process_mode = Node.PROCESS_MODE_DISABLED

func DespawnChunk():
	if (LoadedChunk == null):
		return
	RehabSceneRoot.Root.UnloadChunk(LoadedChunk.name)
	LoadedChunk = null

func SwitchToChunk(chunk):
	RehabSceneRoot.Root.SwitchToChunk(chunk)

