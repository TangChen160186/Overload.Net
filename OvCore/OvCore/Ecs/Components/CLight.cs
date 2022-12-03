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
    public class CLight:AComponent
    {
        public Light Data { get; protected set; }
        public override string Name => nameof(CLight);
        [DataMember]
        public Vector3 Color
        {
            get => Data.Color;
            set => Data.Color = value;
        }
        [DataMember]
        public float Intensity
        {
            get=> Data.Intensity;
            set => Data.Intensity = value;
        }
        public CLight(Actor actor) : base(actor)
        {
            Data = new Light(Owner.Transform.Transform);
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
            Data.Transform = owner.Transform.Transform;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Data = new Light(new FTransform());
        }
    }
}
