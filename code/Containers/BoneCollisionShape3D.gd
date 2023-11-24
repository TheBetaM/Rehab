#@tool
extends CollisionShape3D
class_name BoneCollisionShape3D

# This is needed because physics objects can only have collision shapes as children
@export var bone : int = 0
var skeleton : Skeleton3D
var noParent : bool = false
#var offset : Vector3 = Vector3.ZERO

func _ready():
	#offset = position
	skeleton = get_parent().get_node("Armature")
	if (bone == 255):
		bone = 0

func _physics_process(delta):
	transform = skeleton.get_bone_pose(bone)
	#translate(offset)
