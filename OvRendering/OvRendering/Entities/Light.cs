using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OvMath;

namespace OvRendering.OvRendering.Entities
{
    public enum LightType {Point, Directional, Spot, AmbientBox, AmbientSphere }
    public class Light
    {
        public FTransform Transform { get; set; }
        public Vector3 Color { get; set; }
        public float Intensity { get; set; } = 1;
        public float Constant { get; set; } = 0;
        public float Linear { get; set; } = 0;
        public float Quadratic { get; set; } = 1;
        public float Cutoff { get; set; } = 12;
        public float OuterCutoff { get; set; } = 15;
        public float Type { get; set; } = 0;


        public Light(FTransform transform)
        {
            Transform = transform;
        }

        public Light(FTransform transform, LightType type)
        {
            Transform = transform;
            Type = (float)type;
        }


        /// <summary>
        /// 生成Light参数矩阵，方便输入着色器
        /// </summary>
        /// <returns></returns>
        public Matrix4 GenerateMatrix()
        {
            Matrix4 result = new Matrix4();
            var position = Transform.WorldPosition;
            result[0, 0] = position.X;
            result[0, 1] = position.Y;
            result[0, 2] = position.Z;

            var forward = Transform.WorldForward;
            result[1, 0] = position.X;
            result[1, 1] = position.Y;
            result[1, 2] = position.Z;

            result[2, 0] = Pack(Color);

            result[3, 0] = Type;
            result[3, 1] = Cutoff;
            result[3, 2] = OuterCutoff;

            result[0, 3] = Constant;
            result[1, 3] = Linear;
            result[2, 3] = Quadratic;
            result[3, 3] = Intensity;
            return result;
        }

        private int Pack(byte c0, byte c1, byte c2, byte c3)
        {
            return c0 << 24 | c1 << 16 | c2 << 8 | c3;
        }
        private int Pack(Vector3 toPack)
        {
            return Pack((byte)toPack.X, (byte)toPack.Y, (byte)toPack.Z, 0);
        }
        /// <summary>
        /// 计算光的衰减
        /// </summary>
        /// <param name="constant"></param>
        /// <param name="linear"></param>
        /// <param name="quadratic"></param>
        /// <param name="intensity"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private float CalculateLuminosity(float constant, float linear, float quadratic, float intensity, float distance)
        {
            var attenuation = (constant + linear * distance + quadratic * (distance * distance));
            return (1.0f / attenuation) * MathHelper.Abs(Intensity);
        }

        private float CalculatePointLightRadius(float constant, float linear, float quadratic, float intensity)
        {
            float threshold = 1 / 255.0f;
            float step = 1.0f;
            float distance = 0;
            if (CalculateLuminosity(constant, linear, quadratic, intensity, 1000) > threshold)
            {
                return float.PositiveInfinity;
            }
            else if (CalculateLuminosity(constant, linear, quadratic, intensity, 20) < threshold)
            {
                distance = 0;
            }
            else if (CalculateLuminosity(constant, linear, quadratic, intensity, 750) > threshold)
            {
                distance = 750;
            }
            else if (CalculateLuminosity(constant, linear, quadratic, intensity, 50) < threshold)
            {
                distance = 20 + step;
            }
            else if (CalculateLuminosity(constant, linear, quadratic, intensity, 100) < threshold)
            {
                distance = 50 + step;
            }
            else if (CalculateLuminosity(constant, linear, quadratic, intensity, 500) > threshold)
            {
                distance = 500;
            }
            else if (CalculateLuminosity(constant, linear, quadratic, intensity, 250) > threshold)
            {
                distance = 250;
            }

            while (true)
            {
                if (CalculateLuminosity(constant, linear, quadratic, intensity, 100) < threshold)
                {
                    return distance;
                }
                else
                {
                    distance += step;
                }
            }
        }

        private float CalculateAmbientBoxLightRadius(Vector3 position, Vector3 size)
        {
            return Vector3.Distance(position, position + size);
        }

        public float GetEffectRange()
        {
            switch ((LightType)(int)(Type))
            {
                case LightType.Point:
                case LightType.Spot:
                    return CalculatePointLightRadius(Constant, Linear, Quadratic, Intensity);
                case LightType.AmbientBox:
                    return CalculateAmbientBoxLightRadius(Transform.WorldPosition,
                        new Vector3(Constant, Linear, Quadratic));
                case LightType.AmbientSphere: return Constant;
            }

            return float.PositiveInfinity;
        }



    }
}
