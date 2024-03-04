using Godot;
namespace Rehab;
public partial class MainMenuDynamic : Control
{
    Node3D Root3D;
    Camera3D CamRoot3D;
    Viewport Viewport3D;
    string ActorPath = "res://assets/frontend/dynamic/FE_Actors.tscn";
    bool ActorsExist;
    PackedScene ActorScene;
    Node3D ActorNode;
    string AudioPath = RehabGame.AssetsPath + "Sounds/VO/Cortex_Panic_08.res";
    string[] RequiredAssets = [
        RehabGame.AssetsPath + "Rigs/Rig_Crash.tscn",
        RehabGame.AssetsPath + "Rigs/Rig_Cortex.tscn",
        RehabGame.AssetsPath + "Rigs/RigRESET_Crash.res",
        RehabGame.AssetsPath + "Rigs/RigRESET_Cortex.res",
    ];
    string DemoCheck = RehabGame.AssetsPath + "Animations/Cortex_SkateFall.res";

    public override void _Ready()
    {
        Root3D = GetNode<Node3D>("ViewHolder/SubViewportContainer/SubViewport/FE_ROOT");
        CamRoot3D = GetNode<Camera3D>("ViewHolder/SubViewportContainer/SubViewport/Camera3D");
        Viewport3D = GetNode<Viewport>("ViewHolder/SubViewportContainer/SubViewport");
    }

    public void LoadActors()
    {
        ActorsExist = true;
        foreach (var i in RequiredAssets)
        {
            if (!ResourceLoader.Exists(i))
                ActorsExist = false;
                break;
        }

        if (ActorsExist)
        {
            ActorScene = (PackedScene)ResourceLoader.Load(ActorPath);
            ActorNode = (Node3D)ActorScene.Instantiate();
            Root3D.AddChild(ActorNode);
        }
    }

    public async void StartAnim()
    {
        Visible = false;
        if (!ActorsExist)
        {
            LoadActors();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        if (ActorsExist)
        {
            if (ResourceLoader.Exists(AudioPath))
            {
                GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stream = (AudioStream)ResourceLoader.Load(AudioPath);
                GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            }
            if (ResourceLoader.Exists(DemoCheck))
            {
                ActorNode.GetNode<AnimationPlayer>("AnimationPlayer").Play("scene/menu_start");
            }
            else
            {
                ActorNode.GetNode<AnimationPlayer>("AnimationPlayer").Play("scene/menu_start1");
            }
        }
        GetNode<AnimationPlayer>("AnimationPlayer").Play("menu_start");
        Visible = true;
    }

    public async void Activate()
    {
        UpdateViewport();
        GetNode<Control>("Button1").Visible = false;
        GetNode<Control>("Button2").Visible = false;
        GetNode<Control>("Button3").Visible = false;
        GetNode<Control>("Button4").Visible = false;
        CamRoot3D.Visible = true;
        CamRoot3D.ProcessMode = ProcessModeEnum.Inherit;
        Root3D.Visible = true;
        Root3D.ProcessMode = ProcessModeEnum.Inherit;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("RESET");
        if (ActorsExist)
            ActorNode.GetNode<AnimationPlayer>("AnimationPlayer").Play("RESET");
        ProcessMode = ProcessModeEnum.Inherit;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Visible = true;
        StartAnim();
        RehabScene.Root.PlayMusic(54);
        await ToSignal(GetTree().CreateTimer(1.86f), SceneTreeTimer.SignalName.Timeout);
        GetNode<RehabMenuButton>("Button1").quiet = true;
        GetNode<Control>("Button1").GrabFocus();
        GetNode<RehabMenuButton>("Button1").quiet = false;
        GetNode<Control>("Button1").Visible = true;
        GetNode<Control>("Button2").Visible = true;
        GetNode<Control>("Button3").Visible = true;
        GetNode<Control>("Button4").Visible = true;
    }

    public async void Go_LevelSelect()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        RehabScene.Root.StartLevelSelect();
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        Visible = false;
        CamRoot3D.Visible = false;
        CamRoot3D.ProcessMode = ProcessModeEnum.Disabled;
        Root3D.Visible = false;
        Root3D.ProcessMode = ProcessModeEnum.Disabled;
    }

    public void Go_Options()
    {
        GetNode<Control>("Button1").FocusMode = FocusModeEnum.None;
        GetNode<Control>("Button2").FocusMode = FocusModeEnum.None;
        GetNode<Control>("Button3").FocusMode = FocusModeEnum.None;
        GetNode<Control>("Button4").FocusMode = FocusModeEnum.None;
        RehabScene.Root.StartPauseMenu(true);
    }

    public void Go_Credits()
    {
        RehabScene.Root.PlayCredits();
    }

    public void Go_QuitGame()
    {
        GetTree().Quit();
    }

    public void ReturnOptions()
    {
        GetNode<Control>("Button1").FocusMode = FocusModeEnum.All;
        GetNode<Control>("Button2").FocusMode = FocusModeEnum.All;
        GetNode<Control>("Button3").FocusMode = FocusModeEnum.All;
        GetNode<Control>("Button4").FocusMode = FocusModeEnum.All;
        GetNode<Control>("Button2").GrabFocus();
    }

    public void UpdateViewport()
    {
        var view = GetViewport();
        //Viewport3D.Scaling3DMode = view.Scaling3DMode;
        Viewport3D.Scaling3DScale = view.Scaling3DScale;
        Viewport3D.Msaa3D = view.Msaa3D;
        Viewport3D.ScreenSpaceAA = view.ScreenSpaceAA;
    }


}