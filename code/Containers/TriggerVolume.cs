using Godot;
using Twinsanity;
namespace Rehab;
public partial class TriggerVolume : Area3D
{
    [Export]
    public int MsgOnEnterOnce = -1;
    [Export]
    public int MsgOnEnter = -1;
    [Export]
    public int MsgOnStay = -1;
    [Export]
    public int MsgOnExit = -1;
    [Export]
    public Godot.Collections.Array<NodePath> InstanceRefs;
    [Export]
    public float SomeFloat;
    [Export]
    public uint Mask;
    public bool EnterOnceDone;

    public override void _Ready()
    {
        BodyEntered += OnTriggerEnter;
        BodyExited += OnTriggerExit;
        EnterOnceDone = false;
    }

    void OnTriggerEnter(Node3D body)
    {
        if (InstanceRefs == null) return;
        if (body is not AgentCharacter agent) return;
        if (AgentCharacter.activeCharacter != agent) return;
        foreach (var item in InstanceRefs)
        {
            if (GetNodeOrNull(item) == null) continue;
            var recieverNode = GetNode(item);
            Agent reciever = null;
            if (recieverNode is ActorInstance inst)
            {
                reciever = inst.Actor;
            }
            else if (recieverNode is Agent a)
            {
                reciever = a;
            }
            if (reciever == null) continue;
            if (MsgOnEnter != -1)
            {
                reciever.OnMessage(MsgOnEnter);
            }
            if (MsgOnStay != -1)
            {
                reciever.OnMessage(MsgOnStay);
            }
            if (!EnterOnceDone && MsgOnEnterOnce != -1)
            {
                EnterOnceDone = true;
                reciever.OnMessage(MsgOnEnterOnce);
            }
        }
    }

    void OnTriggerExit(Node3D body)
    {
        if (InstanceRefs == null) return;
        if (body is not AgentCharacter agent) return;
        if (AgentCharacter.activeCharacter != agent) return;
        foreach (var item in InstanceRefs)
        {
            if (GetNodeOrNull(item) == null) continue;
            var recieverNode = GetNode(item);
            Agent reciever = null;
            if (recieverNode is ActorInstance inst)
            {
                reciever = inst.Actor;
            }
            else if (recieverNode is Agent a)
            {
                reciever = a;
            }
            if (reciever == null) continue;
            if (MsgOnExit != -1)
            {
                reciever.OnMessage(MsgOnExit);
            }
        }
    }

}