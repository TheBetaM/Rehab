using System;
using Godot;
using RehabSetup;
using System.Collections.Generic;
namespace Rehab;
public partial class FrontendInstaller : Control
{

    public AssetExporter Exporter = null;
    Label TitleLabel;
    Label MainLabel;
    Label BottomLabel;
    ProgressBar ProgBar;
    FileDialog Dialog;
    HBoxContainer OptionsHolder;
    Button Button1;
    Button Button2;
    ProcessStep Step = ProcessStep.Start;
    string FilePath;
    bool ProcessActive = false;
    double TextTimer = 0f;
    int TextStep = 3;
    List<Node> DialogItems = new List<Node>();

    enum ProcessStep
    {
        Start = 0,
        Confirm = 1,
        End = 2,
        Failed = 3,
    }

    public override void _Ready()
    {
        TitleLabel = GetNode<Label>("RehabLabel");
        BottomLabel = GetNode<Label>("RehabLabel2");
        MainLabel = GetNode<Label>("RehabLabel3");
        ProgBar = GetNode("Control").GetNode<ProgressBar>("ProgressBar");
        Dialog = GetNode<FileDialog>("FileDialog");
        OptionsHolder = GetNode<HBoxContainer>("HBoxContainer");
        Button1 = OptionsHolder.GetNode<Button>("Button1");
        Button2 = OptionsHolder.GetNode<Button>("Button2");

        Dialog.FileSelected += Dialog_Select;
        Dialog.Canceled += Dialog_Cancel;
        Button1.Pressed += Button1_Click;
        Button2.Pressed += Button2_Click;
    }

    public override void _Process(double delta)
    {
        if (!ProcessActive) return;
        TextTimer -= delta;
        if (TextTimer <= 0d)
        {
            TextTimer = 0.5d;
            string Add;
            switch (TextStep)
            {
                default:
                case 3:
                    Add = ".";
                break;
                case 2:
                    Add = "..";
                break;
                case 1:
                    Add = "...";
                break;
            }
            MainLabel.Text = $"{TranslationServer.Translate("#FE-Installer-Installing")}{Add}";
            TextStep--;
            if (TextStep <= 0)
            {
                TextStep = 3;
            }
        }
    }

    public void Activate()
    {
        TitleLabel.Text = "#FE-Installer-Header";
        BottomLabel.Text = "";
        MainLabel.Text = "#FE-Installer-StartMessage";
        ProgBar.Value = 0f;
        Step = ProcessStep.Start;
        ProcessMode = ProcessModeEnum.Inherit;
        Exporter = new();
        Exporter.WorkerFinished += ExportDone;
        Exporter.WorkerProgressChanged += UpdateProgress;
        Button1.Text = "#FE-Installer-Browse";
        Button2.Text = "#FE-QuitGame";
        ProcessActive = false;
        TextStep = 3;
        StartAnim();
    }

    async void StartAnim()
    {
        ProgBar.Visible = false;
        MainLabel.Visible = false;
        OptionsHolder.Visible = false;
        GetNode<Control>("ColorRect").Scale = new Vector2(1f, 0f);
        GetNode<Control>("ColorRect2").Scale = new Vector2(1f, 0f);
        GetNode<Control>("ColorRect3").Scale = new Vector2(1f, 0f);
        var tween1 = CreateTween();
        tween1.TweenProperty(GetNode<Control>("ColorRect2"), "scale:y", 1f, 1f).SetTrans(Tween.TransitionType.Circ);
        var tween2 = CreateTween();
        tween2.TweenProperty(GetNode<Control>("ColorRect3"), "scale:y", 1f, 1f).SetTrans(Tween.TransitionType.Circ);
        var tween3 = CreateTween();
        tween3.TweenProperty(GetNode<Control>("ColorRect"), "scale:y", 1f, 0.5f).SetTrans(Tween.TransitionType.Circ);
        Visible = true;
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        ProgBar.Visible = true;
        MainLabel.Visible = true;
        OptionsHolder.Visible = true;
        Button1.GrabFocus();
    }

    public void Deactivate()
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
        Exporter = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void Button1_Click()
    {
        OptionsHolder.Visible = false;
        switch (Step)
        {
            case ProcessStep.Start:
                Dialog.UseNativeDialog = false;
                if (Input.IsActionPressed("pad1_R1"))
                {
                    Dialog.UseNativeDialog = true;
                }
                if (OS.GetName() == "Android")
                {
                    Dialog.CurrentPath = OS.GetSystemDir(OS.SystemDir.Downloads) + "/";
                }
                Dialog.Visible = true;
                Dialog.GrabFocus();
                if (RehabScene.Root.XR_Enabled)
                {
                    Dialog.Visible = false;
                    Dialog.Exclusive = false;
                    foreach (var item in Dialog.GetChildren(true))
                    {
                        item.Reparent(this);
                        DialogItems.Add(item);
                        if (item is Control c)
                        {
                            c.Position += new Vector2(180f, 90f);
                        }
                    }
                }
                break;
            case ProcessStep.Confirm:
                MainLabel.Text = TranslationServer.Translate("#FE-Installer-Installing");
                Exporter.StartWorker(FilePath);
                ProcessActive = true;
                break;
            case ProcessStep.End:
                EndAnim();
                break;
            case ProcessStep.Failed:
                GetTree().Quit();
                break;
        }
    }

    public void Button2_Click()
    {
        GetTree().Quit();
    }

    public void Dialog_Select(string file)
    {
        foreach (var item in DialogItems)
        {
            item.Reparent(Dialog);
        }
        DialogItems.Clear();
        FilePath = file;
        bool isXbox = Exporter.DetectXBE(file);
        bool isPS2 = false;
        if (!isXbox)
        {
            isPS2 = Exporter.DetectPS2(file);
        }
        if (!isXbox && !isPS2)
        {
            MainLabel.Text = "#FE-Installer-DetectFailed";
        }
        else
        {
            string verString = isPS2 ? "PS2" : "XBOX";
            if (isPS2)
            {
                verString = Exporter.isDemo ? "DEMO" : "PS2";
            }
            string regionString = Exporter.isPAL ? "EUR" : "USA";
            if (!Exporter.isPAL)
            {
                regionString = Exporter.isJPN ? "JPN" : "USA";
            }
            MainLabel.Text = $"{TranslationServer.Translate("#FE-Installer-Detected")}: {regionString} {verString}";
            Step = ProcessStep.Confirm;
            Button1.Text = "#FE-Continue";
        }
        OptionsHolder.Visible = true;
        Button1.GrabFocus();
    }

    public void Dialog_Cancel()
    {
        foreach (var item in DialogItems)
        {
            item.Reparent(Dialog);
        }
        DialogItems.Clear();
        OptionsHolder.Visible = true;
        Button1.GrabFocus();
    }

    public void ExportDone(object s, EventArgs e)
    {
        ProcessActive = false;
        MainLabel.Text = $"#FE-Installer-Complete";
        Step = ProcessStep.End;
        Button1.Text = "#FE-Continue";
        OptionsHolder.Visible = true;
        Button1.GrabFocus();
        GetWindow().RequestAttention();
        Input.StartJoyVibration(0, 0.5f, 0f, 0.5f);
        Exporter = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void UpdateProgress(object s, int e)
    {
        ProgBar.Value = e;
    }

    async void EndAnim()
    {
        ProgBar.Visible = false;
        MainLabel.Visible = false;
        OptionsHolder.Visible = false;
        var tween1 = CreateTween();
        tween1.TweenProperty(GetNode<Control>("ColorRect2"), "scale:y", 0f, 1f).SetTrans(Tween.TransitionType.Circ);
        var tween2 = CreateTween();
        tween2.TweenProperty(GetNode<Control>("ColorRect3"), "scale:y", 0f, 1f).SetTrans(Tween.TransitionType.Circ);
        var tween3 = CreateTween();
        tween3.TweenProperty(GetNode<Control>("ColorRect"), "scale:y", 0f, 0.5f).SetTrans(Tween.TransitionType.Circ);
        Visible = true;
        await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        Deactivate();
        RehabScene.Root.GameInit();
    }
}