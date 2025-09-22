#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Rehab.Tools;

[Tool]
public partial class RehabToolsMenu : Control
{

    public RehabTools plugin;
    FileDialog OpenDialog;
    Button MountButton;

    public override void _Ready()
    {
        //OpenDialog = (FileDialog)GetChild(1);
        //MountButton = (Button)FindChild("CheckBox_MountData");
        //MountButton.Toggled += MountButtonToggled;
        GetWindow().FilesDropped += RehabToolsMenu_FilesDropped;

        if (OS.GetName() == "Android")
        {
            RehabGame.DataPath = OS.GetUserDataDir() + "/";
            RehabGame.PacksPath = OS.GetSystemDir(OS.SystemDir.Downloads) +"/Rehab/";
            if (!DirAccess.DirExistsAbsolute(RehabGame.PacksPath))
                DirAccess.MakeDirAbsolute(RehabGame.PacksPath);
        }
        else
        {
            var PathSplit = RehabGame.DataPath.Split("/");
            var PacksAddPath = "";
            var PathID = 0;
            foreach (var i in PathSplit)
            {
                PathID++;
                if (PathID < PathSplit.Length)
                {
                    PacksAddPath += i + "/";
                }
            }
            PacksAddPath += "Packs/";
            RehabGame.DataPath = PacksAddPath;
            if (!DirAccess.DirExistsAbsolute(PacksAddPath))
                DirAccess.MakeDirAbsolute(PacksAddPath);
            RehabGame.PacksPath = PacksAddPath;
        }
    }

    private void BrowseButton_ButtonDown()
    {
        OpenDialog.PopupCenteredRatio(0.5f);
        OpenDialog.Visible = true;
        OpenDialog.FileSelected += Dialog_FileSelected;
    }

    private void Dialog_FileSelected(string path)
    {
        OpenDialog.FileSelected -= Dialog_FileSelected;
    }

    private void RehabToolsMenu_FilesDropped(string[] files)
    {

    }

    private void MountButtonToggled(bool state)
    {
        // Doesn't work
        /*
        if (state)
        {
            string PacksPath = RehabGame.DataPath;
            var pdir = DirAccess.Open(PacksPath);
            if (pdir != null)
            {
                foreach (var i in pdir.GetFiles())
                {
                    var success = ProjectSettings.LoadResourcePack(PacksPath + i);
                    if (success)
                        GD.Print("[ROOT] Pack loaded from " + i);
                    else
                        GD.PrintErr("[ROOT] Pack FAILED from " + i);
                }
            }
            else
            {
                GD.Print("[ROOT] Data directory failed to open! " + PacksPath);
            }
        }
        */
    }

}

#endif