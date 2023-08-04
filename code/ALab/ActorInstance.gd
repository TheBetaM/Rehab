# Godot data container class for Instance
@tool
extends Marker3D
class_name ActorInstance
@export var Prefab : PackedScene
@export var InstanceScript : ALabScript
@export var RefList: int = -1
@export var LinkInstance : Array[NodePath]
@export var LinkPath : Array[NodePath]
@export var LinkPoint : Array[NodePath]
@export var RegAngle : Array[int]
@export var RegFloat : Array[float]
@export var RegInt : Array[int]
var Actor : Agent

func _ready():
	#Preventing load stutter
	for i in randi_range(1, 30):
		await get_tree().process_frame
	
	var act = Prefab.instantiate()
	if (act is Agent):
		Actor = act
	else:
		add_child(act)
		return
	Actor.InstanceScript = InstanceScript
	Actor.RefList = RefList
	Actor.RegAngle = RegAngle
	Actor.RegFloat = RegFloat
	Actor.RegInt = RegInt
	#for i in LinkInstance:
	#	Actor.LinkInstance.append(get_node_or_null(i))
	#for i in LinkPath:
	#	Actor.LinkPath.append(get_node_or_null(i))
	#for i in LinkPoint:
	#	Actor.LinkPoint.append(get_node_or_null(i))
	add_child(Actor)
	if (Engine.is_editor_hint()):
		Actor.set("metadata/_edit_lock_", true)
