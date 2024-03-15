using Godot;
namespace Rehab;
public partial class PlayerOnlyTrigger : Area3D
{
    [Signal]
    public delegate void OnTriggerEnterEventHandler();
    [Signal]
    public delegate void OnTriggerExitedEventHandler();

    public override void _Ready()
    {
        BodyEntered += OnTrigger;
        BodyExited += OnTriggerExit;
    }

    void OnTrigger(Node3D body)
    {
        if (body != AgentCharacter.activeCharacter) return;
        EmitSignal(SignalName.OnTriggerEnter);
    }

    void OnTriggerExit(Node3D body)
    {
        if (body != AgentCharacter.activeCharacter) return;
        EmitSignal(SignalName.OnTriggerExited);
    }

}