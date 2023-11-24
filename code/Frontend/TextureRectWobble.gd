extends TextureRect

var mode : bool = false;
@export var isAnim : bool = true;
@export var scaleMagnitude : float = 0.05;
@export var speed : float = 0.5

func _process(delta):
	if (!isAnim): return;
	#pivot_offset = Vector2(size.x / 2, size.y / 2)
	
	if (!mode):
		scale = scale.move_toward(Vector2.ONE + (Vector2.ONE * scaleMagnitude), delta * speed)
		if (scale.x + 0.01 >= 1.0 + scaleMagnitude):
			mode = !mode
	else:
		scale = scale.move_toward(Vector2.ONE - (Vector2.ONE * scaleMagnitude), delta * speed)
		if (scale.x - 0.01 <= 1.0 - scaleMagnitude):
			mode = !mode
