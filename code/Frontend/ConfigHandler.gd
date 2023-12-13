extends Node

var Dict : Dictionary

func Load():
	var file = FileAccess.open(RehabGame.ConfigPath, FileAccess.READ)
	var text = file.get_as_text().to_lower()
	if (text.contains("object") or text.contains("refcounted") or text.contains("script")):
		file.close()
		return;
	file.close()
	
	var config = ConfigFile.new()
	var result = config.load(RehabGame.ConfigPath)
	if (result != Error.OK):
		return
	for b in config.get_section_keys("core"):
		Dict[b] = config.get_value("core", b)

func Save():
	var config = ConfigFile.new()
	for a in Dict:
		config.set_value("core", a, Dict[a])
	config.save(RehabGame.ConfigPath)

func Reset():
	Dict.clear()
	Setup()

func UpdateAll():
	var view = get_viewport()
	Dict[Key_Fullscreen] = int(DisplayServer.window_get_mode())
	Dict[Key_WindowSizeX] = get_window().size.x
	Dict[Key_WindowSizeY] = get_window().size.y
	Dict[Key_VSYNC] = int(DisplayServer.window_get_vsync_mode())
	Dict[Key_FXAA] = int(view.screen_space_aa)
	Dict[Key_MSAA] = int(view.msaa_3d)
	Dict[Key_FSR] = int(view.scaling_3d_mode)
	Dict[Key_RenderScale] = view.scaling_3d_scale
	Dict[Key_CameraH] = RehabGame.InvertCameraX
	Dict[Key_CameraV] = RehabGame.InvertCameraY
	Dict[Key_Lang] = TranslationServer.get_locale()
	Dict[Key_VolumeGlobal] = AudioServer.get_bus_volume_db(0)
	Dict[Key_VolumeMusic] = AudioServer.get_bus_volume_db(1)
	Dict[Key_VolumeSFX] = AudioServer.get_bus_volume_db(2)
	Dict[Key_VolumeVoice] = AudioServer.get_bus_volume_db(4)

func Update(key : String, val : Variant):
	Dict[key] = val

func Setup():
	if (Dict.has(Key_Fullscreen)):
		DisplayServer.window_set_mode(int(Dict[Key_Fullscreen]))
	if (Dict.has(Key_WindowSizeX) and Dict.has(Key_WindowSizeY)):
		get_window().size = Vector2(float(Dict[Key_WindowSizeX]), float(Dict[Key_WindowSizeY]))
		get_window().move_to_center()
	if (Dict.has(Key_VSYNC)):
		DisplayServer.window_set_vsync_mode(int(Dict[Key_VSYNC]))
	if (Dict.has(Key_FXAA)):
		get_viewport().screen_space_aa = Dict[Key_FXAA] as Viewport.ScreenSpaceAA
	if (Dict.has(Key_MSAA)):
		get_viewport().msaa_3d = Dict[Key_MSAA] as Viewport.MSAA
	if (Dict.has(Key_FSR)):
		get_viewport().scaling_3d_mode = Dict[Key_FSR] as Viewport.Scaling3DMode
	if (Dict.has(Key_RenderScale)):
		get_viewport().scaling_3d_scale = float(Dict[Key_RenderScale])
	if (Dict.has(Key_CameraH)):
		RehabGame.InvertCameraX = bool(Dict[Key_CameraH])
	if (Dict.has(Key_CameraV)):
		RehabGame.InvertCameraY = bool(Dict[Key_CameraV])
	if (Dict.has(Key_VolumeGlobal)):
		AudioServer.set_bus_volume_db(0, float(Dict[Key_VolumeGlobal]))
		if (float(Dict[Key_VolumeGlobal]) <= -25.0):
			AudioServer.set_bus_mute(0, true)
	if (Dict.has(Key_VolumeMusic)):
		AudioServer.set_bus_volume_db(1, float(Dict[Key_VolumeMusic]))
		if (float(Dict[Key_VolumeMusic]) <= -25.0):
			AudioServer.set_bus_mute(1, true)
	if (Dict.has(Key_VolumeSFX)):
		AudioServer.set_bus_volume_db(2, float(Dict[Key_VolumeSFX]))
		AudioServer.set_bus_volume_db(3, float(Dict[Key_VolumeSFX]))
		if (float(Dict[Key_VolumeSFX]) <= -25.0):
			AudioServer.set_bus_mute(2, true)
			AudioServer.set_bus_mute(3, true)
	if (Dict.has(Key_VolumeVoice)):
		AudioServer.set_bus_volume_db(4, float(Dict[Key_VolumeVoice]))
		if (float(Dict[Key_VolumeVoice]) <= -25.0):
			AudioServer.set_bus_mute(4, true)
	if (Dict.has(Key_Lang)):
		TranslationServer.set_locale(str(Dict[Key_Lang]))

const Key_Fullscreen = "fullscreen"
const Key_MSAA = "msaa"
const Key_FXAA = "fxaa"
const Key_VSYNC = "vsync"
const Key_FSR = "fsr"
const Key_RenderScale = "renderscale"
const Key_VolumeGlobal = "volume-global"
const Key_VolumeMusic = "volume-music"
const Key_VolumeSFX = "volume-sfx"
const Key_VolumeVoice = "volume-voice"
const Key_CameraH = "camera-h"
const Key_CameraV = "camera-v"
const Key_Lang = "lang"
const Key_WindowSizeX = "window-size-x"
const Key_WindowSizeY = "window-size-y"

