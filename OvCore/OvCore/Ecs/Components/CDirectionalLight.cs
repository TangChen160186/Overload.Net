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
    public class CDirectionalLight:CLight
    {
        public override string Name => nameof(CDirectionalLight);

        public CDirectionalLight(Actor actor) : base(actor)
        {
            Data.Type = (int)LightType.Directional;
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
        }
    }
}
