using System.Collections.Generic;
using Godot;
namespace Rehab;
public partial class AgentFurniture : Agent
{
    List<string> Doors = [
        "Boiler_DoubleDoor_Anim",
        "Door_Cortex_Lab_Big",
        "AltEarth_AntCaves_GiantBlastDoor",
        "AltEarth_Core_BlastDoor_Small",
        "Battleship_Door_B",
        "Battleship_IronDoor_B",
        "Generic_GreyStoneDoor",
        "Global_StreamingDoor",
        "Ice_CortexDoorCockBlocker",
        "School_Classroom_Door",
        "School_OneWayActiveDoor",
        "School_OneWayDoor",
        "Village_Stockade_Gate",
        "Boiler_Glass_Panel",
        "Boiler_Steam_Hazard",
    ];

    public override void _Ready()
    {
        base._Ready();

        DoAnimation(0, true);
	
        // temp to make exploring in explorer easier
        if (Doors.Contains(Name))
        {
            Set("contact_monitor", true);
            Set("max_contacts_reported", 256);
            Set("freeze_mode", (int)RigidBody3D.FreezeModeEnum.Kinematic);
            Set("collision_layer", 0);
            Connect("body_entered", Callable.From<Node3D>(OnDoorTouch));
        }
    }

    public override void OnChunkEnter(object a, System.EventArgs e)
    {
        switch (Name)
        {
            default: break;
            case "DJ":
                RehabScene.Root.PlayMusic(RegInt[0]);
                RehabGame.SetLevelID(RegInt[2]);
            break;
            case "Global_Ambient_Sound":
                RehabScene.Root.PlayAmbience(RegInt[2]);
            break;
            case "Util_JungleHousekeeping":
                RehabScene.Root.PlayMusic(27);
                RehabGame.SetLevelID(0);
            break;
            case "Util_CavernHousekeeping":
                RehabScene.Root.PlayMusic(28);
                RehabGame.SetLevelID(3);
            break;
            case "Util_IceHousekeeping":
                RehabScene.Root.PlayMusic(1);
                RehabGame.SetLevelID(6);
            break;
            case "Util_TotemHousekeeping":
                // todo: only in demo chunks
                RehabScene.Root.PlayMusic(29);
            break;
        }
    }

    public void OnDoorTouch(Node3D body)
    {
        if (body is AgentCharacter)
        {
            if (AgentCharacter.activeCharacter == body)
            {
                //CollisionLayer = 0;
			    Visible = false;
			    CallDeferred("DisableDoor");
            }
        }	    
    }

    public void DisableDoor()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

}