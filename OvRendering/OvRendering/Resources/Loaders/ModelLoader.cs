using Assimp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvRendering.OvRendering.Resources.Parsers;

namespace OvRendering.OvRendering.Resources.Loaders
{
    public class ModelLoader
    {
        private static readonly AssimpParser Assimp = new();
        private ModelLoader() { }

        public static Model? Create(string filePath, PostProcessSteps postProcessSteps = PostProcessSteps.None)
        {
            Model result = new Model(filePath);

            if (Assimp.LoadModel(filePath, result.Meshes, result.MaterialNames, postProcessSteps))
            {
                result.ComputeBoundingSphere();
                return result;
            }

            return null;
        }

        public static void Reload(Model model, string filePath, PostProcessSteps postProcessSteps = PostProcessSteps.None)
        {
            Model? newModel = Create(filePath, postProcessSteps);
            if (newModel != null)
            {
                model.Meshes = newModel.Meshes;
                model.MaterialNames = newModel.MaterialNames;
                model.BoundingSphere = newModel.BoundingSphere;
            }
        }

        public static bool Destroy(ref Model? model)
        {
            if (model != null)
            {
                model.Dispose();
                model = null;
                return true;
            }

            return false;
        }
    }
}
