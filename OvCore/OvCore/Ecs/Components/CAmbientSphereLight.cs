using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OvRendering.OvRendering.Entities;

namespace OvCore.OvCore.Ecs.Components
{
    [DataContract]
    public class CAmbientSphereLight:CLight
    {
        public override string Name => nameof(CAmbientSphereLight);

        public CAmbientSphereLight(Actor actor) : base(actor)
        {
            Data.Intensity = 0.1f;
            Data.Constant = 1.0f;
            Data.Type = (int)LightType.AmbientSphere;
        }
        [DataMember]
        public float Radius
        {
            get => Data.Quadratic;
            set=> Data.Constant = value;
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
        }
    }
}
