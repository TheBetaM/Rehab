using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Twinsanity;

namespace RehabSetup
{
    public class GodotBinaryCollisionShape : GodotBinaryResourceFile
    {

        public override string ResType => "ConcavePolygonShape3D";

        public GodotBinaryCollisionShape()
        {

        }

        public GodotBinaryCollisionShape(ColData Data, int surface)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var posArray = new List<Vector3>();
            List<int> LayerIndices = new List<int>();
            for (int g = 0; g < Data.Tris.Count; g++)
            {
                if (Data.Tris[g].Surface == surface)
                {
                    LayerIndices.Add(Data.Tris[g].Vert1);
                    LayerIndices.Add(Data.Tris[g].Vert2);
                    LayerIndices.Add(Data.Tris[g].Vert3);
                }
            }
            for (int g = 0; g < LayerIndices.Count; g++)
            {
                posArray.Add(new Vector3(-Data.Vertices[LayerIndices[g]].X, Data.Vertices[LayerIndices[g]].Y, Data.Vertices[LayerIndices[g]].Z));
            }
            res.Add("data", posArray.ToArray());
            res.Add("backface_collision", true);
            Resources.Add(res);
        }
    }
}