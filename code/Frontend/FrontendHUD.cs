using Godot;
namespace Rehab;
public partial class FrontendHUD : Control
{
    Label WumpaLabel;
    Label LivesLabel;
    Label BottomTextLabel;
    Label TimerLabel;
    Label CounterLabel;
    Control WumpaHolder;
    Control LivesHolder;
    TextureRect WumpaIcon;
    TextureRect LivesIcon;
    TextureRect GemIcon;
    TextureRect CrystalIcon;

    float WumpaTimer;
    float LivesTimer;
    string WumpaIconPath = RehabGame.AssetsPath + "Textures/Icons/wumpa_icon.res";
    string[] LivesIconPaths = [RehabGame.AssetsPath + "Textures/Icons/1up-crash.res", RehabGame.AssetsPath + "Textures/Icons/1up-cortex.res",
    RehabGame.AssetsPath + "Textures/Icons/1up-coco.res", RehabGame.AssetsPath + "Textures/Icons/1up-nina.res", 
    RehabGame.AssetsPath + "Textures/Icons/1up-evilcrash.res", RehabGame.AssetsPath + "Textures/Icons/1up-mechabandicoot.res"];
    Tween WumpaHolderAnim;
    Tween LivesHolderAnim;
    string CrystalIconPath = RehabGame.AssetsPath + "Textures/Icons/Crystal_Single.res";
    string[] GemIconPaths = [
        RehabGame.AssetsPath + "Textures/Icons/gem-blue.res",
        RehabGame.AssetsPath + "Textures/Icons/gem-clear.res",
        RehabGame.AssetsPath + "Textures/Icons/gem-green.res",
        RehabGame.AssetsPath + "Textures/Icons/gem-purple.res",
        RehabGame.AssetsPath + "Textures/Icons/gem-red.res",
        RehabGame.AssetsPath + "Textures/Icons/gem-yellow.res",
    ];

    public override void _Ready()
    {
        WumpaLabel = GetNode<Label>("Wumpa/CountWumpa");
        LivesLabel = GetNode<Label>("Lives/CountLives");
        BottomTextLabel = GetNode<Label>("LabelBottom");
        TimerLabel = GetNode<Label>("LabelTimer");
        CounterLabel = GetNode<Label>("LabelCounter");
        WumpaHolder = GetNode<Control>("Wumpa");
        LivesHolder = GetNode<Control>("Lives");
        WumpaIcon = GetNode<TextureRect>("Wumpa/IconWumpa");
        LivesIcon = GetNode<TextureRect>("Lives/IconLives");
        GemIcon = GetNode<TextureRect>("Gem/IconGem");
        CrystalIcon = GetNode<TextureRect>("Crystal/IconCrystal");
    }

    public void Setup()
    {
        if (ResourceLoader.Exists(WumpaIconPath))
            WumpaIcon.Texture = (Texture2D)ResourceLoader.Load(WumpaIconPath);
        else
            WumpaIcon.Visible = false;
        if (ResourceLoader.Exists(LivesIconPaths[0]))
            LivesIcon.Texture = (Texture2D)ResourceLoader.Load(LivesIconPaths[0]);
        else
            LivesIcon.Visible = false;
        if (ResourceLoader.Exists(CrystalIconPath))
            CrystalIcon.Texture = (Texture2D)ResourceLoader.Load(CrystalIconPath);
        if (ResourceLoader.Exists(GemIconPaths[0]))
            GemIcon.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[0]);
    }

    public override void _Process(double delta)
    {
        if (WumpaTimer > 0f)
        {
            WumpaTimer -= (float)delta;
            if (WumpaTimer <= 0f)
            {
                if (WumpaHolderAnim != null)
                    WumpaHolderAnim.Kill();
                WumpaHolderAnim = CreateTween();
                WumpaHolderAnim.TweenProperty(WumpaHolder, "position:x", -300f, 0.25f);
                WumpaHolderAnim.TweenCallback(Callable.From(() => WumpaHolder.Visible = false));
            }
        }
        
        if (LivesTimer > 0f)
        {
            LivesTimer -= (float)delta;
            if (LivesTimer <= 0f)
            {
                if (LivesHolderAnim != null)
                    LivesHolderAnim.Kill();
                LivesHolderAnim = CreateTween();
                LivesHolderAnim.TweenProperty(LivesHolder,"position:x", Size.X - 20f, 0.25f);
                LivesHolderAnim.TweenCallback(Callable.From(() => LivesHolder.Visible = false));
            }
        }
    }

    public void UpdateWumpa()
    {
        WumpaLabel.Text = RehabGame.Fruit.ToString();
        if (WumpaHolder.Visible)
        {
            WumpaTimer = 5f;
            return;
        }
        WumpaHolder.Position = new Vector2(-300f, WumpaHolder.Position.Y);
        WumpaHolder.Visible = true;
        WumpaTimer = 5f;
        if (WumpaHolderAnim != null)
            WumpaHolderAnim.Kill();
        WumpaHolderAnim = CreateTween();
        WumpaHolderAnim.TweenProperty(WumpaHolder,"position:x", 0f, 0.25f);
    }

    public void ForceAnimOut()
    {
        WumpaTimer = 0.01f;
	    LivesTimer = 0.01f;
    }

    public void AnimateWumpa()
    {
        var iconAnim = CreateTween();
	    iconAnim.TweenProperty(WumpaIcon, "scale", new Vector2(0.9f, 1.2f), 0.05f);
	    iconAnim.TweenProperty(WumpaIcon, "scale", new Vector2(0.75f, 1.0f), 0.05f).SetDelay(0.05f);
    }

    public void UpdateLives()
    {
        UpdateXR();
        LivesLabel.Text = RehabGame.Lives.ToString();
        if (LivesHolder.Visible)
        {
            LivesTimer = 5f;
            return;
        }
        if (!LivesHolder.Visible)
        {
            int iconID = 0;
            if (AgentCharacter.activeCharacter != null)
                iconID = (int)AgentCharacter.activeCharacter.CharType;
            if (ResourceLoader.Exists(LivesIconPaths[iconID]))
                LivesIcon.Texture = (Texture2D)ResourceLoader.Load(LivesIconPaths[iconID]);
        }
        LivesHolder.Position = new Vector2(Size.X - 20.0f, LivesHolder.Position.Y);
        LivesHolder.Visible = true;
        LivesTimer = 5f;
        if (LivesHolderAnim != null)
            LivesHolderAnim.Kill();
        LivesHolderAnim = CreateTween();
        LivesHolderAnim.TweenProperty(LivesHolder,"position:x", Size.X - 400.0f, 0.25f);
    }

    public void AnimateLife()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("LifeIconANim");
    }

    public async void AnimateGem(int gem)
    {
        UpdateXR();
        if (ResourceLoader.Exists(GemIconPaths[gem]))
            GemIcon.Texture = (Texture2D)ResourceLoader.Load(GemIconPaths[gem]);
        
        GemIcon.Scale = Vector2.Zero;
        GemIcon.Modulate = new Color(1f, 1f, 1f, 0f);
        var holder = (Control)GemIcon.GetParent();
        holder.Visible = true;
        var gemTween1 = CreateTween();
        var gemTween2 = CreateTween();
        gemTween1.TweenProperty(GemIcon, "scale", new Vector2(1.0f, 1.0f), 0.5f);
        gemTween2.TweenProperty(GemIcon, "modulate:a", 1.0f, 0.5f);
        await ToSignal(GetTree().CreateTimer(3f), SceneTreeTimer.SignalName.Timeout);
        gemTween1 = CreateTween();
        gemTween2 = CreateTween();
        gemTween1.TweenProperty(GemIcon, "scale", new Vector2(0.0f, 0.0f), 0.5f);
        gemTween2.TweenProperty(GemIcon, "modulate:a", 0.0f, 0.5f);
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        holder.Visible = false;
    }

    public async void AnimateCrystal()
    {
        UpdateXR();
        CrystalIcon.Scale = Vector2.Zero;
        CrystalIcon.Modulate = new Color(1f, 1f, 1f, 0f);
        var holder = (Control)CrystalIcon.GetParent();
        holder.Visible = true;
        var gemTween1 = CreateTween();
        var gemTween2 = CreateTween();
        gemTween1.TweenProperty(CrystalIcon, "scale", new Vector2(1.0f, 1.0f), 0.5f);
        gemTween2.TweenProperty(CrystalIcon, "modulate:a", 1.0f, 0.5f);
        await ToSignal(GetTree().CreateTimer(3f), SceneTreeTimer.SignalName.Timeout);
        gemTween1 = CreateTween();
        gemTween2 = CreateTween();
        gemTween1.TweenProperty(CrystalIcon, "scale", new Vector2(0.0f, 0.0f), 0.5f);
        gemTween2.TweenProperty(CrystalIcon, "modulate:a", 0.0f, 0.5f);
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        holder.Visible = false;
    }

    public void UpdateAll()
    {
        WumpaLabel.Text = RehabGame.Fruit.ToString();
	    LivesLabel.Text = RehabGame.Lives.ToString();
    }

    public async void FlashMessage(string text, float fadeTime = 0.5f)
    {
        UpdateXR();
        BottomTextLabel.Visible = false;
        BottomTextLabel.Text = text;
        BottomTextLabel.Modulate = new Color(1f, 1f, 1f, 0f);
        var tTween1 = CreateTween();
        tTween1.TweenProperty(BottomTextLabel, "modulate:a", 1.0f, fadeTime);
        BottomTextLabel.Visible = true;
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        var tTween2 = CreateTween();
        tTween2.TweenProperty(BottomTextLabel, "modulate:a", 0f, fadeTime);
        await ToSignal(GetTree().CreateTimer(fadeTime), SceneTreeTimer.SignalName.Timeout);
        BottomTextLabel.Visible = false;
    }

    public void Clear()
    {
        TimerLabel.Visible = false;
        CounterLabel.Visible = false;
        BottomTextLabel.Visible = false;
        WumpaHolder.Visible = false;
        LivesHolder.Visible = false;
        LivesTimer = 0f;
        WumpaTimer = 0f;
        UpdateAll();
    }

    public void OnPause()
    {
        ForceAnimOut();
        GetNode<Control>("TouchControls").Visible = false;
    }

    public void OnUnPause()
    {
        GetNode<Control>("TouchControls").Visible = true;
    }

    void UpdateXR()
    {
        if (RehabScene.Root.XR_Enabled)
            RehabScene.Root.XR_Origin.ResetOrientation();
    }

    public void DisplayMessage(string text, float fadeTime = 0.5f)
    {
        UpdateXR();
        BottomTextLabel.Visible = false;
        BottomTextLabel.Text = text;
        BottomTextLabel.Modulate = new Color(1f, 1f, 1f, 0f);
        var tTween1 = CreateTween();
        tTween1.TweenProperty(BottomTextLabel, "modulate:a", 1.0f, fadeTime);
        BottomTextLabel.Visible = true;
    }

    public async void ClearMessage(float fadeTime = 0.5f)
    {
        var tTween2 = CreateTween();
        tTween2.TweenProperty(BottomTextLabel, "modulate:a", 0f, fadeTime);
        await ToSignal(GetTree().CreateTimer(fadeTime), SceneTreeTimer.SignalName.Timeout);
        BottomTextLabel.Visible = false;
    }

    public void SetMessage(string text)
    {
        BottomTextLabel.Visible = true;
        BottomTextLabel.Text = text;
        BottomTextLabel.Modulate = new Color(1f, 1f, 1f, 1f);
    }
}