using Godot;
namespace Rehab.Agents.Furniture.School.Rooftop;

public partial class GargoyleSmall : AgentFurniture
{
    public override void _Ready()
    { 
        base._Ready();
        
        Set("collision_layer", 0);
        Set("collision_mask", 0);
        DoAnimation(4, true);
        var hook = (AgentChiChiGrass)SubActorsScenes[0].Instantiate();
        hook.Visible = false;
        hook.Name = "ChiChiGrass_Ceiling";
        hook.LinkPoint = LinkPoint;
        AddChild(hook);
    }
}