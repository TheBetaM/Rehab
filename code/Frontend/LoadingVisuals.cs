using Godot;
namespace Rehab;
public partial class LoadingVisuals : Control
{
    TextureRectWobble LevelIcon;
    TextureRect LevelIcon2;
    Node3D Root3D;
    Camera3D CamRoot3D;
    Viewport Viewport3D;
    string ActorPath = "res://assets/frontend/dynamic/FE_Actors.tscn";
    bool ActorsExist = false;
    PackedScene ActorScene;
    Node3D ActorNode;
    string[] RequiredAssets = [
        RehabGame.AssetsPath + "Rigs/Rig_Crash.tscn",
        RehabGame.AssetsPath + "Rigs/Rig_Cortex.tscn",
        RehabGame.AssetsPath + "Rigs/RigRESET_Crash.res",
        RehabGame.AssetsPath + "Rigs/RigRESET_Cortex.res",
    ];
    Material LoadMat1;

    public override void _Ready()
    {
        LevelIcon = GetNode<TextureRectWobble>("LevelIcon");
        LevelIcon2 = GetNode<TextureRect>("LevelIcon/LevelIcon2");

        Root3D = GetNode<Node3D>("ViewHolder/SubViewportContainer/SubViewport/FE_ROOT");
        CamRoot3D = GetNode<Camera3D>("ViewHolder/SubViewportContainer/SubViewport/Camera3D");
        Viewport3D = GetNode<Viewport>("ViewHolder/SubViewportContainer/SubViewport");
        LoadMat1 = (Material)ResourceLoader.Load("res://assets/frontend/dynamic/SolidColorWhite.tres");
    }

    public void LoadActors()
    {
        ActorsExist = true;
        foreach (var i in RequiredAssets)
        {
            if (!ResourceLoader.Exists(i))
            {
                ActorsExist = false;
                break;
            }
        }
        
        if (ActorsExist)
        {
            ActorScene = (PackedScene)ResourceLoader.Load(ActorPath);
            ActorNode = (Node3D)ActorScene.Instantiate();
            Root3D.AddChild(ActorNode);
            UpdateActorMat(ActorNode);
        }
    }

    public override void _Process(double delta)
    {
        if (!ActorsExist) return;
	
        float camdirX = 0f;
        float _camdirY = 0f;
        float oldX = Root3D.RotationDegrees.X;
        float oldY = Root3D.RotationDegrees.Y;
        
        camdirX += Input.GetActionStrength("pad1_dpad_right");
        camdirX -= Input.GetActionStrength("pad1_dpad_left");
        if (Input.IsActionPressed("pad1_rstick_left"))
            if (!RehabGame.InvertCameraX)
                camdirX -= Input.GetActionStrength("pad1_rstick_left");
            else
                camdirX += Input.GetActionStrength("pad1_rstick_left");
        if (Input.IsActionPressed("pad1_rstick_right"))
            if (!RehabGame.InvertCameraX)
                camdirX += Input.GetActionStrength("pad1_rstick_right");
            else
                camdirX -= Input.GetActionStrength("pad1_rstick_right");
        if (Input.IsActionPressed("pad1_rstick_up"))
            if (!RehabGame.InvertCameraY)
                _camdirY += Input.GetActionStrength("pad1_rstick_up");
            else
                _camdirY -= Input.GetActionStrength("pad1_rstick_up");
        if (Input.IsActionPressed("pad1_rstick_down"))
            if (!RehabGame.InvertCameraY)
                _camdirY -= Input.GetActionStrength("pad1_rstick_down");
            else
                _camdirY += Input.GetActionStrength("pad1_rstick_down");
        
        //oldX = oldX + (camdirY * (float)delta * 45.0f);
        oldY = oldY + (camdirX * (float)delta * 45.0f);
        Root3D.RotationDegrees = new Vector3(oldX, oldY, 0);
    }

    public void UpdateActorMat(Node parent)
    {
        if (parent is VisualInstance3D vis)
            vis.Layers = 1024;
        if (parent is MeshInstance3D mesh)
        {
            for (int i = 0; i < mesh.GetSurfaceOverrideMaterialCount(); i++)
                mesh.SetSurfaceOverrideMaterial(i, LoadMat1);
        }
        foreach (var a in parent.GetChildren())
            UpdateActorMat(a);
    }

    public async void UpdateVisuals()
    {
        LevelIcon.Texture = null;
        LevelIcon2.Texture = null;
        
        if (!ActorsExist)
        {
            LoadActors();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        
        GetNode<Label>("LabelLevelName").Text = RehabScene.Root.LoadingChunkName.Replace("_","/");
        string path;
        switch (GetNode<Label>("LabelLevelName").Text)
        {
            case "levels/earth/hub/beach":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub01.res";
            break; case "levels/earth/hub/huba":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level01.res";
            break; case "levels/earth/cavern/cavent":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level02.res";
            break; case "levels/earth/docamok/docamok1":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level03.res";
            break; case "levels/ice/hub/labext":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub02.res";
            break; case "levels/ice/iceclimb/caveent":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level04.res";
            break; case "levels/ice/slipslide/l05start":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level05.res";
            break; case "levels/ice/highseas/gpa01":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level06.res";
            break; case "levels/school/sch/hub/sch/hub":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub03.res";
            break; case "levels/school/boiler/boiler/1":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level08.res";
            break; case "levels/school/crash/crashent":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level09.res";
            break; case "levels/school/rooftop/roof01":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level10.res";
            break; case "levels/altearth/lab/labext":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub04.res";
            break; case "levels/altearth/rockslid/l10start":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level11.res";
            break; case "levels/altearth/hub/altdoc":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level12.res";
            break; case "levels/altearth/core/corea":
                path = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level13.res";
            break; default:
                if (ActorsExist)
                    GetNode<Label>("LabelLevelName").Text = "";
                return;
        }
        if (path != "" && ResourceLoader.Exists(path))
        {
            LevelIcon.Texture = (Texture2D)ResourceLoader.Load(path);
            LevelIcon2.Texture = LevelIcon.Texture;
            GetNode<Label>("LabelLevelName").Text = "";
        }
        LevelIcon.Texture = null;
        LevelIcon2.Texture = null;
        GetNode<Label>("LabelLevelName").Text = "";
    }

    public async void AnimIn()
    {
        UpdateVisuals();
        UpdateViewport();
        LevelIcon.isAnim = false;
        LevelIcon.Scale = Vector2.One;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("LoadingStart");
        Modulate = new Color(1f, 1f, 1f, 1f);
        LevelIcon.PivotOffset = new Vector2(LevelIcon.Size.X / 2, LevelIcon.PivotOffset.Y);
        GetNode<Control>("LabelLevelName").PivotOffset = new Vector2(GetNode<Control>("LabelLevelName").Size.X / 2, LevelIcon.PivotOffset.Y);
        GetNode<Control>("LoadingBG").PivotOffset = new Vector2(GetNode<Control>("LoadingBG").Size.X / 2, LevelIcon.PivotOffset.Y);
        GetNode<Control>("LoadingBG/ColorRectCenter").PivotOffset = new Vector2(GetNode<Control>("LoadingBG/ColorRectCenter").Size.X / 2, LevelIcon.PivotOffset.Y);
        foreach (var i in GetNode("LoadingBG/ColorRectCenter").GetChildren())
        {
            var c = (Control)i;
            c.PivotOffset = new Vector2(c.Size.X / 2, c.PivotOffset.Y);
        }
        float origY = GetNode<Control>("Control").Position.Y;
        GetNode<Control>("Control").Position = new Vector2(GetNode<Control>("Control").Position.X, GetNode<Control>("Control").Position.Y + GetNode<Control>("Control").Size.Y);
        var aTween = CreateTween();
        aTween.TweenProperty(GetNode("Control"), "position:y", origY, 0.5f);
        GetNode<Control>("ViewHolder").Modulate = new Color(1f, 1f, 1f, 0f);
        var aTween1 = CreateTween();
        aTween1.TweenProperty(GetNode("ViewHolder"), "modulate:a", 1f, 0.25f);
        Visible = true;
        if (ActorsExist)
        {
            var randPos = System.Random.Shared.Next(0, 2);
            if (randPos == 0)
                Root3D.RotationDegrees = new Vector3(0, 90f, 0);
            else
                Root3D.RotationDegrees = new Vector3(0, -90f, 0);
            var randAnim = System.Random.Shared.Next(1, 2);
            ActorNode.Visible = true;
            Root3D.Visible = true;
            Root3D.ProcessMode = ProcessModeEnum.Inherit;
            ActorNode.GetNode<AnimationPlayer>("AnimationPlayer").Play("RESET");
            ActorNode.GetNode<AnimationPlayer>("AnimationPlayer").Queue($"scene/loading_{randAnim}");
        }
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("TextAnim");
        LevelIcon.isAnim = true;
    }

    public async void AnimOut()
    {
        Modulate = new Color(1f, 1f, 1f, 1f);
        var mTween = CreateTween();
        mTween.TweenProperty(this, "modulate:a", 0f, 0.5f);
        await ToSignal(GetTree().CreateTimer(0.49f), SceneTreeTimer.SignalName.Timeout);
        Root3D.Visible = false;
        Root3D.ProcessMode = ProcessModeEnum.Disabled;
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