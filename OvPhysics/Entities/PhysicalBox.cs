using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK.Mathematics;
using OvMath;
using OvPhysics.Tools;

namespace OvPhysics.Entities
{
    public class PhysicalBox : PhysicalObject
    {
        private Vector3 _size;
        public Vector3 Size
        {
            get => _size;
            set
            {
                if (value != _size)
                {
                    RecreateCollisionShape(value);
                }

            }
        }

        public PhysicalBox() : this(new Vector3(0.5f, 0.5f, 0.5f))
        {

        }
        public PhysicalBox(Vector3 size)
        {
            CreateCollisionShape(size);
            Init();
        }

        public PhysicalBox(FTransform transform, Vector3 size) : base(transform)
        {
            CreateCollisionShape(size);
            Init();
        }
        private void CreateCollisionShape(Vector3 size)
        {
            Shape = new BoxShape(size.X, size.Y, size.Z);
            _size = size;
        }
        private void RecreateCollisionShape(Vector3 size)
        {
            CreateCollisionShape(size);
            RecreateBody();
        }
        protected override void SetLocalScaling(Vector3 scaling)
        {
            Shape.LocalScaling = Conversion.ToBtVector3(scaling);
        }
    }
}
