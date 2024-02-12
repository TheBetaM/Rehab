using Godot;
namespace Rehab;
public partial class RehabMenuButton : Button
{
    public bool mode = false;
    public bool isAnim = false;
    public bool quiet = false;
    Vector2 origScale = Vector2.One;

    public override void _Process(double delta)
    {
        if (!isAnim) return;
        PivotOffset = new Vector2(Size.X / 2f, Size.Y / 2f);

        if (!mode)
        {
            Scale = Scale.MoveToward(origScale * new Vector2(0.9f, 0.9f), (float)delta * 0.5f);
            if (Scale.X <= origScale.X * 0.91f)
                mode = !mode;
        }
        else
        {
            Scale = Scale.MoveToward(origScale * new Vector2(1.1f, 1.1f), (float)delta * 0.5f);
            if (Scale.X >= origScale.X * 1.09f)
                mode = !mode;
        }
    }

    public void StartFocus()
    {
        if (!quiet)
            RehabScene.Root.PlayMenuSound_Select();
        origScale = Scale;
        PivotOffset = new Vector2(Size.X / 2f, Size.Y / 2f);
        Scale = origScale * new Vector2(1.1f, 1.1f);
        Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        mode = false;
        isAnim = true;
    }

    public void EndFocus()
    {
        Scale = origScale;
        Modulate = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        isAnim = false;
    }

    public void OnPress()
    {
        if (!quiet)
            RehabScene.Root.PlayMenuSound_Click();
    }
}