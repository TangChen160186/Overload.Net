using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK.Mathematics;
using OvMath;

namespace OvPhysics.Entities
{
    public class PhysicalSphere : PhysicalObject
    {

        private float _radius;

        public float Radius
        {
            get => _radius;
            set
            {
                if (Math.Abs(value - _radius) > 0.001)
                {
                    RecreateCollisionShape(value);
                }
            }
        }
        public PhysicalSphere(float radius = 1.0f)
        {
            CreateCollisionShape(radius);
            Init();
        }

        public PhysicalSphere(FTransform transform, float size) : base(transform)
        {
            CreateCollisionShape(size);
            Init();
        }
        private void CreateCollisionShape(float radius)
        {
            Shape = new SphereShape(radius);
            _radius = radius;
        }
        private void RecreateCollisionShape(float radius)
        {
            CreateCollisionShape(radius);
            RecreateBody();
        }
        protected override void SetLocalScaling(Vector3 scaling)
        {
            float radiusScale = MathHelper.Max(MathHelper.Max(scaling.X, scaling.Y), scaling.Z);
            Shape.LocalScaling = new BulletSharp.Math.Vector3(radiusScale);
        }


    }
}
