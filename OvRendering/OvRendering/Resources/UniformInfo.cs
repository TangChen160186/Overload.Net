using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OvRendering.OvRendering.Resources
{
    public struct UniformInfo
    {
        public string Name;
        public int Location;
        public object? DefaultValue;
        public ActiveUniformType UniformType;
    }
}
