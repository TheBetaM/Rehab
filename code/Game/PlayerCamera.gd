extends Camera3D

var camangle = 0.0
#var camheight = 0.0
var camvector = Vector3.ZERO
var camTarget : Node3D
var camright = Vector3.ZERO

func _process(delta):
	
	if (camTarget == null):
		return;
	
	var camdir = 0.0
	
	if Input.is_action_pressed("pad1_rstick_left"):
		camdir += Input.get_action_raw_strength("pad1_rstick_left")
	if Input.is_action_pressed("pad1_rstick_right"):
		camdir -= Input.get_action_raw_strength("pad1_rstick_right")
	if Input.is_action_just_pressed("ui_cancel"):
		var freeCam = RehabSceneRoot.Root.FreeLookCam
		freeCam.global_position = global_position
		freeCam.global_rotation_degrees = global_rotation_degrees
		current = false
		freeCam.current = true
		freeCam.cooldown = 0.1
		process_mode = Node.PROCESS_MODE_DISABLED
		freeCam.process_mode = Node.PROCESS_MODE_ALWAYS
		camTarget.process_mode = Node.PROCESS_MODE_DISABLED
		return;
	if Input.is_action_just_pressed("ui_select"):
		ReturnToLevelSelect()
		return;
	
	camvector = global_transform.basis.z
	camvector = camvector.normalized()
	camright = Vector3(camvector.z, 0, -camvector.x)
	
	camangle += camdir * delta * 1.0
	if (camangle < -1.0):
		camangle = 1.0
	if (camangle > 1.0):
		camangle = -1.0
	
	var camx = 0.0
	var camz = 0.0
	var angle = Vector3.FORWARD.rotated(Vector3.UP, camangle * PI)
	camx += clamp((angle.x * 6), -6.0, 6.0)
	camz += clamp((angle.z * 6), -6.0, 6.0)
	
	global_position = global_position.lerp( camTarget.global_transform.origin + Vector3(camx, 4.5, camz), delta * 8.0)
	look_at(camTarget.global_transform.origin + (Vector3.UP * 3), Vector3.UP)

func ReturnToLevelSelect():
	if (current):
		await get_tree().create_timer(0.1).timeout
		RehabSceneRoot.Root.ExitLevel()

func FullReset():
	camTarget = null
	global_position = Vector3.ZERO
	global_rotation = Vector3.ZERO
	camangle = 0.0
