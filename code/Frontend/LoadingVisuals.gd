extends Control

@onready var LevelIcon : TextureRect = $LevelIcon

func _ready():
	$AnimationPlayer.play("TextAnim")

func UpdateVisuals():
	LevelIcon.texture = null
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
			return;
	if (path != "" and ResourceLoader.exists(path)):
		LevelIcon.texture = load(path)
		$LabelLevelName.text = ""

func AnimIn():
	UpdateVisuals()
	modulate.a = 1.0
	#modulate.a = 0.0
	#var mTween = create_tween();
	#mTween.tween_property(self, "modulate:a", 1.0, 0.5)
	visible = true

func AnimOut():
	modulate.a = 1.0
	var mTween = create_tween();
	mTween.tween_property(self, "modulate:a", 0.0, 0.5)
