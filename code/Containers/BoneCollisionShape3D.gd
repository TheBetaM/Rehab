#@tool
extends CollisionShape3D
class_name BoneCollisionShape3D

# This is needed because physics objects can only have collision shapes as children
@export var bone : int = 0
var skeleton : Skeleton3D
var noParent : bool = false

func _ready():
	skeleton = get_parent().get_node("Armature")
	if (bone == 255):
		noParent = true

func _physics_process(delta):
	if !noParent:
		transform = skeleton.get_bone_pose(bone)
