using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OvMath;
using OvRendering.OvRendering.Entities;

namespace OvCore.OvCore.Ecs.Components
{
    [DataContract]
    public class CAmbientBoxLight : CLight
    {
        public override string Name => nameof(CAmbientBoxLight);

        [DataMember]
        public Vector3 Size
        {
            get => new Vector3(Data.Constant, Data.Linear, Data.Quadratic);
            set
            {
                Data.Constant = value.X;
                Data.Linear = value.Y;
                Data.Quadratic = value.Z;
            }
        }
        public CAmbientBoxLight(Actor actor) : base(actor)
        {
            Data.Type = (int)LightType.AmbientBox;
            Data.Intensity = 0.1f;
            Data.Constant = 1.0f;
            Data.Linear = 1.0f;
            Data.Quadratic = 1.0f;
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
        }
    }
}
