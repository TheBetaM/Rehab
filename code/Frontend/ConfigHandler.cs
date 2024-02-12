using Godot;
using System.Collections.Generic;
namespace Rehab;
public partial class ConfigHandler : Node
{
    public Dictionary<string, Variant> Dict = new();

    public void Load()
    {
        if (!FileAccess.FileExists(RehabGame.ConfigPath)) return;
        var file = FileAccess.Open(RehabGame.ConfigPath, FileAccess.ModeFlags.Read);
        var text = file.GetAsText().ToLower();
        if (text.Contains("object") || text.Contains("refcounted") || text.Contains("script"))
        {
            file.Close();
            return;
        }
        file.Close();
        
        var config = new ConfigFile();
        var result = config.Load(RehabGame.ConfigPath);
        if (result != Error.Ok)
            return;
        foreach (var b in config.GetSectionKeys("core"))
            Dict[b] = config.GetValue("core", b);
    }

    public void Save()
    {
        var config = new ConfigFile();
        foreach (var a in Dict)
            config.SetValue("core", a.Key, a.Value);
        config.Save(RehabGame.ConfigPath);
    }

    public void Reset()
    {
        Dict.Clear();
        Setup();
    }

    public void UpdateAll()
    {
        var view = GetViewport();
        Dict[Key_Fullscreen] = (int)DisplayServer.WindowGetMode();
        Dict[Key_WindowSizeX] = GetWindow().Size.X;
        Dict[Key_WindowSizeY] = GetWindow().Size.Y;
        Dict[Key_VSYNC] = (int)DisplayServer.WindowGetVsyncMode();
        Dict[Key_FXAA] = (int)view.ScreenSpaceAA;
        Dict[Key_MSAA] = (int)view.Msaa3D;
        Dict[Key_FSR] = (int)view.Scaling3DMode;
        Dict[Key_RenderScale] = view.Scaling3DScale;
        Dict[Key_CameraH] = RehabGame.InvertCameraX;
        Dict[Key_CameraV] = RehabGame.InvertCameraY;
        Dict[Key_Lang] = TranslationServer.GetLocale();
        Dict[Key_VolumeGlobal] = AudioServer.GetBusVolumeDb(0);
        Dict[Key_VolumeMusic] = AudioServer.GetBusVolumeDb(1);
        Dict[Key_VolumeSFX] = AudioServer.GetBusVolumeDb(2);
        Dict[Key_VolumeVoice] = AudioServer.GetBusVolumeDb(4);
    }

    public void Update(string key, Variant val)
    {
        Dict[key] = val;
    }

    public void Setup()
    {
        if (Dict.ContainsKey(Key_Fullscreen))
            DisplayServer.WindowSetMode((DisplayServer.WindowMode)(int)Dict[Key_Fullscreen]);
        if (Dict.ContainsKey(Key_WindowSizeX) && Dict.ContainsKey(Key_WindowSizeY))
        {
            GetWindow().Size = new Vector2I((int)Dict[Key_WindowSizeX], (int)Dict[Key_WindowSizeY]);
            GetWindow().MoveToCenter();
        }
        if (Dict.ContainsKey(Key_VSYNC))
            DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)(int)Dict[Key_VSYNC]);
        if (Dict.ContainsKey(Key_FXAA))
            GetViewport().ScreenSpaceAA = (Viewport.ScreenSpaceAAEnum)(int)Dict[Key_FXAA];
        if (Dict.ContainsKey(Key_MSAA))
            GetViewport().Msaa3D = (Viewport.Msaa)(int)Dict[Key_MSAA];
        if (Dict.ContainsKey(Key_FSR))
            GetViewport().Scaling3DMode = (Viewport.Scaling3DModeEnum)(int)Dict[Key_FSR];
        if (Dict.ContainsKey(Key_RenderScale))
            GetViewport().Scaling3DScale = (float)Dict[Key_RenderScale];
        if (Dict.ContainsKey(Key_CameraH))
            RehabGame.InvertCameraX = (bool)Dict[Key_CameraH];
        if (Dict.ContainsKey(Key_CameraV))
            RehabGame.InvertCameraY = (bool)Dict[Key_CameraV];
        if (Dict.ContainsKey(Key_VolumeGlobal))
        {
            AudioServer.SetBusVolumeDb(0, (float)Dict[Key_VolumeGlobal]);
            if ((float)Dict[Key_VolumeGlobal] <= -25.0f)
                AudioServer.SetBusMute(0, true);
        }
        if (Dict.ContainsKey(Key_VolumeMusic))
        {
            AudioServer.SetBusVolumeDb(1, (float)Dict[Key_VolumeMusic]);
            if ((float)Dict[Key_VolumeMusic] <= -25.0f)
                AudioServer.SetBusMute(1, true);
        }
        if (Dict.ContainsKey(Key_VolumeSFX))
        {
            AudioServer.SetBusVolumeDb(2, (float)Dict[Key_VolumeSFX]);
            AudioServer.SetBusVolumeDb(3, (float)Dict[Key_VolumeSFX]);
            if ((float)Dict[Key_VolumeSFX] <= -25.0f)
            {
                AudioServer.SetBusMute(2, true);
                AudioServer.SetBusMute(3, true);
            }
        }
        if (Dict.ContainsKey(Key_VolumeVoice))
        {
            AudioServer.SetBusVolumeDb(4, (float)Dict[Key_VolumeVoice]);
            if ((float)Dict[Key_VolumeVoice] <= -25.0f)
                AudioServer.SetBusMute(4, true);
        }
        if (Dict.ContainsKey(Key_Lang))
            TranslationServer.SetLocale((string)Dict[Key_Lang]);
    }

    const string Key_Fullscreen = "fullscreen";
    const string Key_MSAA = "msaa";
    const string Key_FXAA = "fxaa";
    const string Key_VSYNC = "vsync";
    const string Key_FSR = "fsr";
    const string Key_RenderScale = "renderscale";
    const string Key_VolumeGlobal = "volume-global";
    const string Key_VolumeMusic = "volume-music";
    const string Key_VolumeSFX = "volume-sfx";
    const string Key_VolumeVoice = "volume-voice";
    const string Key_CameraH = "camera-h";
    const string Key_CameraV = "camera-v";
    const string Key_Lang = "lang";
    const string Key_WindowSizeX = "window-size-x";
    const string Key_WindowSizeY = "window-size-y";
}