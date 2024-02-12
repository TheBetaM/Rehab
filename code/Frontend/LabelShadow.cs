using Godot;
namespace Rehab;
public partial class LabelShadow : Label
{
    public Button button;

    public override void _Ready()
    {
        button = (Button)GetParent();
    }

    public override void _Process(double delta)
    {
        if (button.Text != Text)
            UpdateText();
    }

    void UpdateText()
    {
        Text = button.Text;
        HorizontalAlignment = button.Alignment;
        TextOverrunBehavior = button.TextOverrunBehavior;
        RemoveThemeFontSizeOverride("font_size");
        AddThemeFontSizeOverride("font_size", button.GetThemeFontSize("font_size"));
    }
}