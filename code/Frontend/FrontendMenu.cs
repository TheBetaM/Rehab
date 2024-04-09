using Godot;
using System;
using System.Collections.Generic;
namespace Rehab;
public partial class FrontendMenu : Control
{
    [Export] Theme LabelTheme;
    [Export] Material LabelMaterial;
    Control HeaderHolder;
    Label HeaderLabel;
    Label MainLabel;
    Control FooterHolder;
    TextureRect LevelIcon;
    TextureRect LevelIcon2;
    TextureRect WumpaIcon;
    TextureRect LivesIcon;
    TextureRect CrystalsIcon;
    Label WumpaText;
    Label LivesText;
    Label CrystalsText;
    TextureRect GemIcon1;
    TextureRect GemIcon2;
    TextureRect GemIcon3;
    TextureRect GemIcon4;
    TextureRect GemIcon5;
    TextureRect GemIcon6;
    VBoxContainer ExtrasList;
    TextureRect ExtrasItem;
    VideoStreamPlayer ExtrasItemVideo;

    float FadeTime = 0.25f;
    bool MenuActive;
    bool OptionsOnly;
    bool IconsInit;
    string CrystalIconPath = RehabGame.AssetsPath + "Textures/Icons/crystal_icon.tex";
    string WumpaIconPath = RehabGame.AssetsPath + "Textures/Icons/wumpa_icon.tex";
    string[] LivesIconPaths = [RehabGame.AssetsPath + "Textures/Icons/1up-crash.tex", RehabGame.AssetsPath + "Textures/Icons/1up-cortex.tex",
    RehabGame.AssetsPath + "Textures/Icons/1up-coco.tex", RehabGame.AssetsPath + "Textures/Icons/1up-nina.tex", 
    RehabGame.AssetsPath + "Textures/Icons/1up-evilcrash.tex", RehabGame.AssetsPath + "Textures/Icons/1up-mechabandicoot.tex"];
    string[] GemIconPaths = [
        RehabGame.AssetsPath + "Textures/Icons/gem-blue.tex",
        RehabGame.AssetsPath + "Textures/Icons/gem-clear.tex",
        RehabGame.AssetsPath + "Textures/Icons/gem-green.tex",
        RehabGame.AssetsPath + "Textures/Icons/gem-purple.tex",
        RehabGame.AssetsPath + "Textures/Icons/gem-red.tex",
        RehabGame.AssetsPath + "Textures/Icons/gem-yellow.tex",
    ];
    string EmptyGemIconPath = RehabGame.AssetsPath + "Textures/Icons/gem_greyed.tex";
    float BaseScale = 0.9f;
    int LastExtrasItem;

    public override void _Ready()
    {
        HeaderHolder = GetNode<Control>("WindowRoundHeader");
        HeaderLabel = GetNode<Label>("WindowRoundHeader/HeaderLabel");
        MainLabel = GetNode<Label>("WindowMainRound/RehabLabel");
        FooterHolder = GetNode<Control>("WindowRound");
        LevelIcon = GetNode<TextureRect>("WindowMainRound/LevelIcon");
        LevelIcon2 =  GetNode<TextureRect>("WindowMainRound/LevelIcon/LevelIcon2");
        WumpaIcon = GetNode<TextureRect>("WindowRoundWumpa/WumpaIcon");
        LivesIcon = GetNode<TextureRect>("WindowRoundLives/LivesIcon");
        CrystalsIcon = GetNode<TextureRect>("WindowRoundCrystals/CrystalsIcon");
        WumpaText = GetNode<Label>("WindowRoundWumpa/CountWumpa");
        LivesText = GetNode<Label>("WindowRoundLives/CountLives");
        CrystalsText = GetNode<Label>("WindowRoundCrystals/CountCrystals");
        GemIcon1 = GetNode<TextureRect>("WindowGems/GemIcon1");
        GemIcon2 = GetNode<TextureRect>("WindowGems/GemIcon2");
        GemIcon3 = GetNode<TextureRect>("WindowGems/GemIcon3");
        GemIcon4 = GetNode<TextureRect>("WindowGems/GemIcon4");
        GemIcon5 = GetNode<TextureRect>("WindowGems/GemIcon5");
        GemIcon6 = GetNode<TextureRect>("WindowGems/GemIcon6");
        ExtrasList = GetNode<VBoxContainer>("WindowMainRound/MenuExtrasList/VBoxContainer");
        ExtrasItem = GetNode<TextureRect>("WindowExtrasItem/TextureRect");
        ExtrasItemVideo = GetNode<VideoStreamPlayer>("WindowExtrasItem/ControlAspect/VideoStreamPlayer");
    }

    public void Full_AnimIn()
    {
        Visible = false;
        Scale = Vector2.Zero;
        PivotOffset = new Vector2(Size.X / 2f, PivotOffset.Y);
        Modulate = new Color(1f, 1f, 1f, 0f);
        Visible = true;
        var TargetScale = Vector2.One * BaseScale;
        var anim = CreateTween();
        anim.TweenProperty(this, "scale", TargetScale, FadeTime);
        var anim1 = CreateTween();
        anim1.TweenProperty(this, "modulate:a", 1f, FadeTime);
        if (!RehabScene.Root.XR_Enabled) Input.MouseMode = Input.MouseModeEnum.Visible;
        if (RehabScene.Root.XR_Enabled) RehabScene.Root.XR_Origin.FE_Active();
    }

    public void Full_AnimOut()
    {
        MenuActive = false;
        Scale = Vector2.One * BaseScale;
        PivotOffset = new Vector2(Size.X / 2f, PivotOffset.Y);
        Modulate = new Color(1f, 1f, 1f, 1f);
        var anim = CreateTween();
        anim.TweenProperty(this, "scale", Vector2.Zero, FadeTime);
        var anim1 = CreateTween();
        anim1.TweenProperty(this, "modulate:a", 0f, FadeTime);
        anim.TweenCallback(Callable.From(AnimOutEnd));
    }

    public void AnimOutEnd()
    {
        Visible = false;
    }

    public void InitIcons()
    {
        if (ResourceLoader.Exists(WumpaIconPath))
            WumpaIcon.Texture = (Texture2D)ResourceLoader.Load(WumpaIconPath);
        if (ResourceLoader.Exists(LivesIconPaths[0]))
            LivesIcon.Texture = (Texture2D)ResourceLoader.Load(LivesIconPaths[0]);
        if (ResourceLoader.Exists(CrystalIconPath))
            CrystalsIcon.Texture = (Texture2D)ResourceLoader.Load(CrystalIconPath);
    }

    public void Start_PauseMenu(bool optOnly)
    {
        OptionsOnly = optOnly;
        Full_AnimIn();
        ProcessMode = ProcessModeEnum.Always;
        GetTree().Paused = true;
        HeaderHolder.Visible = false;
        FooterHolder.Visible = true;
        HeaderLabel.Text = "#FE-Paused";
        MainLabel.Text = "";
        WumpaText.Text = RehabGame.Fruit.ToString();
        LivesText.Text = RehabGame.Lives.ToString();
        CrystalsText.Text = RehabGame.Crystals.ToString();
        GetNode<Control>("WindowRoundWumpa").Visible = true;
        GetNode<Control>("WindowRoundLives").Visible = true;
        GetNode<Control>("WindowRoundCrystals").Visible = true;
        GetNode<Control>("WindowGems").Visible = true;
        if (!IconsInit)
            InitIcons();
        SetLevelIcon();
        UpdateLives();
        LevelIcon.Visible = true;
        foreach (var i in GetNode<Control>("WindowRound").GetChildren())
        {
            if (i is VBoxContainer box)
                box.Visible = false;
        }
        GetNode<Control>("WindowRound/MenuPauseExplorer").Visible = true;
        MenuActive = true;
        if (optOnly)
        {
            Pause_ToOptions();
        }
        else
        {
            var holder = GetNode<Control>("WindowRound/MenuPauseExplorer");
            var button = (Control)holder.GetChild(holder.GetChildCount() - 1);
            button.GrabFocus();
        }
        if (OS.HasFeature("mobile"))
        {
            // No fullscreen switch on mobile, VSync switching doesn't work
            GetNode<Control>("WindowMainRound/MenuOptionsGraphics/Button1").Visible = false;
            GetNode<Control>("WindowMainRound/MenuOptionsGraphics/Button3").Visible = false;
        }
    }

    public void Start_Message(string text)
    {
        Full_AnimIn();
        ProcessMode = ProcessModeEnum.Always;
        GetTree().Paused = true;
        HeaderHolder.Visible = false;
        FooterHolder.Visible = true;
        MainLabel.Text = text;
        LevelIcon.Visible = false;
        GetNode<Control>("WindowRoundWumpa").Visible = false;
        GetNode<Control>("WindowRoundLives").Visible = false;
        GetNode<Control>("WindowRoundCrystals").Visible = false;
        GetNode<Control>("WindowGems").Visible = false;
        foreach (var i in GetNode<Control>("WindowRound").GetChildren())
        {
            if (i is VBoxContainer box)
                box.Visible = false;
        }
        var notice = GetNode<Control>("WindowRound/MenuNotice");
        notice.Visible = true;
        var button = (Control)notice.GetChild(0);
        button.GrabFocus();
        MenuActive = true;
    }

    public async void Notice_Close()
    {
        if (!MenuActive) return;
        GetTree().Paused = false;
        Full_AnimOut();
        await ToSignal(GetTree().CreateTimer(FadeTime), SceneTreeTimer.SignalName.Timeout);
        RehabScene.Root.ProcessMode = ProcessModeEnum.Inherit;
        ProcessMode = ProcessModeEnum.Inherit;
        if (AgentCharacter.activeCharacter != null)
        {
            if (RehabGame.UseMouseCamera)
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            if (RehabScene.Root.XR_Enabled) RehabScene.Root.XR_Origin.FE_Inactive();
            RehabScene.GameHUD.OnUnPause();
        }
        if (RehabScene.FE.GetNode<Control>("LevelSelect").Visible)
        {
            RehabScene.FE.GetNode<Control>("LevelSelect/AdvList/VBoxContainer").GetChild<Control>(0).GrabFocus();
        }
    }

    public void Pause_Resume()
    {
        Notice_Close();
    }

    public void Pause_QuitGame()
    {
        GetTree().Quit();
    }

    public void Pause_ReturnToLevelSelect()
    {
        MenuActive = false;
        Visible = false;
        RehabScene.Root.ProcessMode = ProcessModeEnum.Inherit;
        ProcessMode = ProcessModeEnum.Inherit;
        RehabScene.Root.ExitLevel(false);
        GetTree().Paused = false;
    }

    public void Pause_ReturnToMainMenu()
    {
        MenuActive = false;
        Visible = false;
        RehabScene.Root.ProcessMode = ProcessModeEnum.Inherit;
        ProcessMode = ProcessModeEnum.Inherit;
        RehabScene.Root.ExitLevel(true);
        GetTree().Paused = false;
    }

    public void Pause_ToOptions()
    {
        HeaderLabel.Text = "#FE-Options";
        HeaderHolder.Visible = true;
        LevelIcon.Visible = false;
        GetNode<Control>("WindowRoundWumpa").Visible = false;
        GetNode<Control>("WindowRoundLives").Visible = false;
        GetNode<Control>("WindowRoundCrystals").Visible = false;
        GetNode<Control>("WindowRound").Visible = false;
        GetNode<Control>("WindowGems").Visible = false;
        GetNode<Control>("WindowRound/MenuPauseExplorer").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = true;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsMain_ToGraphics()
    {
        HeaderLabel.Text = "#FE-GFXOptions";
        var scaling = GetViewport().Scaling3DScale;
        GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button7").Text = $"{Tr("#FE-RenderScale")}: {(int)Math.Round(scaling * 100)}%";
        var mset1 =  GetViewport().Scaling3DMode;
        if (mset1 == Viewport.Scaling3DModeEnum.Bilinear)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button6").Text = "#FE-FSR-Off";
        else if (mset1 == Viewport.Scaling3DModeEnum.Fsr)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button6").Text = "#FE-FSR-On1";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button6").Text = "#FE-FSR-On2";
        var mset2 = GetViewport().Msaa3D;
        if (mset2 == Viewport.Msaa.Disabled)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-Off";
        else if (mset2 == Viewport.Msaa.Msaa2X)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-2x";
        else if (mset2 == Viewport.Msaa.Msaa4X)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-4x";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-8x";
        var mset3 = GetViewport().ScreenSpaceAA;
        if (mset3 == Viewport.ScreenSpaceAAEnum.Disabled)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button4").Text = "#FE-FXAA-Off";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button4").Text = "#FE-FXAA-On";
        var mset4 = DisplayServer.WindowGetVsyncMode();
        if (mset4 == DisplayServer.VSyncMode.Disabled)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-Off";
        else if (mset4 == DisplayServer.VSyncMode.Enabled)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-On";
        else if (mset4 ==  DisplayServer.VSyncMode.Adaptive)
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-Adaptive";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-Fast";
        
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsGraphics").Visible = true;
        if (OS.HasFeature("mobile"))
        {
            GetNode<Control>("WindowMainRound/MenuOptionsGraphics").GetChild<Control>(1).GrabFocus();
        }
        else
        {
            GetNode<Control>("WindowMainRound/MenuOptionsGraphics").GetChild<Control>(0).GrabFocus();
        }
    }

    public void OptionsMain_ToGame()
    {
        HeaderLabel.Text = "#FE-GameOptions";
        if (RehabGame.InvertCameraX)
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button1").Text = "#FE-CamInvertH-On";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button1").Text = "#FE-CamInvertH-Off";
        if (RehabGame.InvertCameraY)
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button3").Text = "#FE-CamInvertV-On";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button3").Text = "#FE-CamInvertV-Off";
        if (RehabGame.UseMouseCamera)
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button5").Text = $"{Tr("#FE-MouseCamera")}: {Tr("#FE-On")}";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button5").Text = $"{Tr("#FE-MouseCamera")}: {Tr("#FE-Off")}";
        GetNode<Button>("WindowMainRound/MenuOptionsGame/Button6").Text = $"{Tr("#FE-MouseSensitivity")}: {Mathf.CeilToInt(RehabGame.MouseSensitivity * 100f)}%";
        GetNode<Button>("WindowMainRound/MenuOptionsGame/Button4").Text = $"{Tr("#FE-Language")}: {Tr("#FE-LanguageName")}";
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsGame").Visible = true;
        GetNode<Control>("WindowMainRound/MenuOptionsGame").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsMain_ToSound()
    {
        OptionsSound_ToggleVolume(0, true);
        OptionsSound_ToggleVolume(1, true);
        OptionsSound_ToggleVolume(2, true);
        OptionsSound_ToggleVolume(4, true);
        HeaderLabel.Text = "#FE-SFXOptions";
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsSound").Visible = true;
        UpdateVoiceLangText();
        GetNode<Control>("WindowMainRound/MenuOptionsSound").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsMain_ToPause()
    {
        RehabScene.Root.ConfigSave();
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = false;
        if (OptionsOnly)
        {
            Pause_Resume();
            RehabScene.FE.GetNode<MainMenuDynamic>("FE_MainMenuDynamic").ReturnOptions();
            return;
        }
        HeaderLabel.Text = "#FE-Paused";
        HeaderHolder.Visible = false;
        LevelIcon.Visible = true;
        GetNode<Control>("WindowRoundWumpa").Visible = true;
        GetNode<Control>("WindowRoundLives").Visible = true;
        GetNode<Control>("WindowRoundCrystals").Visible = true;
        GetNode<Control>("WindowGems").Visible = true;
        GetNode<Control>("WindowRound").Visible = true;
        GetNode<Control>("WindowRound/MenuPauseExplorer").Visible = true;
        GetNode<Control>("WindowRound/MenuPauseExplorer").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsGame_ToMain()
    {
        HeaderLabel.Text = "#FE-Options";
        GetNode<Control>("WindowMainRound/MenuOptionsGame").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = true;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsGraphics_ToMain()
    {
        HeaderLabel.Text = "#FE-Options";
        GetNode<Control>("WindowMainRound/MenuOptionsGraphics").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = true;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsSound_ToMain()
    {
        HeaderLabel.Text = "#FE-Options";
        GetNode<Control>("WindowMainRound/MenuOptionsSound").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = true;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsGraphics_ToggleFullscreen()
    {
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            GetWindow().Size = new Vector2I(1280, 720);
            GetWindow().MoveToCenter();
        }
    }

    public void OptionsGame_ToggleVibrations()
    {
        
    }

    public void OptionsGame_ToggleMouseCamera()
    {
        RehabGame.UseMouseCamera = !RehabGame.UseMouseCamera;
        if (RehabGame.UseMouseCamera)
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button5").Text = $"{Tr("#FE-MouseCamera")}: {Tr("#FE-On")}";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button5").Text = $"{Tr("#FE-MouseCamera")}: {Tr("#FE-Off")}";
    }

    public void OptionsGame_ToggleMouseSens()
    {
        RehabGame.MouseSensitivity -= 0.1f;
        if (RehabGame.MouseSensitivity <= 0f)
        {
            RehabGame.MouseSensitivity = 2f;
        }
        GetNode<Button>("WindowMainRound/MenuOptionsGame/Button6").Text = $"{Tr("#FE-MouseSensitivity")}: {Mathf.CeilToInt(RehabGame.MouseSensitivity * 100f)}%";
    }

    public void OptionsGraphics_ToggleMSAA()
    {
        var mset = GetViewport().Msaa3D;
        if (mset == Viewport.Msaa.Disabled)
        {
            GetViewport().Msaa3D = Viewport.Msaa.Msaa2X;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-2x";
        }
        else if (mset == Viewport.Msaa.Msaa2X)
        {
            GetViewport().Msaa3D = Viewport.Msaa.Msaa4X;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-4x";
        }
        else if (mset == Viewport.Msaa.Msaa4X)
        {
            GetViewport().Msaa3D = Viewport.Msaa.Msaa8X;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-8x";
        }
        else
        {
            GetViewport().Msaa3D = Viewport.Msaa.Disabled;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button2").Text = "#FE-MSAA-Off";
        }
        RehabScene.Root.MainMenu_UpdateViewport();
    }

    public void OptionsGraphics_ToggleTXAA()
    {
        var mset = GetViewport().ScreenSpaceAA;
        if (mset == Viewport.ScreenSpaceAAEnum.Disabled)
        {
            GetViewport().ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button4").Text = "#FE-FXAA-On";
        }
        else
        {
            GetViewport().ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button4").Text = "#FE-FXAA-Off";
        }
        RehabScene.Root.MainMenu_UpdateViewport();
    }

    public void OptionsGraphics_ToggleVSync()
    {
        if (RehabScene.Root.XR_Enabled) return;
        var mset = DisplayServer.WindowGetVsyncMode();
        if (mset == DisplayServer.VSyncMode.Disabled)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-On";
        }
        else if (mset == DisplayServer.VSyncMode.Enabled)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Adaptive);
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-Adaptive";
        }
        else if (mset == DisplayServer.VSyncMode.Adaptive)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Mailbox);
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-Fast";
        }
        else
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button3").Text = "#FE-VSync-Off";
        }
    }

    public void OptionsGraphics_ToggleFSR()
    {
        var mset = GetViewport().Scaling3DMode;
        if (mset == Viewport.Scaling3DModeEnum.Bilinear)
        {
            GetViewport().Scaling3DScale = 0.75f;
            GetViewport().Scaling3DMode = Viewport.Scaling3DModeEnum.Fsr;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button6").Text = "#FE-FSR-On1";
        }
        else if (mset == Viewport.Scaling3DModeEnum.Fsr)
        {
            GetViewport().Scaling3DMode = Viewport.Scaling3DModeEnum.Fsr2;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button6").Text = "#FE-FSR-On2";
        }
        else
        {
            GetViewport().Scaling3DScale = 1f;
            GetViewport().Scaling3DMode = Viewport.Scaling3DModeEnum.Bilinear;
            GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button6").Text = "#FE-FSR-Off";
        }
        
        var scaling = GetViewport().Scaling3DScale;
        GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button7").Text = $"{Tr("#FE-RenderScale")}: {(int)Math.Round(scaling * 100)}%";
        RehabScene.Root.MainMenu_UpdateViewport();
    }

    public void OptionsGraphics_ToggleRenderScale()
    {
        var mset = GetViewport().Scaling3DMode;
        var scaling = GetViewport().Scaling3DScale;
        if (scaling > 0.51f)
            scaling = scaling - 0.05f;
        else
        {
            if (mset == Viewport.Scaling3DModeEnum.Bilinear)
                scaling = 2f;
            else
                scaling = 1f;
        }
        GetViewport().Scaling3DScale = scaling;
        GetNode<Button>("WindowMainRound/MenuOptionsGraphics/Button7").Text = $"{Tr("#FE-RenderScale")}: {(int)Math.Round(scaling * 100)}%";
        RehabScene.Root.MainMenu_UpdateViewport();
    }

    public void OptionsSound_ToggleVolume_Global()
    {
        OptionsSound_ToggleVolume(0, false);
    }
    
    public void OptionsSound_ToggleVolume_Music()
    {
        OptionsSound_ToggleVolume(1, false);
    }

    public void OptionsSound_ToggleVolume_SFX()
    {
        OptionsSound_ToggleVolume(2, false);
	    OptionsSound_ToggleVolume(3, false);
    }

    public void OptionsSound_ToggleVolume_Voice()
    {
        OptionsSound_ToggleVolume(4, false);
    }

    public void OptionsSound_ToggleVoiceLanguage()
    {
        bool CanChangeLang = false;
        foreach (var mod in RehabGame.ModsInstalled)
        {
            if (mod.IsPAL)
            {
                CanChangeLang = true;
                break;
            }
        }
        if (!CanChangeLang) return;
        if (RehabGame.VoiceLang < RehabGame.VoiceLanguage.Spanish)
            RehabGame.VoiceLang++;
        else
            RehabGame.VoiceLang = RehabGame.VoiceLanguage.English;
        UpdateVoiceLangText();
    }
    void UpdateVoiceLangText()
    {
        string LangString = "#FE-Dub-English";
        switch (RehabGame.VoiceLang)
        {
            default: break;
            case RehabGame.VoiceLanguage.French: LangString = "#FE-Dub-French"; break;
            case RehabGame.VoiceLanguage.German: LangString = "#FE-Dub-German"; break;
            case RehabGame.VoiceLanguage.Italian: LangString = "#FE-Dub-Italian"; break;
            case RehabGame.VoiceLanguage.Spanish: LangString = "#FE-Dub-Spanish"; break;
            case RehabGame.VoiceLanguage.Japanese: LangString = "#FE-Dub-Japanese"; break;
        }
        GetNode<Button>("WindowMainRound/MenuOptionsSound/Button6").Text = $"{Tr("#FE-DubSelection")}: {Tr(LangString)}";
    }

    public void OptionsGame_ToggleCameraH()
    {
        RehabGame.InvertCameraX = !RehabGame.InvertCameraX;
        if (RehabGame.InvertCameraX)
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button1").Text = "#FE-CamInvertH-On";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button1").Text = "#FE-CamInvertH-Off";
    }

    public void OptionsGame_ToggleCameraV()
    {
        RehabGame.InvertCameraY = !RehabGame.InvertCameraY;
        if (RehabGame.InvertCameraY)
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button3").Text = "#FE-CamInvertV-On";
        else
            GetNode<Button>("WindowMainRound/MenuOptionsGame/Button3").Text = "#FE-CamInvertV-Off";
    }

    public void OptionsGame_ToggleLanguage()
    {
        var myloc = TranslationServer.GetLocale();
        var loc = TranslationServer.GetLoadedLocales();
        var dict = new List<string>();
        foreach (var i in loc)
        {
            if (!dict.Contains(i))
                dict.Add(i);
        }
        var iter = dict.IndexOf(myloc);
        if (iter >= dict.Count - 1)
            iter = 0;
        else
            iter = iter + 1;
        TranslationServer.SetLocale(dict[iter]);
        GetNode<Button>("WindowMainRound/MenuOptionsGame/Button4").Text = $"{Tr("#FE-Language")}: {Tr("#FE-LanguageName")}";
        SetLevelIcon();
    }

    public void OptionsGame_ToExtras()
    {
        HeaderLabel.Text = "#FE-Extras";
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = false;
        GetNode<Control>("WindowMainRound/MenuExtras").Visible = true;
        GetNode<Control>("WindowMainRound/MenuExtras").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsExtras_ToGame()
    {
        HeaderLabel.Text = "#FE-Options";
        GetNode<Control>("WindowMainRound/MenuExtras").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = true;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").GetChild<Control>(0).GrabFocus();
    }

    public void OptionsSound_ToggleVolume(int busID, bool textOnly)
    {
        var vol = AudioServer.GetBusVolumeDb(busID);
        var muted = AudioServer.IsBusMute(busID);
        string targetText = "";
        float targetVol;
        
        if (textOnly)
            vol += 2.6f;
        
        switch (busID)
        {
            case 0: targetText += Tr("#FE-GlobalVolume") + ": ";
            break; case 1: targetText += Tr("#FE-MusicVolume") + ": ";
            break; case 2: case 3: targetText += Tr("#FE-EffectsVolume") + ": ";
            break; case 4: targetText += Tr("#FE-VoiceVolume") + ": ";
            break; default: break;
        }
        
        if (muted)
        {
            if (!textOnly)
            {
                AudioServer.SetBusVolumeDb(busID, 0f);
                AudioServer.SetBusMute(busID, false);
                targetText += "100%";
            }
            else
            {
                targetText += "0%";
            }
            switch (busID)
            {
                case 0: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button1").Text = targetText;
                break; case 1: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button3").Text = targetText;
                break; case 2: case 3: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button2").Text = targetText;
                break; case 4: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button4").Text = targetText;
                break; default: break;
            }
            return;
        }
        
        if (vol >= 2.5f)
        {
            targetVol = -2.5f;
            targetText += "100%";
        }
        else if (vol >= 0f)
        {
            targetVol = -2.5f;
            targetText += "90%";
        }
        else if (vol >= -2.6f)
        {
            targetVol = -5.0f;
            targetText += "80%";
        }
        else if (vol >= -5.1f)
        {
            targetVol = -7.5f;
            targetText += "70%";
        }
        else if (vol >= -7.6f)
        {
            targetVol = -10.0f;
            targetText += "60%";
        }
        else if (vol >= -10.1f)
        {
            targetVol = -12.5f;
            targetText += "50%";
        }
        else if (vol >= -12.6f)
        {
            targetVol = -15.0f;
            targetText += "40%";
        }
        else if (vol >= -15.1f)
        {
            targetVol = -17.5f;
            targetText += "30%";
        }
        else if (vol >= -17.6f)
        {
            targetVol = -20.0f;
            targetText += "20%";
        }
        else if (vol >= -20.1f)
        {
            targetVol = -22.5f;
            targetText += "10%";
        }
        else
        {
            targetVol = -30f;
            if (!textOnly)
                AudioServer.SetBusMute(busID, true);
            targetText += "0%";
        }
        
        if (!textOnly)
            AudioServer.SetBusVolumeDb(busID, targetVol);
        
        switch (busID)
        {
            case 0: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button1").Text = targetText;
            break; case 1: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button3").Text = targetText;
            break; case 2: case 3: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button2").Text = targetText;
            break; case 4: GetNode<Button>("WindowMainRound/MenuOptionsSound/Button4").Text = targetText;
            break; default: break;
        }
    }

    public void SetLevelIcon()
    {
        var LevelID = RehabGame.LevelID;
        var IconPath = "";

        string langPath = "English";
        bool CanChangeLang = false;
        foreach (var mod in RehabGame.ModsInstalled)
        {
            if (mod.IsPAL)
            {
                CanChangeLang = true;
                break;
            }
        }
        if (CanChangeLang)
        {
            switch (TranslationServer.GetLocale())
            {
                default: break;
                case "de_DE":
                case "de-DE":
                case "de":
                    langPath = "German";
                break;
                case "fr_FR":
                case "fr-FR":
                case "fr":
                    langPath = "French";
                break;
                case "it_IT":
                case "it-IT":
                case "it":
                    langPath = "Italian";
                break;
                case "es_ES":
                case "es-ES":
                case "es":
                    langPath = "Spanish";
                break;
            }
        }
        
        switch (LevelID)
        {
            case 0: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Hub01.tex";
            break; case 1: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level01.tex";
            break; case 3: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level02.tex";
            break; case 4: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level03.tex";
            break; case 6: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Hub02.tex";
            break; case 7: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level04.tex";
            break; case 9: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level05.tex";
            break; case 10: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level06.tex";
            break; case 13: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Hub03.tex";
            break; case 15: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level08.tex";
            break; case 17: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level09.tex";
            break; case 18: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level10.tex";
            break; case 20: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Hub04.tex";
            break; case 21: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level11.tex";
            break; case 22: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level12.tex";
            break; case 23: IconPath = $"{RehabGame.AssetsPath}Textures/Language/Titles/{langPath}/Level13.tex";
            break; default: break;
        }
        
        if (IconPath != "" && ResourceLoader.Exists(IconPath))
        {
            LevelIcon.Texture = (Texture2D)ResourceLoader.Load(IconPath);
            LevelIcon2.Texture = LevelIcon.Texture;
        }
        else
        {
            LevelIcon.Texture = null;
            LevelIcon2.Texture = null;
        }
    }

    public void UpdateLives()
    {
        var iconID = 0;
        if (AgentCharacter.activeCharacter != null)
            iconID = (int)AgentCharacter.activeCharacter.CharType;
        if (ResourceLoader.Exists(LivesIconPaths[iconID]))
            LivesIcon.Texture = (Texture2D)ResourceLoader.Load(LivesIconPaths[iconID]);
        
        if (ResourceLoader.Exists(EmptyGemIconPath))
        {
            var tex = (Texture2D)ResourceLoader.Load(EmptyGemIconPath);
            GemIcon1.Texture = tex;
            GemIcon2.Texture = tex;
            GemIcon3.Texture = tex;
            GemIcon4.Texture = tex;
            GemIcon5.Texture = tex;
            GemIcon6.Texture = tex;
        }
        if (RehabGame.Gems.Count != 0)
        {
            if (RehabGame.Gems.ContainsKey(RehabGame.LevelID))
            {
                if (RehabGame.Gems[RehabGame.LevelID].Contains(0) && ResourceLoader.Exists(GemIconPaths[0]))
                    GemIcon1.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[0]);
                if (RehabGame.Gems[RehabGame.LevelID].Contains(1) && ResourceLoader.Exists(GemIconPaths[1]))
                    GemIcon2.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[1]);
                if (RehabGame.Gems[RehabGame.LevelID].Contains(2) && ResourceLoader.Exists(GemIconPaths[2]))
                    GemIcon3.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[2]);
                if (RehabGame.Gems[RehabGame.LevelID].Contains(3) && ResourceLoader.Exists(GemIconPaths[3]))
                    GemIcon4.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[3]);
                if (RehabGame.Gems[RehabGame.LevelID].Contains(4) && ResourceLoader.Exists(GemIconPaths[4]))
                    GemIcon5.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[4]);
                if (RehabGame.Gems[RehabGame.LevelID].Contains(5) && ResourceLoader.Exists(GemIconPaths[5]))
                    GemIcon6.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[5]);
            }
        }
    }

    public void Extras_ToBlue() => ExtrasListStart(0);
    public void Extras_ToClear() => ExtrasListStart(1);
    public void Extras_ToGreen() => ExtrasListStart(2);
    public void Extras_ToPurple() => ExtrasListStart(3);
    public void Extras_ToRed() => ExtrasListStart(4);
    public void Extras_ToYellow() => ExtrasListStart(5);
    public void Extras_ToComplete() => ExtrasListStart(6);
    public void Go_Mods() => ExtrasListStart(7);

    public void ExtrasListStart(int type)
    {
        var first = false;
        Button prefab = null;
        foreach (var i in ExtrasList.GetChildren())
        {
            if (!first)
            {
                first = true;
                prefab = (Button)i;
            }
            else
            {
                i.QueueFree();
            }
        }
        string headerName = "#FE-Extras";
        string dirPath;
        DirAccess dir;

        switch (type)
        {
            case 0:
                headerName = "#FE-GemExtrasBlue";
                dirPath = RehabGame.AssetsPath + "Textures/Extras/Bosses/";
                dir = DirAccess.Open(dirPath);
                if (dir != null)
                {
                    var iter = 0;
                    foreach (var i in dir.GetFiles())
                    {
                        iter += 1;
                        Button inst = (Button)prefab.Duplicate();
                        inst.Text = $"{Tr("#FE-BossExtras")} {iter}";
                        inst.Disconnect("pressed", Callable.From(ExtrasList_ToExtras));
                        inst.Connect("pressed", Callable.From(() => ExtrasItemStart(dirPath + i)));
                        ExtrasList.AddChild(inst);
                    }
                }
                else GD.Print("[EXTRAS] Cannot open folder."); break;
            case 1:
                headerName = "#FE-GemExtrasClear";
                dirPath = RehabGame.AssetsPath + "Movies/";
                dir = DirAccess.Open(dirPath);
                if (dir != null)
                {
                    var iter = 0;
                    foreach (var i in dir.GetFiles())
                    {
                        iter += 1;
                        Button inst = (Button)prefab.Duplicate();
                        inst.Text = $"{Tr("#FE-MovieExtras")} {iter}";
                        inst.Disconnect("pressed", Callable.From(ExtrasList_ToExtras));
                        inst.Connect("pressed", Callable.From(() => ExtrasItemStartMovie(dirPath + i)));
                        ExtrasList.AddChild(inst);
                    }
                }
                else GD.Print("[EXTRAS] Cannot open folder."); break;
            case 2:
                headerName = "#FE-GemExtrasGreen";
                dirPath = RehabGame.AssetsPath + "Textures/Extras/Concept/";
                dir = DirAccess.Open(dirPath);
                if (dir != null)
                {
                    var iter = 0;
                    foreach (var i in dir.GetFiles())
                    {
                        iter += 1;
                        Button inst = (Button)prefab.Duplicate();
                        inst.Text = $"{Tr("#FE-ConceptExtras")} {iter}";
                        inst.Disconnect("pressed", Callable.From(ExtrasList_ToExtras));
                        inst.Connect("pressed", Callable.From(() => ExtrasItemStart(dirPath + i)));
                        ExtrasList.AddChild(inst);
                    }
                }
                else GD.Print("[EXTRAS] Cannot open folder."); break;
            case 3:
                headerName = "#FE-GemExtrasPurple";
                dirPath = RehabGame.AssetsPath + "Textures/Extras/Storyboards/01-NSanity/";
                dir = DirAccess.Open(dirPath);
                if (dir != null)
                {
                    var iter = 0;
                    foreach (var i in dir.GetFiles())
                    {
                        iter += 1;
                        Button inst = (Button)prefab.Duplicate();
                        inst.Text = $"{Tr("#FE-ConceptExtras")} {iter}";
                        inst.Disconnect("pressed", Callable.From(ExtrasList_ToExtras));
                        inst.Connect("pressed", Callable.From(() => ExtrasItemStart(dirPath + i)));
                        ExtrasList.AddChild(inst);
                    }
                }
                else GD.Print("[EXTRAS] Cannot open folder."); break;
            case 4:
                headerName = "#FE-GemExtrasRed";
                dirPath = RehabGame.AssetsPath + "Textures/Extras/Enemies/";
                dir = DirAccess.Open(dirPath);
                if (dir != null)
                {
                    var iter = 0;
                    foreach (var i in dir.GetFiles())
                    {
                        iter += 1;
                        Button inst = (Button)prefab.Duplicate();
                        inst.Text = $"{Tr("#FE-EnemyExtras")} {iter}";
                        inst.Disconnect("pressed", Callable.From(ExtrasList_ToExtras));
                        inst.Connect("pressed", Callable.From(() => ExtrasItemStart(dirPath + i)));
                        ExtrasList.AddChild(inst);
                    }
                }
                else GD.Print("[EXTRAS] Cannot open folder."); break;
            case 5:
                headerName = "#FE-GemExtrasYellow";
                dirPath = RehabGame.AssetsPath + "Textures/Extras/Unseen/";
                dir = DirAccess.Open(dirPath);
                if (dir != null)
                {
                    var iter = 0;
                    foreach (var i in dir.GetFiles())
                    {
                        iter += 1;
                        Button inst = (Button)prefab.Duplicate();
                        inst.Text = $"{Tr("#FE-UnseenExtras")} {iter}";
                        inst.Disconnect("pressed", Callable.From(ExtrasList_ToExtras));
                        inst.Connect("pressed", Callable.From(() => ExtrasItemStart(dirPath + i)));
                        ExtrasList.AddChild(inst);
                    }
                }
                else GD.Print("[EXTRAS] Cannot open folder."); break;
            case 6:
                headerName = "#FE-CompleteExtras";
                dirPath = RehabGame.AssetsPath + "Textures/Language/Loading/";
                dir = DirAccess.Open(dirPath);
                if (dir != null)
                {
                    var iter = 0;
                    foreach (var i in dir.GetFiles())
                    {
                        iter += 1;
                        Button inst = (Button)prefab.Duplicate();
                        inst.Text = $"{Tr("#FE-CompleteExtras")} {iter}";
                        inst.Disconnect("pressed", Callable.From(ExtrasList_ToExtras));
                        inst.Connect("pressed", Callable.From(() => ExtrasItemStart(dirPath + i)));
                        ExtrasList.AddChild(inst);
                    }
                }
                else GD.Print("[EXTRAS] Cannot open folder."); break;
            case 7:
                headerName = "#FE-Explorer-ModsInstalled";
                foreach (var i in RehabGame.ModsInstalled)
                {
                    Button inst = (Button)prefab.Duplicate();
                    inst.Text = i.Name;
                    ExtrasList.AddChild(inst);
                }
                break;
            default: break;
        }

        HeaderLabel.Text = headerName;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = false;
        GetNode<Control>("WindowMainRound/MenuExtras").Visible = false;
        GetNode<Control>("WindowMainRound/MenuExtrasList").Visible = true;
        ExtrasList.GetChild<Control>(0).GrabFocus();
    }

    public void ExtrasList_ToExtras()
    {
        HeaderLabel.Text = "#FE-Extras";
        GetNode<Control>("WindowMainRound/MenuExtrasList").Visible = false;
        GetNode<Control>("WindowMainRound/MenuExtras").Visible = true;
        GetNode<Control>("WindowMainRound/MenuExtras").GetChild<Control>(0).GrabFocus();
        var first = false;
        foreach (var i in ExtrasList.GetChildren())
        {
            if (!first)
                first = true;
            else
                i.QueueFree();
        }
    }

    public void ExtrasItemStart(string path)
    {
        if (!ResourceLoader.Exists(path)) return;
        ExtrasItem.Texture = (Texture2D)ResourceLoader.Load(path);
        var iter = 0;
        foreach (var i in ExtrasList.GetChildren())
        {
            if (i is Control cont && cont.HasFocus())
            {
                LastExtrasItem = iter;
                break;
            }
            iter += 1;
        }
        ExtrasList.Visible = false;
        GetNode<Control>("WindowExtrasItem").Modulate = new Color(1f, 1f, 1f, 0f);
        GetNode<Control>("WindowExtrasItem").Visible = true;
        var aTween = CreateTween();
        aTween.TweenProperty(GetNode<Control>("WindowExtrasItem"),"modulate:a", 1.0, 0.5);
        GetNode<Control>("WindowExtrasItem/Button").GrabFocus();
    }

    public void ExtrasItemStartMovie(string path)
    {
        if (!ResourceLoader.Exists(path)) return;
        ExtrasItemVideo.Stream = (VideoStream)ResourceLoader.Load(path);
        var iter = 0;
        foreach (var i in ExtrasList.GetChildren())
        {
            if (i is Control cont && cont.HasFocus())
            {
                LastExtrasItem = iter;
                break;
            }
            iter += 1;
        }
        ExtrasList.Visible = false;
        GetNode<Control>("WindowExtrasItem").Modulate = new Color(1f, 1f, 1f, 0f);
        GetNode<Control>("WindowExtrasItem").Visible = true;
        ExtrasItemVideo.Play();
        GetNode<Control>("WindowExtrasItem/Button").GrabFocus();
    }

    public void ExitExtrasItem()
    {
        if (GetNode<Control>("WindowExtrasItem").Modulate.A < 0.95f) return;
        GetNode<Control>("WindowExtrasItem").Visible = false;
        if (ExtrasItemVideo.IsPlaying())
        {
            ExtrasItemVideo.Stop();
            ExtrasItemVideo.Stream = null;
        }
        ExtrasList.Visible = true;
        ExtrasList.GetChild<Control>(LastExtrasItem).GrabFocus();
    }

    public void LongTextExit()
    {
        HeaderLabel.Text = "#FE-Options";
        GetNode<Control>("WindowMainRound/MenuLongText").Visible = false;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = true;
        GetNode<Control>("WindowMainRound/MenuOptionsMain").GetChild<Control>(0).GrabFocus();
    }

    public void Go_License()
    {
        HeaderLabel.Text = "#FE-Explorer-ViewLicense";
        GetNode<Control>("WindowMainRound/MenuOptionsMain").Visible = false;
        GetNode<Control>("WindowMainRound/MenuLongText").Visible = true;
        var label = GetNode<Control>("WindowMainRound/MenuLongText").GetChild<RichTextLabel>(0);
        TextResource file = (TextResource)ResourceLoader.Load("res://assets/lang/license.tres");
        label.Text = file.text;
        GetNode<Control>("WindowMainRound/MenuLongText").GetChild<Control>(1).GrabFocus();
    }
}