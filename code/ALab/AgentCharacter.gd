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
var velocity = Vector3.ZERO
var modeldirection = Vector3.ZERO
var physBody : Node3D
var physCam : Camera3D
var isReparenting : bool = false
var headdirX = 0.0
var headdirY = 0.0
var footsteptimer = 0.0
var footsteplast = false

var FS_Dirt_1 : AudioStream
var FS_Dirt_2 : AudioStream
var FS_Grass_1 : AudioStream
var FS_Grass_2 : AudioStream
var FS_Metal_1 : AudioStream
var FS_Metal_2 : AudioStream
var FS_Sand_1 : AudioStream
var FS_Sand_2 : AudioStream
var FS_Stone_1 : AudioStream
var FS_Stone_2 : AudioStream
var FS_Water_1 : AudioStream
var FS_Water_2 : AudioStream
var FS_Wood_1 : AudioStream
var FS_Wood_2 : AudioStream
var FS_Tile_1 : AudioStream
var FS_Tile_2 : AudioStream

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
	charBody.global_position.y += 3.0
	#charBody.get_child(0).disabled = false
	oldBody.queue_free()
	physBody = charBody
	physCam = RehabSceneRoot.Root.PlayerCam
	physCam.SetupCam(physBody)
	var animPlayer : AnimationPlayer = SubModels[0].get_node("AnimationPlayer")
	var armature = charBody.get_node("Armature")
	animPlayer.root_node = NodePath("../" + charBody.name + "/Armature")
	DoAnimation(8, true)
	FS_Dirt_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_dirt_3.tres")
	FS_Dirt_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_dirt_5.tres")
	FS_Grass_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_grass_2.tres")
	FS_Grass_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_grass_3.tres")
	FS_Metal_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_metal_1.tres")
	FS_Metal_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_metal_5.tres")
	FS_Sand_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_sand_1.tres")
	FS_Sand_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_sand_3.tres")
	FS_Stone_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_stone_3.tres")
	FS_Stone_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_stone_5.tres")
	FS_Water_1 = load(RehabGame.AssetsPath + "Sounds/Surface/FS_WAT1.tres")
	FS_Water_2 = load(RehabGame.AssetsPath + "Sounds/Surface/FS_WAT2.tres")
	FS_Wood_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_wood_1.tres")
	FS_Wood_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_wood_2.tres")
	FS_Tile_1 = load(RehabGame.AssetsPath + "Sounds/Surface/L09_Cortex_boots_tile_2.tres")
	FS_Tile_2 = load(RehabGame.AssetsPath + "Sounds/Surface/L09_Cortex_boots_tile_7.tres")

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
	ActiveActorTypes[RegInt[CharISlot.AgentType]] = get_path()
	if (physBody == null || physBody.process_mode == PROCESS_MODE_DISABLED):
		return
	if (activeCharacter != self):
		return
	if Input.is_action_pressed("pad1_start"):
		RehabSceneRoot.Root.StartPauseMenu()
		return
	
	UpdateMovement(delta)
	UpdateHeadAnim(delta)
	UpdateFootStep(delta)
	

func UpdateMovement(delta):
	var direction = Vector3.ZERO
	var camdir = 0.0
	var pressed = false
	var isJumping = false
	
	if Input.is_action_pressed("ui_up"):
		direction.x -= Input.get_action_strength("ui_up")
		pressed = true
	if Input.is_action_pressed("ui_down"):
		direction.x += Input.get_action_strength("ui_down")
		pressed = true
	if Input.is_action_pressed("ui_right"):
		direction.z += Input.get_action_strength("ui_right")
		pressed = true
	if Input.is_action_pressed("ui_left"):
		direction.z -= Input.get_action_strength("ui_left")
		pressed = true
	if Input.is_action_pressed("ui_accept"):
		velocity.y += 80 * delta
		if (ActiveAnim != 19):
			DoAnimation(19, false)
			DoSound(0, 1.0, 0.0)
		isJumping = true
	if Input.is_action_just_pressed("ui_select"):
		RehabSceneRoot.Root.Game.DisplayHUD()
	
	direction = direction.clamp(-Vector3.ONE, Vector3.ONE)
	direction = (direction.x * physCam.camvector) + (direction.z * physCam.camright)
	direction.y = 0.0
	var speed = RegFloat[CharFSlot.RunSpeed]
	if (abs(direction.length()) < 0.3):
		speed = 0
	elif (abs(direction.length()) < 0.8):
		speed = RegFloat[CharFSlot.WalkSpeed]
	direction = direction.normalized()
	
	if (pressed):
		velocity.x = direction.x * speed
		velocity.z = direction.z * speed
		physBody.global_rotation = Vector3(physBody.global_rotation.x, atan2(direction.x, direction.z), physBody.global_rotation.z)
		if (!isJumping && physBody.is_on_floor()):
			if (speed == 0):
				DoAnimation(9, true)
			elif (speed == RegFloat[CharFSlot.WalkSpeed]):
				DoAnimation(10, true)
			else:
				DoAnimation(11, true)
	else:
		velocity.x = 0.0
		velocity.z = 0.0
		if (!isJumping && physBody.is_on_floor()):
			DoAnimation(8, true)
		
	if (!isJumping && !physBody.is_on_floor()):
		DoAnimation(27, false)
	
	if not physBody.is_on_floor():
		velocity.y -= 30.0 * delta
	
	physBody.velocity = velocity
	physBody.move_and_slide()

func UpdateHeadAnim(delta):
	if (headdirX > 0):
		headdirX -= delta * 0.4
		headdirX = clampf(headdirX, 0, 0.8)
	else:
		headdirX += delta * 0.4
		headdirX = clampf(headdirX, -0.8, 0)
	
	if (headdirY > 0):
		headdirY -= delta * 0.5
		headdirY = clampf(headdirY, 0, 1.0)
	else:
		headdirY += delta * 0.75
		headdirY = clampf(headdirY, -1.5, 0)
	
	if Input.is_action_pressed("pad1_rstick_left"):
		headdirX += Input.get_action_strength("pad1_rstick_left") * delta * 4.0
	if Input.is_action_pressed("pad1_rstick_right"):
		headdirX -= Input.get_action_strength("pad1_rstick_right") * delta * 4.0
	if Input.is_action_pressed("pad1_rstick_up"):
		headdirY += Input.get_action_strength("pad1_rstick_up") * delta * 4.0
	if Input.is_action_pressed("pad1_rstick_down"):
		headdirY -= Input.get_action_strength("pad1_rstick_down") * delta * 4.0
	
	headdirX = clampf(headdirX, -0.8, 0.8)
	headdirY = clampf(headdirY, -1.5, 1.0)
	
	SubModels[ActiveModel].get_node("AnimationPlayer").playback_process_mode = AnimationPlayer.ANIMATION_PROCESS_MANUAL
	SubModels[ActiveModel].get_node("AnimationPlayer").advance(delta)
	if (ActiveSkeleton != null and JointsConst[2] != -1):
		var headBoneRot = ActiveSkeleton.get_bone_pose_rotation(JointsConst[2])
		var headBoneEuler = headBoneRot.get_euler()
		headBoneEuler.x += headdirY
		headBoneEuler.y += headdirX
		#headBoneEuler.z -= headdirX
		ActiveSkeleton.set_bone_pose_rotation(JointsConst[2], Quaternion.from_euler(headBoneEuler))


func UpdateFootStep(delta):
	if (ActiveAnim != 11 and ActiveAnim != 10):
		return
	
	footsteptimer -= delta
	if (footsteptimer < 0):
		footsteplast = !footsteplast
		var clip1 = FS_Dirt_1
		var clip2 = FS_Dirt_2
		var space_state = get_world_3d().direct_space_state
		var query = PhysicsRayQueryParameters3D.create(physBody.global_position + (Vector3.UP * 1.0), physBody.global_position + (Vector3.UP * -3.0))
		query.exclude = [physBody]
		var result = space_state.intersect_ray(query)
		if (result.has("collider") and result["collider"] is StaticBody3D):
			match result["collider"].get_parent().name:
				"Default", "Normal_Rock", "Slippy_Rock", "Ice", "Ice_LowSlippy":
					clip1 = FS_Stone_1
					clip2 = FS_Stone_2
				"Normal_Grass":
					clip1 = FS_Grass_1
					clip2 = FS_Grass_2
				"Normal_Metal", "Slippy_Metal":
					clip1 = FS_Metal_1
					clip2 = FS_Metal_2
				"Normal_Wood":
					clip1 = FS_Wood_1
					clip2 = FS_Wood_2
				"Normal_Sand", "Normal_Snow", "Sticky_Snow":
					clip1 = FS_Sand_1
					clip2 = FS_Sand_2
				"Normal_Mud":
					clip1 = FS_Dirt_1
					clip2 = FS_Dirt_2
				"Normal_Water":
					clip1 = FS_Water_1
					clip2 = FS_Water_2
				"Normal_StoneTiles":
					clip1 = FS_Tile_1
					clip2 = FS_Tile_2
				_:
					clip1 = FS_Stone_1
					clip2 = FS_Stone_2
			
			if (footsteplast):
				clip1 = clip2
			DoSoundStream(clip1, 1.0, -5.0)
			if (ActiveAnim == 10):
				footsteptimer = 0.5
			else:
				footsteptimer = 0.25
	
