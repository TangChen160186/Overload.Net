using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvRendering.OvRendering.Settings
{
    [Flags]
    public enum ECullingOptions
    {
        None = 0x0,
        FrustumPerModel = 0x1,
        FrustumPerMesh = 0x2
    }
}
