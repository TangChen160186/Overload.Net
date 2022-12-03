using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OvMath;
using OvRendering.OvRendering.Geometry;

namespace OvRendering.OvRendering.Data
{
    internal enum FrustumSide
    {
        Right = 0, Left = 1, Bottom = 2, Top = 3, Back = 4, Front = 5,
    }

    internal enum PlaneData
    {
        A = 0, B = 1, C = 2, D = 3,
    }

    public class Frustum
    {
        private readonly float[,] _frustum = new float[6, 4];

        public void CalculateFrustum(Matrix4 viewProjection)
        {
            var columnMajorViewProjection = Matrix4.Transpose(viewProjection);
            //Right
            _frustum[0, 0] = columnMajorViewProjection[0, 3] - columnMajorViewProjection[0, 0];
            _frustum[0, 1] = columnMajorViewProjection[1, 3] - columnMajorViewProjection[1, 0];
            _frustum[0, 2] = columnMajorViewProjection[2, 3] - columnMajorViewProjection[2, 0];
            _frustum[0, 3] = columnMajorViewProjection[3, 3] - columnMajorViewProjection[3, 0];
            NormalizePlane(_frustum, 0);
            //Left
            _frustum[1, 0] = columnMajorViewProjection[0, 3] + columnMajorViewProjection[0, 0];
            _frustum[1, 1] = columnMajorViewProjection[1, 3] + columnMajorViewProjection[1, 0];
            _frustum[1, 2] = columnMajorViewProjection[2, 3] + columnMajorViewProjection[2, 0];
            _frustum[1, 3] = columnMajorViewProjection[3, 3] + columnMajorViewProjection[3, 0];
            NormalizePlane(_frustum, 1);

            //Bottom
            _frustum[2, 0] = columnMajorViewProjection[0, 3] + columnMajorViewProjection[0, 1];
            _frustum[2, 1] = columnMajorViewProjection[1, 3] + columnMajorViewProjection[1, 1];
            _frustum[2, 2] = columnMajorViewProjection[2, 3] + columnMajorViewProjection[2, 1];
            _frustum[2, 3] = columnMajorViewProjection[3, 3] + columnMajorViewProjection[3, 1];
            NormalizePlane(_frustum, 2);

            //Top
            _frustum[3, 0] = columnMajorViewProjection[0, 3] - columnMajorViewProjection[0, 1];
            _frustum[3, 1] = columnMajorViewProjection[1, 3] - columnMajorViewProjection[1, 1];
            _frustum[3, 2] = columnMajorViewProjection[2, 3] - columnMajorViewProjection[2, 1];
            _frustum[3, 3] = columnMajorViewProjection[3, 3] - columnMajorViewProjection[3, 1];
            NormalizePlane(_frustum, 3);

            //Back
            _frustum[4, 0] = columnMajorViewProjection[0, 3] - columnMajorViewProjection[0, 2];
            _frustum[4, 1] = columnMajorViewProjection[1, 3] - columnMajorViewProjection[1, 2];
            _frustum[4, 2] = columnMajorViewProjection[2, 3] - columnMajorViewProjection[2, 2];
            _frustum[4, 3] = columnMajorViewProjection[3, 3] - columnMajorViewProjection[3, 2];
            NormalizePlane(_frustum, 4);

            //Front
            _frustum[5, 0] = columnMajorViewProjection[0, 3] + columnMajorViewProjection[0, 2];
            _frustum[5, 1] = columnMajorViewProjection[1, 3] + columnMajorViewProjection[1, 2];
            _frustum[5, 2] = columnMajorViewProjection[2, 3] + columnMajorViewProjection[2, 2];
            _frustum[5, 3] = columnMajorViewProjection[3, 3] + columnMajorViewProjection[3, 2];
            NormalizePlane(_frustum, 5);
        }

        public bool PointFrustum(float x, float y, float z)
        {
            for (var i = 0; i < 6; i++)
            {
                if (_frustum[i, 0] * x + _frustum[i, 1] * y + _frustum[i, 2] * z + _frustum[i, 3] <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool SphereInFrustum(float x, float y, float z, float radius)
        {
            for (var i = 0; i < 6; i++)
            {
                if (_frustum[i, 0] * x + _frustum[i, 1] * y + _frustum[i, 2] * z + _frustum[i, 3] <= -radius)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CubeInFrustum(float x, float y, float z, float size)
        {
            for (var i = 0; i < 6; i++)
            {
                if (_frustum[i, 0] * (x - size) + _frustum[i, 1] * (y - size) + _frustum[i, 2] * (z - size) + _frustum[i, 3] > 0) continue;
                if (_frustum[i, 0] * (x + size) + _frustum[i, 1] * (y - size) + _frustum[i, 2] * (z - size) + _frustum[i, 3] > 0) continue;
                if (_frustum[i, 0] * (x - size) + _frustum[i, 1] * (y + size) + _frustum[i, 2] * (z - size) + _frustum[i, 3] > 0) continue;
                if (_frustum[i, 0] * (x - size) + _frustum[i, 1] * (y - size) + _frustum[i, 2] * (z + size) + _frustum[i, 3] > 0) continue;
                if (_frustum[i, 0] * (x + size) + _frustum[i, 1] * (y + size) + _frustum[i, 2] * (z - size) + _frustum[i, 3] > 0) continue;
                if (_frustum[i, 0] * (x + size) + _frustum[i, 1] * (y - size) + _frustum[i, 2] * (z + size) + _frustum[i, 3] > 0) continue;
                if (_frustum[i, 0] * (x - size) + _frustum[i, 1] * (y + size) + _frustum[i, 2] * (z + size) + _frustum[i, 3] > 0) continue;
                if (_frustum[i, 0] * (x + size) + _frustum[i, 1] * (y + size) + _frustum[i, 2] * (z + size) + _frustum[i, 3] > 0) continue;
                return false;
            }

            return true;
        }

        public bool BoundingSphereInFrustum(BoundingSphere boundingSphere, FTransform transform)
        {
            var position = transform.WorldPosition;
            var rotation = transform.WorldRotation;
            var scale = transform.WorldScale;

            float maxScale = MathHelper.Max(MathHelper.Max(scale.X, scale.Y), scale.Z);
            float scaleRadius = boundingSphere.Radius * maxScale;
            var sphereOffset = rotation * boundingSphere.Position;// rotate point......
            var worldCenter = position + sphereOffset;
            return SphereInFrustum(worldCenter.X, worldCenter.Y, worldCenter.Z, scaleRadius);
        }


        public float[] GetNearPlane()
        {
            return new[] { _frustum[5, 0], _frustum[5, 1], _frustum[5, 2], _frustum[5, 3] };
        }

        public float[] GetFarPlane()
        {
            return new[] { _frustum[4, 0], _frustum[4, 1], _frustum[4, 2], _frustum[4, 3] };
        }

        private void NormalizePlane(float[,] frustum, int side)
        {
            float magnitude = (float)MathHelper.Sqrt(frustum[side, 0] * frustum[side, 0] +
                                                     frustum[side, 1] * frustum[side, 1] +
                                                     frustum[side, 2] * frustum[side, 2]);
            frustum[side, 0] /= magnitude;
            frustum[side, 1] /= magnitude;
            frustum[side, 2] /= magnitude;
            frustum[side, 3] /= magnitude;
        }
    }
}
