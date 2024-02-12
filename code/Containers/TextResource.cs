using Godot;
namespace Rehab;
public partial class TextResource : Resource{
    [Export(PropertyHint.MultilineText)]
    public string text;
}