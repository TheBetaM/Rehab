using System;
using Godot;
using RehabSetup;
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
        Visible = true;
        ProcessMode = ProcessModeEnum.Inherit;
        OptionsHolder.Visible = true;
        Exporter = new();
        Exporter.WorkerFinished += ExportDone;
        Exporter.WorkerProgressChanged += UpdateProgress;
        Button1.Text = "#FE-Installer-Browse";
        Button2.Text = "#FE-QuitGame";
        Button1.GrabFocus();
        ProcessActive = false;
        TextStep = 3;
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
                Dialog.UseNativeDialog = true;
                if (Input.IsActionPressed("pad1_R1"))
                {
                    Dialog.UseNativeDialog = false;
                }
                Dialog.Visible = true;
                Dialog.GrabFocus();
                break;
            case ProcessStep.Confirm:
                MainLabel.Text = TranslationServer.Translate("#FE-Installer-Installing");
                Exporter.StartWorker(FilePath);
                ProcessActive = true;
                break;
            case ProcessStep.End:
                Deactivate();
                RehabScene.Root.GameInit();
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
        Exporter = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void UpdateProgress(object s, int e)
    {
        ProgBar.Value = e;
    }
}