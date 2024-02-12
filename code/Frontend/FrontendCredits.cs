using Godot;
namespace Rehab;
public partial class FrontendCredits : Control
{
    TextureRect ImageRect;
    PackedScene LabelScene;
    string ImagePath = RehabGame.AssetsPath + "Textures/Language/Credits/CreditNew.res";
    string CreditsText;
    bool CreditsActive;
    int LineCount = 445;
    bool FirstLoad;

    public override void _Ready()
    {
        ImageRect = GetNode<TextureRect>("ImageRect");
        LabelScene = (PackedScene)ResourceLoader.Load("res://assets/frontend/RehabLabel.tscn");
    }

    public void LoadFirst()
    {
        FirstLoad = true;
        if (ResourceLoader.Exists(ImagePath))
            ImageRect.Texture = (Texture2D)ResourceLoader.Load(ImagePath);
        CreditsActive = false;
        GetNode<Control>("VBox").Position = new Vector2(0f, 720.0f);
    }

    public override void _Process(double delta)
    {
        if (!CreditsActive) return;
        if (Input.IsActionJustPressed("ui_select"))
        {
            CreditsActive = false;
            EndCredits();
            return;
        }
        var VBox = GetNode<Control>("VBox");
        VBox.Position += Vector2.Up * (float)delta * 64.0f;
        if (VBox.Position.Y < -VBox.Size.Y)
        {
            CreditsActive = false;
            EndCredits();
        }
    }

    public async void StartCredits()
    {
        if (!FirstLoad)
        {
            LoadFirst();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        var VBox = GetNode<Control>("VBox");
        //var file = FileAccess.open("res://assets/lang/credits.txt", FileAccess.READ)
        TextResource file = (TextResource)ResourceLoader.Load("res://assets/lang/credits.tres");
        var fileLines = file.text.Split("\n");
        //file.get_line()
        CreditsActive = false;
        //modulate.a = 1.0
        VBox.Position = new Vector2(0f, 720.0f);
        VBox.Visible = true;
        ImageRect.Modulate = new Color(1f, 1f, 1f, 0f);
        ProcessMode = ProcessModeEnum.Always;
        var mTween = CreateTween();
        mTween.TweenProperty(ImageRect, "modulate:a", 1.0f, 0.5f);
        VBox.Position = new Vector2(0f, 720.0f);
        Visible = true;
        foreach (var i in VBox.GetChildren())
            i.QueueFree();
        VBox.Size = new Vector2(VBox.Size.X, 40 * LineCount);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        
        for (int i = 0; i < LineCount; i++){
            var label = (Label)LabelScene.Instantiate();
            //label.text = file.get_line()
            label.Text = fileLines[i];
            VBox.AddChild(label);
        }
        
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        VBox.Position = new Vector2(0f, 720.0f);
        StartMusic();
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        VBox.Visible = true;
        VBox.Position = new Vector2(0f, 720.0f);
        CreditsActive = true;
    }

    public async void StartMusic()
    {
        RehabScene.AudioMusic.GetParent().GetNode("AudioMusic1").ProcessMode = ProcessModeEnum.Always;
        RehabScene.AudioMusic.GetParent().GetNode("AudioMusic2").ProcessMode = ProcessModeEnum.Always;
        RehabScene.Root.PlayMusic(58);
        await ToSignal(GetTree().CreateTimer(20.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(28);
        await ToSignal(GetTree().CreateTimer(20.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(136);
        await ToSignal(GetTree().CreateTimer(18.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(30);
        await ToSignal(GetTree().CreateTimer(20.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(35);
        await ToSignal(GetTree().CreateTimer(18.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(37);
        await ToSignal(GetTree().CreateTimer(20.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(41);
        await ToSignal(GetTree().CreateTimer(20.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(54);
        await ToSignal(GetTree().CreateTimer(18.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(60);
        await ToSignal(GetTree().CreateTimer(18.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(61);
        await ToSignal(GetTree().CreateTimer(20.0f), SceneTreeTimer.SignalName.Timeout);
        if (!CreditsActive) return;
        RehabScene.Root.PlayMusic(27);
    }

    public void EndCredits()
    {
        //var mTween = CreateTween();
        //mTween.TweenProperty(this, "modulate:a", 0.0f, 0.5f);
        //await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        
        CreditsActive = false;
        GetNode<Control>("VBox").Visible = false;
        foreach (var i in GetNode<Control>("VBox").GetChildren())
            i.QueueFree();
        ProcessMode = ProcessModeEnum.Disabled;
        Visible = false;
        RehabScene.AudioMusic.GetParent().GetNode("AudioMusic1").ProcessMode = ProcessModeEnum.Inherit;
        RehabScene.AudioMusic.GetParent().GetNode("AudioMusic2").ProcessMode = ProcessModeEnum.Inherit;
        RehabScene.Root.ProcessMode = ProcessModeEnum.Inherit;
        RehabScene.AudioMusic.ProcessMode = ProcessModeEnum.Inherit;
    }
}