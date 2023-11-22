extends AudioStreamPlayer

var IsFadingOut = false

func _process(delta):
	if (IsFadingOut):
		if (volume_db > -80.0):
			volume_db -= delta * 40.0;
		else:
			stop()
