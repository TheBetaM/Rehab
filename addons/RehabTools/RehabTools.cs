#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rehab.Tools;

[Tool]
public partial class RehabTools : EditorPlugin
{

    PackedScene LoadScene;
    RehabToolsMenu EditorLoad;

    public RehabTools()
    {
        Name = "RehabTools";
        LoadScene = ResourceLoader.Load<PackedScene>("res://addons/RehabTools/Editor/ToolsMenu.tscn");
    }

    public override void _EnablePlugin()
    {
        
    }

    public override void _DisablePlugin()
    {

    }

    public override void _EnterTree()
    {
        EditorLoad = (RehabToolsMenu)LoadScene.Instantiate();
        EditorLoad.plugin = this;
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(EditorLoad);
        //AddControlToDock(DockSlot.LeftUl, EditorLoad);
        _MakeVisible(false);
        //AddNode3DGizmoPlugin(GizmoPlugin);
    }

    public override void _ExitTree()
    {
        if (EditorLoad != null)
        {
            //RemoveControlFromDocks(EditorLoad);
            EditorLoad.QueueFree();
        }
        //RemoveNode3DGizmoPlugin(GizmoPlugin);
    }

    public override bool _HasMainScreen()
    {
        return true;
    }

    public override string _GetPluginName()
    {
        return "RehabTools";
    }

    public override void _MakeVisible(bool visible)
    {
        if (EditorLoad != null)
        {
            EditorLoad.Visible = visible;
        }
    }

    public override void _SaveExternalData()
    {

    }

    public override Texture2D _GetPluginIcon()
    {
        return EditorInterface.Singleton.GetBaseControl().GetThemeIcon("Node", "EditorIcons");
    }


}

#endif
