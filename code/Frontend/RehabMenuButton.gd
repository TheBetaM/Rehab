extends Button

var mode : bool = false;
var isAnim : bool = false;

func _process(delta):
	if (!isAnim): return;
	pivot_offset = Vector2(size.x / 2, size.y / 2)
	
	if (!mode):
		scale = scale.move_toward(Vector2(0.9, 0.9), delta * 0.5)
		if (scale.x <= 0.91):
			mode = !mode
	else:
		scale = scale.move_toward(Vector2(1.1, 1.1), delta * 0.5)
		if (scale.x >= 1.09):
			mode = !mode

func StartFocus():
	RehabSceneRoot.Root.PlayMenuSound_Select()
	pivot_offset = Vector2(size.x / 2, size.y / 2)
	scale = Vector2(1.1, 1.1)
	mode = false
	isAnim = true

func EndFocus():
	scale = Vector2(1, 1)
	isAnim = false

func OnPress():
	RehabSceneRoot.Root.PlayMenuSound_Click()
