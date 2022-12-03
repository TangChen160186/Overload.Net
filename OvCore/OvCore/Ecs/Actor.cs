using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvCore.OvCore.Ecs.Components;

namespace OvCore.OvCore.Ecs
{
    public class Actor
    {
        public CTransform Transform { get; }
        public bool IsActive { get; }
        public Actor()
        {
            Transform = new CTransform(this);
        }
    }
}
