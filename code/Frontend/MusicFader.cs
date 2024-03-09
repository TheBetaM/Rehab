using Godot;
namespace Rehab;
public partial class MusicFader : AudioStreamPlayer
{
    public bool IsFadingOut = false;

    public override void _Process(double delta)
    {
        if (!IsFadingOut) return;
        if (VolumeDb > -80.0)
            VolumeDb -= (float)delta * 20f;
        else
        {
            Stop();
            IsFadingOut = false;
        }
    }
}