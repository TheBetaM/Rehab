using Godot;
namespace Rehab;
public partial class FrontendGameOver : Control
{

    TextureRect Icon;

    string[] IconPaths = [
        RehabGame.AssetsPath + "Textures/Language/GameOver/Crash.res", 
        RehabGame.AssetsPath + "Textures/Language/GameOver/Cortex.res",
        RehabGame.AssetsPath + "Textures/Language/GameOver/CrashAndCortex.res", 
        RehabGame.AssetsPath + "Textures/Language/GameOver/Nina.res", 
        RehabGame.AssetsPath + "Textures/Language/GameOver/Crash.res",
        RehabGame.AssetsPath + "Textures/Language/GameOver/Mecha.res"
    ];

    public override void _Ready()
    {
        Icon = GetNode<TextureRect>("TextureRect");
    }

    public void Activate()
    {
        RehabScene.Root.ProcessMode = ProcessModeEnum.Disabled;
        int iconID = 0;
        if (AgentCharacter.activeCharacter != null)
            iconID = (int)AgentCharacter.activeCharacter.CharType;
        if (ResourceLoader.Exists(IconPaths[iconID]))
            Icon.Texture = (Texture2D)ResourceLoader.Load(IconPaths[iconID]);
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.0f);
        GetNode<RehabMenuButton>("Button1").quiet = true;
        GetNode<RehabMenuButton>("Button1").GrabFocus();
        var fade = CreateTween();
        fade.TweenProperty(this, "modulate:a", 1.0f, 1.0f);
        Visible = true;
        ProcessMode = ProcessModeEnum.Always;
    }

    public void Go_Continue()
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
        RehabScene.Root.ProcessMode = ProcessModeEnum.Inherit;
        RehabScene.Root.ExitLevel(false);
    }

    public void Go_MainMenu()
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
        RehabScene.Root.ProcessMode = ProcessModeEnum.Inherit;
        RehabScene.Root.ExitLevel(true);
    }



}