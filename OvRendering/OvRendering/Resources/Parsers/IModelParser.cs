using Assimp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvRendering.OvRendering.Resources.Parsers
{
    public interface IModelParser
    {
        bool LoadModel(string fileName, List<Mesh> meshes, List<string> materials, PostProcessSteps processSteps);
    }
}
