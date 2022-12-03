using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvPhysics.Entities
{
    public class RayCastHit
    {
        public List<PhysicalObject> ResultObjects = new();
        public PhysicalObject FirstResultObject = null!;

        public static implicit operator bool(RayCastHit? hit) => hit != null;
    }
}
