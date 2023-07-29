extends Resource
class_name ALabScript

var Priority : int = 50
var Participants : Array[Agent]
var ME : Agent
var Unit
var Percepts : Array[ALab_Percept]

func OnRun(a : Agent):
	ME = a
	ME.ActiveState = self
	if (Participants.size() == 0):
		Participants.append(a)
	Reset()
	Run(0)
	pass

func Run(delta):
	var cont : bool = ME.ControlPackRun(delta)
	if (!cont):
		return
	cont = false
	if (Percepts.size() != 0):
		for p in Percepts:
			cont = p.run()
			if (cont):
				break
	if (cont):
		Percepts.clear()
		ME.ControlPackReset()
		Unit.call()
	
func Reset():
	_init()
