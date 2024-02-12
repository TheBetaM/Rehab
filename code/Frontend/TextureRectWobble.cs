using Godot;
namespace Rehab;
public partial class TextureRectWobble : TextureRect
{
    public bool mode = false;
    [Export] public bool isAnim = true;
    [Export] public float scaleMagnitude = 0.05f;
    [Export] public float speed = 0.5f;

    public override void _Process(double delta)
    {
        if (!isAnim) return;
        //PivotOffset = new Vector2(Size.X / 2f, Size.Y / 2f);

        if (!mode)
        {
            Scale = Scale.MoveToward(Vector2.One + (Vector2.One * scaleMagnitude), (float)delta * speed);
            if (Scale.X + 0.01f >= 1.0f + scaleMagnitude)
                mode = !mode;
        }
        else
        {
            Scale = Scale.MoveToward(Vector2.One - (Vector2.One * scaleMagnitude), (float)delta * speed);
            if (Scale.X - 0.01f <= 1.0f - scaleMagnitude)
                mode = !mode;
        }
    }
}