using Godot;
namespace Rehab;

// because AtlasTexture doesn't work in 3D
[Tool]
public partial class ParticleAtlasTexture : ImageTexture
{
    [Export]
    public ImageTexture atlas {
        get {
            return _atlas;
        }
        set {
            _atlas = value;
            //_atlas.GetImage().FlipY(); // UV y is reversed
            UpdateAtlas();
        }
    }
    [Export]
    public Rect2 region {
        get {
            return _region;
        }
        set {
            _region = value;
            UpdateAtlas();
        }
    }
    ImageTexture _atlas;
    Rect2 _region;
    
    //public ParticleAtlasTexture()
    //{
        //UpdateAtlas();
    //}

    public override int _GetWidth()
    {
        if (region.Size.X == 0)
        {
            if (atlas != null)
            {
                return atlas.GetWidth();
            }
            return 1;
        }
        else
        {
            return (int)region.Size.X;
        }
    }

    public override int _GetHeight()
    {
        if (region.Size.Y == 0)
        {
            if (atlas != null)
            {
                return atlas.GetHeight();
            }
            return 1;
        }
        else
        {
            return (int)region.Size.Y;
        }
    }

    public override bool _HasAlpha()
    {
        if (atlas != null)
        {
            return atlas.HasAlpha();
        }
        return false;
    }

    public override bool _IsPixelOpaque(int p_x, int p_y)
    {
        if (atlas == null) return true;

        int x = (int)(p_x + region.Position.X);
        int y = (int)(p_y + region.Position.Y);

        if (x < 0 || x >= atlas.GetWidth()) return false;
        if (y < 0 || y >= atlas.GetHeight()) return false;
        
        return atlas._IsPixelOpaque(x, y);
    }

    void UpdateAtlas()
    {
        Image crop = Image.CreateEmpty(_GetWidth(), _GetHeight(), false, Image.Format.Rgba8);
        //GD.Print($"updading atlas");
        if (_atlas == null) 
        {
            SetImage(crop);
            return;
        }

        int w = atlas.GetWidth();
        int h = atlas.GetHeight();
        int ch = h - 1;
        int nw = _GetWidth();
        int nh = _GetHeight();
        var image = atlas.GetImage();
        int px = 0;
        int py = 0;
        for (int y = (int)region.Position.Y; y < (int)(region.Position.Y + nh) && y < h; y++)
        {
            for (int x = (int)region.Position.X; x < (int)(region.Position.X + nw) && x < w; x++)
            {
                crop.SetPixel(px, py, image.GetPixel(x, ch - y));
                px++;
            }
            px = 0;
            py++;
        }
        //GD.Print($"image updated from {w} {h} to {nw} {nh}");

        SetImage(crop);
    }

}