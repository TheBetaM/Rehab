using System;
using System.IO;

namespace Twinsanity
{
    public class Pos
    {
        public Pos(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float[] ToArray()
        {
            return new float[4] { X, Y, Z, W };
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }

    public class TwinsVector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public int GetLength()
        {
            return 16;
        }

        public void Load(BinaryReader reader, int length)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            W = reader.ReadSingle();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            writer.Write(W);
        }
    }

    public class CamRot
    {
        public CamRot(ushort pitch, ushort yaw, ushort roll)
        {
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
        }

        public ushort Pitch { get; set; }
        public ushort Yaw { get; set; }
        public ushort Roll { get; set; }
    }

    public struct Color : IEquatable<Color>
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public Color(byte r, byte g, byte b, byte a)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public Color(int a, int r, int g, int b)
        {
            A = (byte)a;
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(a, r, g, b);
        }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(r, g, b, a);
        }

        public static Color Black = new Color(255, 0, 0, 0);

        public bool Equals(Color other)
        {
            if (A != other.A) return false;
            if (R != other.R) return false;
            if (G != other.G) return false;
            if (B != other.B) return false;
            return true;
        }

        public static bool operator ==(Color left, Color right){
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right){
            return !left.Equals(right);
        }

    }

}
