using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Twinsanity;

namespace RehabSetup
{
    public class GodotBinaryArrayOccluder : GodotBinaryResourceFile
    {

        public override string ResType => "ArrayOccluder3D";

        public GodotBinaryArrayOccluder()
        {

        }

        public GodotBinaryArrayOccluder(List<Pos> Vertices, List<int> Indices)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var posArray = new List<Vector3>();
            for (int g = 0; g < Vertices.Count; g++)
            {
                posArray.Add(new Vector3(Vertices[g].X, Vertices[g].Y, Vertices[g].Z));
            }
            res.Add("vertices", posArray.ToArray());
            res.Add("indices", Indices.ToArray());
            Resources.Add(res);
        }
    }
}