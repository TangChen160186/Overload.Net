using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OvRendering.OvRendering.Geometry
{
    public struct BoundingSphere
    {
        public Vector3 Position;
        public float Radius = 0;

        public BoundingSphere()
        {
            Position = new Vector3();
        }
    }
}
