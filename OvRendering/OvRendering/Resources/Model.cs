using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OvRendering.OvRendering.Geometry;

namespace OvRendering.OvRendering.Resources
{
    public class Model : IDisposable
    {
        private BoundingSphere _boundingSphere = new();
        public string Path { get; }

        public List<Mesh> Meshes { get; internal set; } = new();
        public List<string> MaterialNames { get; internal set; } = new();

        public BoundingSphere BoundingSphere
        {
            get => _boundingSphere;
            internal set => _boundingSphere = value;
        }

        internal Model(string path)
        {
            Path = path;
        }
        internal void ComputeBoundingSphere()
        {
            if (Meshes.Count == 1)
            {
                BoundingSphere = Meshes[0].BoundingSphere;
            }
            else
            {
                _boundingSphere.Position = Vector3.Zero;
                _boundingSphere.Radius = 0.0f;

                if (Meshes.Any())
                {
                    float minX = float.MaxValue;
                    float minY = float.MaxValue;
                    float minZ = float.MaxValue;

                    float maxX = float.MinValue;
                    float maxY = float.MinValue;
                    float maxZ = float.MinValue;

                    foreach (var mesh in Meshes)
                    {
                        var boundingSphere = mesh.BoundingSphere;
                        minX = MathHelper.Min(minX, boundingSphere.Position.X - boundingSphere.Radius);
                        minY = MathHelper.Min(minY, boundingSphere.Position.Y - boundingSphere.Radius);
                        minZ = MathHelper.Min(minZ, boundingSphere.Position.Z - boundingSphere.Radius);

                        maxX = MathHelper.Max(maxX, boundingSphere.Position.X + boundingSphere.Radius);
                        maxY = MathHelper.Max(maxY, boundingSphere.Position.Y + boundingSphere.Radius);
                        maxZ = MathHelper.Max(maxZ, boundingSphere.Position.Z + boundingSphere.Radius);
                    }

                    _boundingSphere.Position = new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2.0f;

                    _boundingSphere.Radius = Vector3.Distance(BoundingSphere.Position, new Vector3(minX, minY, minZ));
                }
            }
        }

        public void Dispose()
        {
            foreach (var mesh in Meshes)
            {
                mesh.Dispose();
            }
        }
    }
}
