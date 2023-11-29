extends Agent
class_name AgentPickup

var isPickedUp : bool = false
var pickupTarget : Node3D = null
var IsWumpa : bool = true

func _ready():
	super()
	
	rotation_degrees = Vector3(0, (randf() - 0.5) * 360.0, 0);
	set("contact_monitor", true)
	set("max_contacts_reported", 256)
	collision_layer = 0
	set("freeze_mode",RigidBody3D.FREEZE_MODE_KINEMATIC)
	connect("body_entered", OnPickup)
	#body_entered.connect(OnPickup)
	if (name != "Pickup_Wumpa"):
		IsWumpa = false
	else:
		CreateShadow(0, Vector2.ONE * 0.5, 0)

func _physics_process(delta):
	rotate_y(3.0 * delta)
	if (!visible): process_mode = Node.PROCESS_MODE_DISABLED
	if (isPickedUp):
		if (pickupTarget == null):
			process_mode = Node.PROCESS_MODE_DISABLED
			visible = false
			return;
		global_position = global_position.move_toward(pickupTarget.global_position, 15.0 * delta)
		if (global_position.distance_to(pickupTarget.global_position) < 0.1):
			RehabSceneRoot.Game.AddWumpa(1)
			DoSound(1, (randf() / 5.0) + 0.9, 0.0)
			process_mode = Node.PROCESS_MODE_DISABLED
			visible = false

func OnPickup(body):
	#print("entered " + body.name)
	if (isPickedUp):
		return;
	if !(body is CharacterBody3D):
		return;
	collision_layer = 0
	collision_mask = 0
	pickupTarget = body
	isPickedUp = true
	if (IsWumpa): return;
	DoSound(1, 1.0, 0.0)
	visible = false
	match name:
		"Pickup_Crystal": RehabSceneRoot.Game.AddCrystal()
		"Pickup_Gem_Blue": RehabSceneRoot.Game.AddGem(0)
		"Pickup_Gem_Clear": RehabSceneRoot.Game.AddGem(1)
		"Pickup_Gem_Green": RehabSceneRoot.Game.AddGem(2)
		"Pickup_Gem_Purple": RehabSceneRoot.Game.AddGem(3)
		"Pickup_Gem_Red": RehabSceneRoot.Game.AddGem(4)
		"Pickup_Gem_Yellow": RehabSceneRoot.Game.AddGem(5)
		"Pickup_ExtraLife", "Pickup_ExtraLifeCortex", "Pickup_ExtraLifeNina": RehabSceneRoot.Game.AddLives(1)
		_: pass
	

