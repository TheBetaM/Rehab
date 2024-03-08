using Godot;
using System.Collections.Generic;
namespace Rehab;
public partial class LevelSelectList : Control
{
    [Export]
    Theme LabelTheme;
    [Export]
    Material LabelMaterial;
    List<Button> Labels = new();
    List<string> Paths = new();
    int SelectedItem;
    int ItemCount;
    float Cooldown;
    string ListPath;

    public void InitIcons()
    {
        SetTexture(GetNode<Button>("SimpleList/Control/Button2"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub01.res", "#FE-LevelName-001");
        SetTexture(GetNode<Button>("SimpleList/Control/Button3"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level01.res", "#FE-LevelName-002");
        SetTexture(GetNode<Button>("SimpleList/Control/Button4"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level02.res", "#FE-LevelName-003");
        SetTexture(GetNode<Button>("SimpleList/Control/Button5"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level03.res", "#FE-LevelName-004");
        SetTexture(GetNode<Button>("SimpleList/Control/Button6"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub02.res", "#FE-LevelName-005");
        SetTexture(GetNode<Button>("SimpleList/Control/Button7"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level04.res", "#FE-LevelName-006");
        SetTexture(GetNode<Button>("SimpleList/Control/Button8"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level05.res", "#FE-LevelName-007");
        SetTexture(GetNode<Button>("SimpleList/Control/Button9"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level06.res", "#FE-LevelName-008");
        SetTexture(GetNode<Button>("SimpleList/Control/Button10"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub03.res", "#FE-LevelName-009");
        SetTexture(GetNode<Button>("SimpleList/Control/Button11"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level08.res", "#FE-LevelName-010");
        SetTexture(GetNode<Button>("SimpleList/Control/Button12"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level09.res", "#FE-LevelName-011");
        SetTexture(GetNode<Button>("SimpleList/Control/Button13"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level10.res", "#FE-LevelName-012");
        SetTexture(GetNode<Button>("SimpleList/Control/Button14"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub04.res", "#FE-LevelName-013");
        SetTexture(GetNode<Button>("SimpleList/Control/Button15"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level11.res", "#FE-LevelName-014");
        SetTexture(GetNode<Button>("SimpleList/Control/Button16"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level12.res", "#FE-LevelName-015");
        SetTexture(GetNode<Button>("SimpleList/Control/Button17"), RehabGame.AssetsPath + "Textures/Language/Titles/English/Level13.res", "#FE-LevelName-016");
        GetNode<ScrollContainer>("AdvList").GetVScrollBar().Scale = new Vector2(5f, 1f);
        GetNode<ScrollContainer>("AdvList").GetVScrollBar().PivotOffset = new Vector2(GetNode<ScrollContainer>("AdvList").GetVScrollBar().Size.X, GetNode<ScrollContainer>("AdvList").GetVScrollBar().PivotOffset.Y);
    }

    public void GenerateLevels()
    {
        ItemCount = 0;
        Labels.Clear();
        Paths.Clear();
        foreach (var i in GetNode<VBoxContainer>("AdvList/VBoxContainer").GetChildren())
        {
            i.QueueFree();
        }
        CreateItem("", ItemCount);
        ItemCount = ItemCount + 1;
        ListPath = RehabGame.AssetsPath + "Levels/";
        GenerateList();
    }

    void GenerateList()
    {
        var dir = DirAccess.Open(ListPath);
        if (dir != null)
        {
            dir.ListDirBegin();
            var file_name = dir.GetNext();
            while (file_name != "")
            {
                if (!dir.CurrentIsDir())
                {
                    CreateItem(file_name, ItemCount);
                    ItemCount = ItemCount + 1;
                    //GD.Print("Found file: " + file_name);
                }
                file_name = dir.GetNext();
            }
            if (ItemCount == 0)
            {
                GetNode<Label>("TitleLabel").Text = "#FE-Explorer-ImportNotFound";
                return;
            }
        }
        else if (OS.HasFeature("editor"))
        {
            // DirAccess editor bug workaround
            string folderPath = ProjectSettings.GlobalizePath(ListPath);
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderPath);
            foreach (var i in di.EnumerateFiles())
            {
                CreateItem(i.Name, ItemCount);
                ItemCount = ItemCount + 1;
            }
        }
        else
        {
            // still not detected on Android (pck bug with DirAccess)
            if (ListPath.Contains("cutscenes"))
            {
                CreateItem("Doc-Amok.tscn", ItemCount);
                ItemCount = ItemCount + 1;
            }
            //GD.Print("[LEVEL SELECT] Cannot open " + ListPath);
            //GetNode<Label>("TitleLabel").Text = "#FE-Explorer-ImportNotFound";
        }
    }

    void CreateItem(string itemname, int id)
    {
        var NodeScane = (PackedScene)ResourceLoader.Load("res://assets/frontend/windows/RehabMenuButton.tscn");
        var NewNode = (Button)NodeScane.Instantiate();
        NewNode.Name = $"LevelSelectItem{id}";
        var AdvList = GetNode<ScrollContainer>("AdvList");
        if (id == 0)
        {
            NewNode.Text = "#FE-Back";
            NewNode.Pressed += Adv_ToSimple;
        }
        else
        {
            NewNode.Text = itemname.Replace("_","/").Replace(".tscn", "");
            NewNode.Pressed += () => StartLevel(itemname);
            NewNode.FocusEntered += () => AdvList.EnsureControlVisible(NewNode);
        }
        GetNode<VBoxContainer>("AdvList/VBoxContainer").AddChild(NewNode);
        Labels.Add(NewNode);
        Paths.Add(itemname);
    }

    public void StartLevel(string path)
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
        RehabScene.Root.LoadScene(ListPath + path);
    }

    public override void _Process(double delta)
    {
        if (Cooldown > 0f)
        {
            Cooldown = Cooldown - (float)delta;
            return;
        }
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Visible = false;
            ProcessMode = ProcessModeEnum.Disabled;
            RehabScene.Root.StartMainMenu();
        }
    }

    public void _on_VideoPlayer_finished()
    {
        GetNode<Control>("VideoPlayer").Visible = false;
    }

    public async void Activate()
    {
        var SimpleList = GetNode<ScrollContainer>("SimpleList");
        GetNode<Control>("AdvList").Visible = false;
        SimpleList.Visible = false;
        Hover_Clear();
        Cooldown = 2.1f;
        GetNode<Control>("ColorRectBG").Scale = new Vector2(1f, 0f);
        var bgTween = CreateTween();
        bgTween.TweenProperty(GetNode<Control>("ColorRectBG"),"scale:y", 1f, 0.5f).SetTrans(Tween.TransitionType.Circ);
        GetNode<Control>("ColorRectUpper").Position = new Vector2(GetNode<Control>("ColorRectUpper").Position.X, -170f);
        var rectUpperTween = CreateTween();
        rectUpperTween.TweenProperty(GetNode<Control>("ColorRectUpper"), "position:y", 0f, 0.5f).SetTrans(Tween.TransitionType.Circ).SetDelay(0.5f);
        GetNode<Control>("TitleLabel").Position = new Vector2(GetNode<Control>("TitleLabel").Position.X, -140f);
        var rectUpperTextTween = CreateTween();
        rectUpperTextTween.TweenProperty(GetNode<Control>("TitleLabel"), "position:y", 0f, 0.5f).SetTrans(Tween.TransitionType.Circ).SetDelay(0.5f);
        var rootHeight = ((Control)GetParent()).Size.Y;
        GetNode<Control>("TitleLabel2").Position = new Vector2(GetNode<Control>("TitleLabel2").Position.X, rootHeight + 5f); //725.0 / 575.0
        var rectLowerTextTween = CreateTween();
        rectLowerTextTween.TweenProperty(GetNode<Control>("TitleLabel2"), "position:y", rootHeight - 145f, 0.5f).SetTrans(Tween.TransitionType.Circ).SetDelay(0.5f);
        GetNode<Control>("ColorRectLower").Position = new Vector2(GetNode<Control>("ColorRectLower").Position.X, rootHeight + 130f); //840.0 / 680.0
        var rectLowerTween = CreateTween();
        rectLowerTween.TweenProperty(GetNode<Control>("ColorRectLower"), "position:y", rootHeight - 40f, 0.5f).SetTrans(Tween.TransitionType.Circ).SetDelay(0.5f);
        //var parentHeight = GetParent().Size.Y;
        var origPos = SimpleList.Position.Y;
        SimpleList.Position = new Vector2(SimpleList.Size.X, origPos + 200f); // -45.0
        RehabScene.Root.PlayMenuSound_Back();
        Visible = true;
        ProcessMode = ProcessModeEnum.Inherit;
        
        var SimpleListTween = CreateTween();
        SimpleListTween.TweenProperty(SimpleList, "position", new Vector2(0f, origPos), 1.5f).SetTrans(Tween.TransitionType.Bounce);
        //SimpleList.ScrollHorizontalCustomStep = 0f;
        SimpleList.ScrollHorizontal = 0;
        
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        SimpleList.Visible = true;
        GetNode<Button>("SimpleList/Control/Button2").GrabFocus();
        RehabScene.Root.PlayMusic(60);
    }

    public void SetTexture(Button button, string path, string backup)
    {
        if (ResourceLoader.Exists(path))
            button.Icon = (Texture2D)ResourceLoader.Load(path);
        else
            button.Text = backup;
    }

    public void Simple_ToAdvanced()
    {
        GenerateLevels();
        GetNode<Control>("AdvList").Visible = true;
	    GetNode<Control>("SimpleList").Visible = false;
	    Labels[0].GrabFocus();
    }

    public void Adv_ToSimple()
    {
        GetNode<Control>("AdvList").Visible = false;
	    GetNode<Control>("SimpleList").Visible = true;
	    GetNode<Control>("SimpleList/Control/Button2").GrabFocus();
    }

    public async void StartLevelPath(string path)
    {
        if (!ResourceLoader.Exists(path)) return;
		RehabScene.Root.LoadScene(path);
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		Visible = false;
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public void Simple_GoHub01() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_hub_beach.tscn");
    public void Simple_GoLevel01() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_hub_huba.tscn");
    public void Simple_GoLevel02() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_cavern_cavent.tscn");
    public void Simple_GoLevel03() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_docamok_docamok1.tscn");
    public void Simple_GoHub02() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_hub_labext.tscn");
    public void Simple_GoLevel04() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_iceclimb_caveent.tscn");
    public void Simple_GoLevel05() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_slipslide_l05start.tscn");
    public void Simple_GoLevel06() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_highseas_gpa01.tscn");
    public void Simple_GoHub03() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_sch_hub_sch_hub.tscn");
    public void Simple_GoLevel07() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_boiler_boiler_1.tscn");
    public void Simple_GoLevel08() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_crash_crashent.tscn");
    public void Simple_GoLevel09() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_rooftop_roof01.tscn");
    public void Simple_GoHub04() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_lab_labext.tscn");
    public void Simple_GoLevel10() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_rockslid_l10start.tscn");
    public void Simple_GoLevel11() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_hub_altdoc.tscn");
    public void Simple_GoLevel12() => StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_core_corea.tscn");

    public void Hover_Clear() => GetNode<Label>("TitleLabel2").Text = "";
    public void Hover_Hub01() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-001";
    public void Hover_Level01() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-002";
    public void Hover_Level02() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-003";
    public void Hover_Level03() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-004";
    public void Hover_Hub02() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-005";
    public void Hover_Level04() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-006";
    public void Hover_Level05() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-007";
    public void Hover_Level06() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-008";
    public void Hover_Hub03() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-009";
    public void Hover_Level07() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-010";
    public void Hover_Level08() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-011";
    public void Hover_Level09() => GetNode<Label>("TitleLabel2").Text ="#FE-LevelName-012";
    public void Hover_Hub04() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-013";
    public void Hover_Level10() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-014";
    public void Hover_Level11() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-015";
    public void Hover_Level12() => GetNode<Label>("TitleLabel2").Text = "#FE-LevelName-016";

    public void Go_Cutscenes()
    {
        GenerateCutscenes();
        GetNode<Control>("AdvList").Visible = true;
	    GetNode<Control>("SimpleList").Visible = false;
	    Labels[0].GrabFocus();
    }

    public void Go_Minigames()
    {
        GenerateMinigames();
        GetNode<Control>("AdvList").Visible = true;
	    GetNode<Control>("SimpleList").Visible = false;
	    Labels[0].GrabFocus();
    }

    public void GenerateCutscenes()
    {
        ItemCount = 0;
        Labels.Clear();
        Paths.Clear();
        foreach (var i in GetNode<VBoxContainer>("AdvList/VBoxContainer").GetChildren())
        {
            i.QueueFree();
        }
        CreateItem("", ItemCount);
        ItemCount++;
        ListPath = "res://assets/scenes/cutscenes/";
        GenerateList();
    }

    public void GenerateMinigames()
    {
        ItemCount = 0;
        Labels.Clear();
        Paths.Clear();
        foreach (var i in GetNode<VBoxContainer>("AdvList/VBoxContainer").GetChildren())
        {
            i.QueueFree();
        }
        CreateItem("", ItemCount);
        ItemCount++;
        ListPath = "res://assets/scenes/minigames/";
        GenerateList();
    }
}