extends Camera3D

var camangleX = 0.0
var camangleY = 0.0
#var camheight = 0.0
var camvector = Vector3.ZERO
var camTarget : Node3D
var camright = Vector3.ZERO
var mouse_position : Vector2 = Vector2(0.0, 0.0)
var mouse_last_motion : int = 0
var mouse_add : Vector2 = Vector2(0.0, 0.0)
var pivot : Vector3 = Vector3.ZERO

func _input(event):
	if (process_mode == Node.PROCESS_MODE_DISABLED): return;
	if (!RehabGame.UseMouseCamera):	return;
		
	if (event is InputEventMouseMotion):
		mouse_position = mouse_add + event.relative
		mouse_add = Vector2.ZERO
		mouse_last_motion = 2

func _process(delta):
	
	if (camTarget == null):
		return;
	
	var camdirX = 0.0
	var camdirY = 0.0
	var camx = 0.0
	var camz = 0.0
	
	if Input.is_action_pressed("pad1_rstick_left"):
		if !RehabGame.InvertCameraX:
			camdirX -= Input.get_action_strength("pad1_rstick_left")
		else:
			camdirX += Input.get_action_strength("pad1_rstick_left")
	if Input.is_action_pressed("pad1_rstick_right"):
		if !RehabGame.InvertCameraX:
			camdirX += Input.get_action_strength("pad1_rstick_right")
		else:
			camdirX -= Input.get_action_strength("pad1_rstick_right")
	if Input.is_action_pressed("pad1_rstick_up"):
		if !RehabGame.InvertCameraY:
			camdirY += Input.get_action_strength("pad1_rstick_up")
		else:
			camdirY -= Input.get_action_strength("pad1_rstick_up")
	if Input.is_action_pressed("pad1_rstick_down"):
		if !RehabGame.InvertCameraY:
			camdirY -= Input.get_action_strength("pad1_rstick_down")
		else:
			camdirY += Input.get_action_strength("pad1_rstick_down")
	if Input.is_action_just_pressed("pad1_L1"):
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
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
	if Input.is_action_pressed("pad1_R3"):
		camangleX = camTarget.global_rotation_degrees.y / 180.0
		camangleY = 0.0
	
	camvector = global_transform.basis.z
	camvector.y = 0.0
	camvector = camvector.normalized()
	camright = Vector3(camvector.z, 0, -camvector.x)
	
	var instant : bool = false
	if (mouse_last_motion == 2):
		camdirX -= mouse_position.x
		camdirY += mouse_position.y
		instant = true
		mouse_last_motion = 1
	elif (mouse_last_motion == 1):
		mouse_add = -mouse_position
		mouse_last_motion = 0
	
	if (instant):
		camangleX += camdirX / 800.0
		camangleY += camdirY / 200.0
		camangleY = clampf(camangleY, -3.0, 3.0)
	else:
		camdirX = clampf(camdirX, -1.0, 1.0)
		camdirY = clampf(camdirY, -1.0, 1.0)
		camangleX += camdirX * delta * 1.0
		camangleY += camdirY * delta * 3.0
		camangleY = clampf(camangleY, -3.0, 3.0)
	
	var angle = Vector3.FORWARD.rotated(Vector3.UP, camangleX * PI)
	var camdist = 6.0 + (-absf(camangleY))
	var camheight = 4.5 + (camangleY * 1.5)
	var pivotheight = 3.0
	
	camx += clamp((angle.x * camdist), -camdist, camdist)
	camz += clamp((angle.z * camdist), -camdist, camdist)
	if (instant):
		pivot = camTarget.global_transform.origin + Vector3(camx, camheight, camz)
	else:
		pivot = pivot.lerp(camTarget.global_transform.origin + Vector3(camx, camheight, camz), delta * 8.0)
	global_position = global_position.lerp(pivot, delta * 8.0)
	look_at(camTarget.global_transform.origin + (Vector3.UP * pivotheight), Vector3.UP)

func FullReset():
	camTarget = null
	global_position = Vector3.ZERO
	global_rotation = Vector3.ZERO
	camangleX = 0.0
	camangleY = 0.0

func SetupCam(target : Node3D):
	FullReset()
	camTarget = target
	camangleX = target.global_rotation_degrees.y / 180.0
	
	var camx = 0.0
	var camz = 0.0
	var angle = Vector3.FORWARD.rotated(Vector3.UP, camangleX * PI)
	var camdist = 6.0 + (-absf(camangleY))
	var camheight = 4.5 + (camangleY * 1.5)
	var pivotheight = 3.0
	camx += clamp((angle.x * camdist), -camdist, camdist)
	camz += clamp((angle.z * camdist), -camdist, camdist)
	global_position = camTarget.global_transform.origin + Vector3(camx, camheight, camz)
	look_at(camTarget.global_transform.origin + (Vector3.UP * pivotheight), Vector3.UP)
	camvector = global_transform.basis.z
	camvector = camvector.normalized()
	camright = Vector3(camvector.z, 0, -camvector.x)
	camright = camright.normalized()
