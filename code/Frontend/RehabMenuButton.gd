extends Button

var mode : bool = false;
var isAnim : bool = false;
var quiet : bool = false;
var origScale = Vector2.ONE

func _process(delta):
	if (!isAnim): return;
	pivot_offset = Vector2(size.x / 2, size.y / 2)
	
	if (!mode):
		scale = scale.move_toward(origScale * Vector2(0.9, 0.9), delta * 0.5)
		if (scale.x <= origScale.x * 0.91):
			mode = !mode
	else:
		scale = scale.move_toward(origScale * Vector2(1.1, 1.1), delta * 0.5)
		if (scale.x >= origScale.x * 1.09):
			mode = !mode

func StartFocus():
	if (!quiet):
		RehabSceneRoot.Root.PlayMenuSound_Select()
	origScale = scale
	pivot_offset = Vector2(size.x / 2, size.y / 2)
	scale = origScale * Vector2(1.1, 1.1)
	modulate = Color(1.0, 1.0, 1.0, 1.0)
	mode = false
	isAnim = true

func EndFocus():
	scale = origScale
	modulate = Color(0.8, 0.8, 0.8, 1.0)
	isAnim = false

func OnPress():
	if (!quiet):
		RehabSceneRoot.Root.PlayMenuSound_Click()

