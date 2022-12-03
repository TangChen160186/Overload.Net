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
    public class CPointLight:CLight
    {
        public override string Name => nameof(CPointLight);
        [DataMember]
        public float Constant
        {
            get=>Data.Constant;
            set=>Data.Constant = value;
        }
        [DataMember]
        public float Linear
        {
            get=> Data.Linear;
            set=>Data.Linear = value;
        }
        [DataMember]
        public float Quadratic
        {
            get=>Data.Quadratic;
            set=>Data.Quadratic = value;
        }

        public CPointLight(Actor actor) : base(actor)
        {
            Data.Type = (int)LightType.Point;
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
        }
    }
}
