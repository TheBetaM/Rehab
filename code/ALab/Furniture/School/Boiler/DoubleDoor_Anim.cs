using Godot;
namespace Rehab.Agents.Furniture.School.Boiler;

public partial class DoubleDoor_Anim : AgentFurniture
{
    public override void _Ready()
    {
        base._Ready();

        // temp to make exploring in explorer easier
        Connect("body_entered", Callable.From<Node3D>(OnSolidCollisionEnter));
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

    public override void OnNonSolidCollisionEnter(Node3D body)
    {
        OnDoorTouch(body);
    }

    public void OnSolidCollisionEnter(Node3D body)
    {
        OnDoorTouch(body);
    }
}