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


var char_velocity = Vector3.ZERO
var modeldirection = Vector3.ZERO
#var physBody : Node3D
var physCam : Camera3D
var isReparenting : bool = false
var headdirX = 0.0
var headdirY = 0.0
var footsteptimer = 0.0
var footsteplast = false
var gravityOn = true
var spinTimer = 0.0

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
var FS_Slippy : AudioStream

static var activeCharacter : AgentCharacter
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
	#physBody = SubModels[0].get_node("RigidBody")
	#var oldBody = SubModels[0].get_node("RigidBody")
	#var charBody = CharacterBody3D.new()
	#physBody.replace_by(charBody)
	#charBody.global_position.y += 3.0
	##charBody.get_child(0).disabled = false
	#oldBody.queue_free()
	#physBody = charBody
	physCam = RehabSceneRoot.Root.PlayerCam
	physCam.SetupCam(self)
	#physCam.SetupCam(physBody)
	#var animPlayer : AnimationPlayer = SubModels[0].get_node("AnimationPlayer")
	#var armature = charBody.get_node("Armature")
	#animPlayer.root_node = NodePath("../" + charBody.name + "/Armature")
	CreateShadow(0, Vector2.ONE, 0)
	DoAnimation(8, true)
	FS_Dirt_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_dirt_3.res")
	FS_Dirt_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_dirt_5.res")
	FS_Grass_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_grass_2.res")
	FS_Grass_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_grass_3.res")
	FS_Metal_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_metal_1.res")
	FS_Metal_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_metal_5.res")
	FS_Sand_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_sand_1.res")
	FS_Sand_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_sand_3.res")
	FS_Stone_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_stone_3.res")
	FS_Stone_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_stone_5.res")
	FS_Water_1 = load(RehabGame.AssetsPath + "Sounds/Surface/FS_WAT1.res")
	FS_Water_2 = load(RehabGame.AssetsPath + "Sounds/Surface/FS_WAT2.res")
	FS_Wood_1 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_wood_1.res")
	FS_Wood_2 = load(RehabGame.AssetsPath + "Sounds/Surface/fs_wood_2.res")
	FS_Tile_1 = load(RehabGame.AssetsPath + "Sounds/Surface/L09_Cortex_boots_tile_2.res")
	FS_Tile_2 = load(RehabGame.AssetsPath + "Sounds/Surface/L09_Cortex_boots_tile_7.res")
	FS_Slippy = load(RehabGame.AssetsPath + "Sounds/L03_TotemHokum/L03_Tribesmn_fs.res")

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
	if (process_mode == Node3D.PROCESS_MODE_DISABLED): return
	if (activeCharacter != self): return
	if Input.is_action_pressed("pad1_start"):
		RehabSceneRoot.Root.StartPauseMenu(false)
		return
	
	UpdateMovement(delta)
	UpdateHeadAnim(delta)
	UpdateFootStep(delta)

func UpdateMovement(delta):
	var direction = Vector3.ZERO
	var camdir = 0.0
	var isJumping = false
	var onFloor : bool = call("is_on_floor")
	
	direction.x -= Input.get_action_strength("pad1_dpad_up")
	direction.x += Input.get_action_strength("pad1_dpad_down")
	if (direction.x == 0):
		direction.x -= Input.get_action_strength("pad1_lstick_up")
		direction.x += Input.get_action_strength("pad1_lstick_down")
	direction.z += Input.get_action_strength("pad1_dpad_right")
	direction.z -= Input.get_action_strength("pad1_dpad_left")
	if (direction.z == 0):
		direction.z += Input.get_action_strength("pad1_lstick_right")
		direction.z -= Input.get_action_strength("pad1_lstick_left")
	if Input.is_action_pressed("pad1_cross"):
		char_velocity.y += 80 * delta
		if (ActiveAnim != 19 and spinTimer <= 0.0):
			DoAnimation(19, false)
			DoSound(0, 1.0, 0.0)
		isJumping = true
	if Input.is_action_just_pressed("pad1_triangle"):
		RehabSceneRoot.Root.Game.DisplayHUD()
	if Input.is_action_just_pressed("pad1_square") and spinTimer <= 0.0:
		#char_velocity.y = 0.0
		#gravityOn = !gravityOn
		spinTimer = RegFloat[CharFSlot.SpinLength]
		DoAnimation(14, true)
		DoSound(2, 1.0, 0.0)
	
	if (spinTimer > 0.0): 
		spinTimer -= delta
	
	direction = direction.clamp(-Vector3.ONE, Vector3.ONE)
	direction = (direction.x * physCam.camvector) + (direction.z * physCam.camright)
	direction.y = 0.0
	var speed = RegFloat[CharFSlot.RunSpeed]
	var pressed = abs(direction.length()) > 0.05
	if (abs(direction.length()) < 0.3):
		speed = 0
	elif (abs(direction.length()) < 0.8):
		speed = RegFloat[CharFSlot.WalkSpeed]
	direction = direction.normalized()
	
	if (pressed):
		#if not physBody.is_on_floor():
		#	speed *= 3.0
		char_velocity.x = direction.x * speed
		char_velocity.z = direction.z * speed
		var targetRot = Vector3(global_rotation.x, atan2(direction.x, direction.z), global_rotation.z)
		if (speed != 0):
			global_rotation = targetRot
		else:
			global_rotation = global_rotation.slerp(targetRot, 5.0 * delta)
		if (!isJumping && onFloor && spinTimer <= 0.0):
			if (speed == 0):
				DoAnimation(9, true)
			elif (speed == RegFloat[CharFSlot.WalkSpeed]):
				DoAnimation(10, true)
			else:
				DoAnimation(11, true)
	else:
		char_velocity.x = 0.0
		char_velocity.z = 0.0
		if (!isJumping && onFloor && spinTimer <= 0.0):
			DoAnimation(8, true)
		
	if (!isJumping && !onFloor && spinTimer <= 0.0):
		DoAnimation(27, false)
	
	if (!onFloor):
		if (gravityOn):
			char_velocity.y -= 30.0 * delta
		else:
			char_velocity.y = 0.0
	
	#physBody.velocity = velocity
	set("velocity", char_velocity)
	call("move_and_slide")

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
		if !RehabGame.InvertCameraX:
			headdirX -= Input.get_action_strength("pad1_rstick_left") * delta * 4.0
		else:
			headdirX += Input.get_action_strength("pad1_rstick_left") * delta * 4.0
	if Input.is_action_pressed("pad1_rstick_right"):
		if !RehabGame.InvertCameraX:
			headdirX += Input.get_action_strength("pad1_rstick_right") * delta * 4.0
		else:
			headdirX -= Input.get_action_strength("pad1_rstick_right") * delta * 4.0
	if Input.is_action_pressed("pad1_rstick_up"):
		if !RehabGame.InvertCameraY:
			headdirY += Input.get_action_strength("pad1_rstick_up") * delta * 4.0
		else:
			headdirY -= Input.get_action_strength("pad1_rstick_up") * delta * 4.0
	if Input.is_action_pressed("pad1_rstick_down"):
		if !RehabGame.InvertCameraY:
			headdirY -= Input.get_action_strength("pad1_rstick_down") * delta * 4.0
		else:
			headdirY += Input.get_action_strength("pad1_rstick_down") * delta * 4.0
	
	headdirX = clampf(headdirX, -0.8, 0.8)
	headdirY = clampf(headdirY, -1.5, 1.0)
	
	SubModels[ActiveModel].get_node("AnimationPlayer").playback_process_mode = AnimationPlayer.ANIMATION_PROCESS_MANUAL
	SubModels[ActiveModel].get_node("AnimationPlayer").advance(delta)
	if (ActiveSkeleton != null and JointsConst[2] != -1 and ActiveSkeleton.get_bone_count() > JointsConst[2]):
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
		var query = PhysicsRayQueryParameters3D.create(global_position + (Vector3.UP * 1.0), global_position + (Vector3.UP * -3.0))
		query.exclude = [self]
		var result = space_state.intersect_ray(query)
		if (result.has("collider") and result["collider"] is StaticBody3D):
			match result["collider"].get_parent().name:
				"Normal_Rock":
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
				"Normal_Sand", "Normal_Snow":
					clip1 = FS_Sand_1
					clip2 = FS_Sand_2
				"Default", "Normal_Mud", "Generic_MediumSlippy", "Lava", "Slippy_Rock", "Sticky_Snow", "Ice", "Ice_LowSlippy", "Generic_MediumSlippy_RigidOnly":
					clip1 = FS_Dirt_1
					clip2 = FS_Dirt_2
				"Normal_Water":
					clip1 = FS_Water_1
					clip2 = FS_Water_2
				"Normal_StoneTiles":
					clip1 = FS_Tile_1
					clip2 = FS_Tile_2
				"Generic_SlightlySlippy", "HackRail":
					clip1 = FS_Slippy
					clip2 = FS_Slippy
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
	
