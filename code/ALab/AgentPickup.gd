extends Agent
class_name AgentPickup

var isPickedUp : bool = false
var pickupTarget : Node3D = null

func _ready():
	super()
	
	SubModels[0].rotation_degrees = Vector3(0, (randf() - 0.5) * 360.0, 0);
	SubModels[0].get_node("RigidBody").contact_monitor = true
	SubModels[0].get_node("RigidBody").max_contacts_reported = 256
	SubModels[0].get_node("RigidBody").collision_layer = 0
	SubModels[0].get_node("RigidBody").freeze_mode = RigidBody3D.FREEZE_MODE_KINEMATIC
	SubModels[0].get_node("RigidBody").body_entered.connect(OnPickup)

func _physics_process(delta):
	SubModels[0].rotate_y(3.0 * delta)
	if (isPickedUp):
		if (pickupTarget == null):
			process_mode = Node.PROCESS_MODE_DISABLED
			visible = false
			return;
		SubModels[0].global_position = SubModels[0].global_position.move_toward(pickupTarget.global_position, 15.0 * delta)
		if (SubModels[0].global_position.distance_to(pickupTarget.global_position) < 0.1):
			RehabSceneRoot.Root.Game.AddWumpa(1)
			DoSound(1, (randf() / 5.0) + 0.9, 0.0)
			process_mode = Node.PROCESS_MODE_DISABLED
			visible = false

func OnPickup(body):
	#print("entered " + body.name)
	if (isPickedUp):
		return;
	if !(body is CharacterBody3D):
		return;
	SubModels[0].get_node("RigidBody").collision_layer = 0
	SubModels[0].get_node("RigidBody").collision_mask = 0
	pickupTarget = body
	isPickedUp = true

