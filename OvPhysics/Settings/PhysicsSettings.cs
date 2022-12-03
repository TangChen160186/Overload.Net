using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OvPhysics.Settings
{
    public struct PhysicsSettings
    {
        public Vector3 Gravity = new Vector3(0, -9.81f, 0f);

        public PhysicsSettings() { }
    }
}
