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
    public class CSpotLight : CLight
    {
        public override string Name => nameof(CSpotLight);
        [DataMember]
        public float Constant
        {
            get => Data.Constant;
            set => Data.Constant = value;
        }
        [DataMember]
        public float Quadratic
        {
            get => Data.Quadratic;
            set => Data.Quadratic = value;
        }
        [DataMember]
        public float Cutoff
        {
            get => Data.Cutoff;
            set => Data.Cutoff = value;
        }
        [DataMember]
        public float OuterCutoff
        {
            get => Data.OuterCutoff;
            set => Data.OuterCutoff = value;
        }
        public CSpotLight(Actor actor) : base(actor)
        {
            Data.Type = (int)LightType.Spot;
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
        }
    }
}
