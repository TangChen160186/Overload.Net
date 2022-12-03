using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvRendering.OvRendering.Resources
{
    public interface IMesh : IDisposable
    {
        void Bind();
        void Unbind();
        uint VertexCount { get; }
        uint IndexCount { get; }

    }
}
