extends Control

@onready var LevelIcon : TextureRect = $LevelIcon
@onready var LevelIcon2 : TextureRect = $LevelIcon/LevelIcon2

@onready var Root3D : Node3D = $ViewHolder/SubViewportContainer/SubViewport/FE_ROOT
@onready var CamRoot3D : Camera3D = $ViewHolder/SubViewportContainer/SubViewport/Camera3D
@onready var Viewport3D : Viewport = $ViewHolder/SubViewportContainer/SubViewport
var ActorPath = "res://assets/frontend/dynamic/FE_Actors.tscn"
var ActorsExist : bool = false
var ActorScene : PackedScene
var ActorNode : Node3D
var RequiredAssets = [
	RehabGame.AssetsPath + "Rigs/Rig_Crash.tscn",
	RehabGame.AssetsPath + "Rigs/Rig_Cortex.tscn",
	RehabGame.AssetsPath + "Rigs/RigRESET_Crash.tres",
	RehabGame.AssetsPath + "Rigs/RigRESET_Cortex.tres",
]
@onready var LoadMat1 : Material = load("res://assets/frontend/dynamic/SolidColorWhite.tres")

func LoadActors():
	ActorsExist = true
	for i in RequiredAssets:
		if (!ResourceLoader.exists(i)):
			ActorsExist = false
			break;
	
	if (ActorsExist):
		ActorScene = ResourceLoader.load(ActorPath)
		ActorNode = ActorScene.instantiate()
		Root3D.add_child(ActorNode)
		UpdateActorMat(ActorNode)

func _process(delta):
	if (!ActorsExist): return;
	
	var camdirX = 0.0
	var _camdirY = 0.0
	var oldX = Root3D.rotation_degrees.x
	var oldY = Root3D.rotation_degrees.y
	
	camdirX += Input.get_action_strength("pad1_dpad_right")
	camdirX -= Input.get_action_strength("pad1_dpad_left")
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
			_camdirY += Input.get_action_strength("pad1_rstick_up")
		else:
			_camdirY -= Input.get_action_strength("pad1_rstick_up")
	if Input.is_action_pressed("pad1_rstick_down"):
		if !RehabGame.InvertCameraY:
			_camdirY -= Input.get_action_strength("pad1_rstick_down")
		else:
			_camdirY += Input.get_action_strength("pad1_rstick_down")
	
	#oldX = oldX + (camdirY * delta * 45.0)
	oldY = oldY + (camdirX * delta * 45.0)
	Root3D.rotation_degrees = Vector3(oldX, oldY, 0)

func UpdateActorMat(parent : Node):
	if (parent is VisualInstance3D):
		parent.layers = 1024
	if (parent is MeshInstance3D):
		for i in parent.get_surface_override_material_count():
			parent.set_surface_override_material(i, LoadMat1)
	for a in parent.get_children():
		UpdateActorMat(a)

func UpdateVisuals():
	LevelIcon.texture = null
	LevelIcon2.texture = null
	
	if (!ActorsExist):
		LoadActors()
		await get_tree().process_frame
		await get_tree().process_frame
	
	$LabelLevelName.text = RehabSceneRoot.Root.LoadingChunkName.replace("_","/")
	var path : String = ""
	match $LabelLevelName.text:
		"levels/earth/hub/beach":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub01.res"
		"levels/earth/hub/huba":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level01.res"
		"levels/earth/cavern/cavent":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level02.res"
		"levels/earth/docamok/docamok1":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level03.res"
		"levels/ice/hub/labext":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub02.res"
		"levels/ice/iceclimb/caveent":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level04.res"
		"levels/ice/slipslide/l05start":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level05.res"
		"levels/ice/highseas/gpa01":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level06.res"
		"levels/school/sch/hub/sch/hub":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub03.res"
		"levels/school/boiler/boiler/1":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level08.res"
		"levels/school/crash/crashent":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level09.res"
		"levels/school/rooftop/roof01":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level10.res"
		"levels/altearth/lab/labext":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub04.res"
		"levels/altearth/rockslid/l10start":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level11.res"
		"levels/altearth/hub/altdoc":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level12.res"
		"levels/altearth/core/corea":
			path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level13.res"
		_:
			if (ActorsExist):
				$LabelLevelName.text = ""
			return;
	if (path != "" and ResourceLoader.exists(path)):
		LevelIcon.texture = load(path)
		LevelIcon2.texture = LevelIcon.texture
		$LabelLevelName.text = ""

func AnimIn():
	UpdateVisuals()
	UpdateViewport()
	$LevelIcon.isAnim = false
	$LevelIcon.scale = Vector2.ONE
	$AnimationPlayer.play("LoadingStart")
	modulate.a = 1.0
	$LabelLevelName.pivot_offset.x = $LabelLevelName.size.x / 2
	$LevelIcon.pivot_offset.x = $LevelIcon.size.x / 2;
	$LoadingBG.pivot_offset.x = $LoadingBG.size.x / 2
	$LoadingBG/ColorRectCenter.pivot_offset.x = $LoadingBG/ColorRectCenter.size.x / 2
	for i in $LoadingBG/ColorRectCenter.get_children():
		i.pivot_offset.x = i.size.x / 2
	$Control.position.y = get_window().size.y
	var aTween = create_tween();
	aTween.tween_property($Control, "position:y", get_window().size.y - $Control.size.y, 0.5)
	visible = true
	if (ActorsExist):
		var randPos = randi_range(1, 2)
		if (randPos == 1):
			Root3D.rotation_degrees = Vector3(0, 90.0, 0)
		else:
			Root3D.rotation_degrees = Vector3(0, -90.0, 0)
		var randAnim = randi_range(1, 3)
		ActorNode.visible = true
		Root3D.visible = true
		Root3D.process_mode = Node.PROCESS_MODE_INHERIT
		ActorNode.get_node("AnimationPlayer").play("RESET")
		ActorNode.get_node("AnimationPlayer").queue("scene/loading_" + str(randAnim))
	await get_tree().create_timer(0.5).timeout
	$AnimationPlayer.play("TextAnim")
	$LevelIcon.isAnim = true

func AnimOut():
	modulate.a = 1.0
	var mTween = create_tween();
	mTween.tween_property(self, "modulate:a", 0.0, 0.5)
	await get_tree().create_timer(0.49).timeout
	Root3D.visible = false
	Root3D.process_mode = Node.PROCESS_MODE_DISABLED

func UpdateViewport():
	var view = get_viewport()
	#Viewport3D.scaling_3d_mode = view.scaling_3d_mode
	Viewport3D.scaling_3d_scale = view.scaling_3d_scale
	Viewport3D.msaa_3d = view.msaa_3d
	Viewport3D.screen_space_aa = view.screen_space_aa
