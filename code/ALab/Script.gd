extends Resource
class_name ALabScript

var Priority : int = 50
var Participants : Array[Agent]
var ME : Agent
var Unit

func OnRun(a : Agent):
	ME = a
	ME.ActiveState = self
	if (Participants.size() == 0):
		Participants.append(a)
	Reset()
	Run()
	pass

func Run():
	ME.ControlPackReset()
	Unit.call()
	
func Reset():
	_init()
