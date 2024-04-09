using System.Collections.Generic;
using Godot;
namespace Rehab;
public partial class AgentFurniture : Agent
{
    List<string> Doors = [
        "DoubleDoor_Anim",
        "Door_CortexLab_Big",
        "GiantBlastDoor",
        "BlastDoor_Small",
        "Battleship_Door_B",
        "Battleship_IronDoor_B",
        "Generic_GreyStoneDoor",
        "Global_StreamingDoor",
        "Ice_CortexDoorBlocker",
        "Classroom_Door",
        "OneWayActiveDoor",
        "OneWayDoor",
        "Village_StockadeGate",
        "GlassPanel",
        "Steam_Hazard",
    ];
    bool IsDoor;

    public override void _Ready()
    {
        base._Ready();

        DoAnimation(0, true);
	
        // temp to make exploring in explorer easier
        if (Doors.Contains(Name))
        {
            IsDoor = true;
            Connect("body_entered", Callable.From<Node3D>(OnSolidCollisionEnter));
        }
        switch (Name)
        {
            default: break;
            case "GargoyleSmall":
                Set("collision_layer", 0);
                Set("collision_mask", 0);
                DoAnimation(4, true);
                var hook = (AgentChiChiGrass)SubActorsScenes[0].Instantiate();
                hook.Visible = false;
                hook.Name = "ChiChiGrass_Ceiling";
                hook.LinkPoint = LinkPoint;
                AddChild(hook);
            break;
            case "IceClimb_AmbientSounds":
                switch (RegInt[0])
                {
                    case 1: DoSound(4, 1f, 0f, true); break;
                    case 2: DoSound(5, 1f, 0f, true); break;
                    case 4: DoSound(10, 1f, 0f, true); break;
                    case 3: break; // COM_ICECLIMB_AMBIENT_SOUNDS_WATERSPLASHES
                }
            break;
            case "School_Hub_Ambient_Sounds":
            case "Sound_Ambient_SlipSlide":
                DoSound(0, 1f, 0f, true);
            break;
            case "SoundSpot_CamShaft":
            case "SoundSpot_ElectricFence":
            case "SoundSpot_Engine_Wheel":
            case "SoundSpot_Fan_Spin":
            case "SoundSpot_Lava":
            case "SoundSpot_Piston":
            case "SoundSpot_Pistons":
            case "SoundSpot_Engine_Piston":
            case "SoundSpot_Plank_Spin":
            case "SoundSpot_Psychetron":
            case "SoundSpot_Psychetron_Beams":
            case "SoundSpot_PurplePipes":
            case "SoundSpot_PurplePits":
            case "SoundSpot_River_Below":
            case "SoundSpot_Waterfall":
            case "Sound_Spot_Engine_Piston":
                DelayedSoundStart();
            break;
            case "Sound_Ambient_IceHub":
                switch (RegInt[0])
                {
                    case 1: DoSound(2, 1f, 0f, true); break;
                    case 2: DoSound(3, 1f, 0f, true); break;
                    case 3: DoSound(4, 1f, 0f, true); break;
                    case 4: DoSound(1, 1f, 0f, true); break;
                    case 5: DoSound(5, 1f, 0f, true); break;
                    case 6: DoSound(6, 1f, 0f, true); break;
                    default: DoSound(0, 1f, 0f, true); break;
                }
            break;
        }

        if (ParentScene != null)
        {
            ParentScene.OnChunkEnter += OnChunkEnter;
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
        if (!IsDoor) return;
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

    public override void OnNonSolidCollisionEnter(Node3D body)
    {
        OnDoorTouch(body);
    }

    public void OnSolidCollisionEnter(Node3D body)
    {
        OnDoorTouch(body);
    }

    public override void OnMessage(int id)
    {
        switch (Name)
        {
            default: break;
            case "DJ_Triggerable":
                if (id == 87 || id == 138) RehabScene.Root.PlayMusic(RegInt[0]);
            break;
            case "Boiler_TriggeredSound":
                if (id == 4)
                {
                    switch (RegInt[0])
                    {
                        case 1: DoSound(0, 1f, 0f); break;
                        case 2: DoSound(1, 1f, 0f); break;
                        case 3: DoSound(2, 1f, 0f); break;
                        case 4: DoSound(3, 1f, 0f); break;
                    }
                }
            break;
        }
    }

    async void DelayedSoundStart()
    {
        await ToSignal(GetTree().CreateTimer(RegFloat[4]), SceneTreeTimer.SignalName.Timeout);
        switch (Name)
        {
            default: break;
            case "SoundSpot_CamShaft":
            case "SoundSpot_ElectricFence":
            case "SoundSpot_Engine_Wheel":
            case "SoundSpot_Fan_Spin":
            case "SoundSpot_Piston":
            case "SoundSpot_Pistons":
            case "SoundSpot_Engine_Piston":
            case "SoundSpot_Plank_Spin":
            case "SoundSpot_PurplePipes":
            case "SoundSpot_River_Below":
            case "SoundSpot_Waterfall":
            case "Sound_Spot_Engine_Piston":
                switch (RegInt[0])
                {
                    case 1: DoSound(0, 1f, 0f, true); break;
                    case 2: DoSound(1, 1f, 0f, true); break;
                    case 3: DoSound(2, 1f, 0f, true); break;
                    case 4: DoSound(3, 1f, 0f, true); break;
                    case 5: DoSound(4, 1f, 0f, true); break;
                    case 6: DoSound(5, 1f, 0f, true); break;
                    case 7: DoSound(6, 1f, 0f, true); break;
                    case 8: DoSound(7, 1f, 0f, true); break;
                    default: DoSound(0, 1f, 0f, true); break;
                }
            break;
            case "SoundSpot_Lava": 
            case "SoundSpot_Psychetron":
            case "SoundSpot_Psychetron_Beams":
            case "SoundSpot_PurplePits":
                DoSound(0, 1f, 0f, true); break;
        }
    }
}