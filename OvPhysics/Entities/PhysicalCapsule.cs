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
    public class PhysicalCapsule : PhysicalObject
    {
        private float _radius;
        private float _height;
        public float Radius
        {
            get => _radius;
            set
            {
                if (Math.Abs(value - _radius) > 0.01f)
                {
                    RecreateCollisionShape(value, _height);
                }
            }
        }

        public float Height
        {
            get => _height;
            set
            {
                if (Math.Abs(value - _height) > 0.01f)
                {
                    RecreateCollisionShape(_radius, value);
                }
            }
        }

        public PhysicalCapsule(float radius = 1f, float height = 1f)
        {
            CreateCollisionShape(radius, height);
            Init();
        }

        public PhysicalCapsule(FTransform transform, float radius = 1f, float height = 1f) : base(transform)
        {
            CreateCollisionShape(radius, height);
            Init();
        }

        private void CreateCollisionShape(float radius, float height)
        {
            Shape = new CapsuleShape(radius, height);
            _radius = radius;
            _height = height;
        }

        private void RecreateCollisionShape(float radius, float height)
        {
            CreateCollisionShape(radius, height);
            RecreateBody();
        }

        protected override void SetLocalScaling(Vector3 scaling)
        {
            Shape.LocalScaling = new BulletSharp.Math.Vector3(MathHelper.Max(scaling.X, scaling.Y), scaling.Y, 1.0f);
        }
    }
}
