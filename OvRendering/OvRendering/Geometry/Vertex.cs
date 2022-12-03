using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvRendering.OvRendering.Geometry
{
    public struct Vertex
    {
        public float[] Position;
        public float[] TexCoords;
        public float[] Normals;
        public float[] Tangent;
        public float[] Bitangent;

        public Vertex()
        {
            Position = new float[3];
            TexCoords = new float[2];
            Normals = new float[3];
            Tangent = new float[3];
            Bitangent = new float[3];
        }
    }
}
