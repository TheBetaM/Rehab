extends Agent
class_name AgentCharacter

enum CharASlot {
	UnkAngle1 = 0,
	UnkAngle2 = 0,
	UnkAngle3 = 0,
	UnkAngle4 = 0,
	UnkAngle5 = 0,
	UnkAngle6 = 0,
	UnkAngle7 = 0,
	UnkAngle8 = 0,
	UnkAngle9 = 0,
}
enum CharFSlot {
	UnkFloat01 = 0,
	AirGravity = 1,
	UnkFloat03 = 2,
	BaseGravity = 3,
	WalkSpeedPercentage = 4,
	UnkFloat06 = 5,
	WalkSpeed = 6,
	RunSpeed = 7,
	StrafingSpeed = 8,
	SpinThrowForwardForce = 9,
	SpinLength = 11,
	SpinDelay = 12,
	UnkFloat13 = 13,
	UnkFloat14 = 14,
	JumpAirSpeed = 15,
	JumpHeight = 16,
	UnkFloat17Jump = 17,
	UnkFloat18Jump = 18,
	JumpEdgeSpeed = 19,
	DoubleJumpHeight = 20,
	UnkFloat21DoubleJump = 21,
	UnkFloat22DoubleJump = 22,
	UnkFloat23SlideJump = 23,
	UnkFloat24SlideJump = 24,
	UnkFloat25SlideJump = 25,
	UnkFloat26SlideJump = 26,
	UnkFloat27 = 27,
	UnkFloat28 = 28,
	UnkFloat29 = 29,
	UnkFloat30 = 30,
	BodyslamHangTime = 31,
	BodyslamUpwardForce = 32,
	BodyslamGravityForce = 33,
	FlyingKickHangTime = 34,
	FlyingKickForwardSpeed = 35,
	FlyingKickGravity = 36,
	RadialBlastTimeToStart = 37,
	UnkFloat38RadialBlast = 38,
	UnkFloat39RadialBlast = 39,
	CrawlSpeed = 40,
	CrawlTimeFromStand = 41,
	CrawlTimeToStand = 42,
	CrawlTimeToRun = 43,
	SlideSpeed = 44,
	UnkFloat45Slide = 45,
	UnkFloat46Slide = 46,
	UnkFloat47Slide = 47,
	UnkFloat48Slide = 48,
	UnkFloat49Slide = 49,
	GunButtonHoldTimeToStartCharging = 50,
	GunChargeTime = 51,
	GunTimeBetweenChargedShots = 52,
	GunTimeBetweenShots = 53,
	UnkFloat54 = 54,
	RadialBlastChargeTime = 55,
}
enum CharISlot {
	AgentType = 0,
	UnkInt = 1,
	Health = 2,
}


# Test Logic Below

var speed = 14
var fall_acceleration = 30

var velocity = Vector3.ZERO
var modeldirection = Vector3.ZERO
var physBody : Node3D
var physCam : Camera3D
var isReparenting : bool = false

static var activeCharacter : Agent
static var ActiveActorTypes : Dictionary #int type : Agent character

func _ready():
	super()
	
	if (!ActiveActorTypes.has(RegInt[CharISlot.AgentType])):
		ActiveActorTypes[RegInt[CharISlot.AgentType]] = get_path()
	else:
		visible = false
		process_mode = Node.PROCESS_MODE_DISABLED
		
	if (RegInt[CharISlot.AgentType] > 3):
		return
	if (activeCharacter != null):
		return
		
	activeCharacter = self
	physBody = SubModels[0].get_node("RigidBody")
	var oldBody = SubModels[0].get_node("RigidBody")
	var charBody = CharacterBody3D.new()
	physBody.replace_by(charBody)
	charBody.global_position.y += 5.0
	#charBody.get_child(0).disabled = false
	oldBody.queue_free()
	physBody = charBody
	physCam = RehabSceneRoot.Root.PlayerCam
	physCam.camTarget = physBody
	var animPlayer : AnimationPlayer = SubModels[0].get_node("AnimationPlayer")
	var armature = charBody.get_node("Armature")
	animPlayer.root_node = NodePath("../" + charBody.name + "/Armature")
	DoAnimation(8, true)

func _exit_tree():
	if (isReparenting):
		isReparenting = false
		return
	if (ActiveActorTypes.has(RegInt[CharISlot.AgentType])):
		if (ActiveActorTypes[RegInt[CharISlot.AgentType]] == get_path()):
			ActiveActorTypes.erase(RegInt[CharISlot.AgentType])
	if (activeCharacter == self):
		activeCharacter = null

func _physics_process(delta):
	var direction = Vector3.ZERO
	var camdir = 0.0
	var pressed = false
	var isJumping = false
	
	ActiveActorTypes[RegInt[CharISlot.AgentType]] = get_path()
	if (physBody == null || physBody.process_mode == PROCESS_MODE_DISABLED):
		return
	if (activeCharacter != self):
		return

	if Input.is_action_pressed("ui_up"):
		direction.x -= Input.get_action_raw_strength("ui_up")
		pressed = true
	if Input.is_action_pressed("ui_down"):
		direction.x += Input.get_action_raw_strength("ui_down")
		pressed = true
	if Input.is_action_pressed("ui_right"):
		direction.z += Input.get_action_raw_strength("ui_right")
		pressed = true
	if Input.is_action_pressed("ui_left"):
		direction.z -= Input.get_action_raw_strength("ui_left")
		pressed = true
	if Input.is_action_pressed("ui_accept"):
		velocity.y += 100 * delta
		DoAnimation(19, false)
		isJumping = true
	
	if direction != Vector3.ZERO:
		direction = direction.normalized()
	
	direction = (direction.x * physCam.camvector) + (direction.z * physCam.camright)
	#direction.x *= camvector.x
	#direction.z *= camvector.z
	
	if (pressed):
		#var dirVector = Vector2(direction.x, direction.z)
		#var dirAngle = (dirVector.angle_to(Vector2(0, 1)) / PI) * 180
		#modeldirection = modeldirection.lerp(Vector3(0, dirAngle, 0), 0.1 * delta)
		#physBody.rotation = modeldirection
		velocity.x = direction.x * speed
		velocity.z = direction.z * speed
		physBody.global_rotation = Vector3(physBody.global_rotation.x, atan2(direction.x, direction.z), physBody.global_rotation.z)
		if (!isJumping && physBody.is_on_floor()):
			DoAnimation(11, true)
	else:
		velocity.x = move_toward(velocity.x, 0, speed)
		velocity.z = move_toward(velocity.z, 0, speed)
		if (!isJumping && physBody.is_on_floor()):
			DoAnimation(8, true)
		
	if (!isJumping && !physBody.is_on_floor()):
		DoAnimation(27, false)
	
	if not physBody.is_on_floor():
		velocity.y -= fall_acceleration * delta
	
	physBody.velocity = velocity
	physBody.move_and_slide()
	



