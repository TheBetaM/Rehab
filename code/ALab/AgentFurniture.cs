using System.Collections.Generic;
using Godot;
namespace Rehab;
public partial class AgentFurniture : Agent
{
    public override void _Ready()
    {
        base._Ready();

        DoAnimation(0, true);
    }
}