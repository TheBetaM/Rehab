#@tool
extends Decal
class_name DecalShadow

# This is needed for shadows to follow the right model
@export var bone : int = 0
@export var model_id : int = 0
var skeleton : Skeleton3D
var parentNode : Node3D

func _ready():
	parentNode = get_parent_node_3d()
	skeleton = get_parent().get_parent().get_node("Models").get_child(model_id).get_node("RigidBody").get_node("Armature")
	if (bone == 255):
		bone = 0

func _physics_process(delta):
	if (!visible or !parentNode.visible): 
		pass;
	# transform = skeleton.get_bone_pose(bone)
