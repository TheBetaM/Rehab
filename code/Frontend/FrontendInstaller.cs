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

    public void Activate()
    {
        TitleLabel.Text = "Installer";
        BottomLabel.Text = "";
        MainLabel.Text = "The game data must be installed to play.";
        ProgBar.Value = 0f;
        Step = ProcessStep.Start;
        Visible = true;
        ProcessMode = ProcessModeEnum.Inherit;
        OptionsHolder.Visible = true;
        Exporter = new();
        Exporter.WorkerFinished += ExportDone;
        Exporter.WorkerProgressChanged += UpdateProgress;
        Button1.Text = "BROWSE";
        Button2.Text = "QUIT";
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
                Dialog.Visible = true;
                Dialog.GrabFocus();
                break;
            case ProcessStep.Confirm:
                MainLabel.Text = "Installing...";
                Exporter.StartWorker(FilePath);
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
            MainLabel.Text = "Failed to detect the game.";
        }
        else
        {
            string verString = isPS2 ? "PS2" : "XBOX";
            string regionString = Exporter.isPAL ? "EUR" : "USA";
            MainLabel.Text = $"Detected the {regionString} {verString} version.\nBegin install?";
            Step = ProcessStep.Confirm;
            Button1.Text = "START";
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
        MainLabel.Text = $"Install complete!";
        Step = ProcessStep.End;
        Button1.Text = "START GAME";
        OptionsHolder.Visible = true;
        Button1.GrabFocus();
        GetWindow().RequestAttention();
    }

    public void UpdateProgress(object s, int e)
    {
        ProgBar.Value = e;
    }
}