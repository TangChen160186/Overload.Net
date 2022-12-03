using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using BulletSharp.Math;
using OpenTK.Mathematics;
using OvMath;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace OvPhysics.Tools
{
    internal static class Conversion
    {
        public static BulletSharp.Math.Vector3 ToBtVector3(OpenTK.Mathematics.Vector3 vector)
        {
            return new BulletSharp.Math.Vector3(vector.X, vector.Y, vector.Z);
        }

        public static OpenTK.Mathematics.Vector3 ToTkVector3(BulletSharp.Math.Vector3 vector)
        {
            return new OpenTK.Mathematics.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }
        public static BulletSharp.Math.Vector4 ToBtVector4(OpenTK.Mathematics.Vector4 vector)
        {
            return new BulletSharp.Math.Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }

        public static OpenTK.Mathematics.Vector4 ToTkVector4(BulletSharp.Math.Vector4 vector)
        {
            return new OpenTK.Mathematics.Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
        }
        public static BulletSharp.Math.Quaternion ToBtQuaternion(OpenTK.Mathematics.Quaternion quaternion)
        {
            return new BulletSharp.Math.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static OpenTK.Mathematics.Quaternion ToTkQuaternion(BulletSharp.Math.Quaternion quaternion)
        {
            return new OpenTK.Mathematics.Quaternion((float)quaternion.X, (float)quaternion.Y, (float)quaternion.Z, (float)quaternion.W);
        }

        public static BulletSharp.Math.Matrix ToBtTransform(FTransform transform)
        {
            var worldPosition = transform.WorldPosition;
            var worldRotation = transform.WorldRotation;
            var matrix = Matrix4.CreateTranslation(worldPosition) * Matrix4.CreateFromQuaternion(worldRotation);
            return new BulletSharp.Math.Matrix(matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44);
        }

        public static FTransform ToFTransform(BulletSharp.Math.Matrix matrix)
        {
            matrix.Decompose(out var scale, out var rotation, out var translation);
            return new FTransform(ToTkVector3(translation), ToTkQuaternion(rotation), Vector3.One);
        }
    }
}
