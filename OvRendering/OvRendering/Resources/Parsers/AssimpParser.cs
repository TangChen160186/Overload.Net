using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OvRendering.OvRendering.Geometry;

namespace OvRendering.OvRendering.Resources.Parsers
{
    public class AssimpParser : IModelParser
    {
        public bool LoadModel(string fileName, List<Mesh> meshes, List<string> materials, PostProcessSteps postProcessSteps)
        {
            using AssimpContext context = new AssimpContext();
            Scene scene = context.ImportFile(fileName, postProcessSteps);
            if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete ||
                scene.RootNode == null)
                return false;
            ProcessMaterials(scene, materials);
            Assimp.Matrix4x4 identity = Matrix4x4.Identity;
            ProcessNode(identity, scene.RootNode, scene, meshes);
            return true;
        }

        private void ProcessMaterials(Scene scene, List<string> materials)
        {
            for (var i = 0; i < scene.MaterialCount; i++)
            {
                var material = scene.Materials[i];
                if (material != null)
                {
                    if (material.HasName)
                    {
                        materials.Add(material.Name);
                    }
                }
            }
        }

        private void ProcessNode(Assimp.Matrix4x4 transform, Assimp.Node node, Scene scene, List<Mesh> meshes)
        {
            for (var i = 0; i < node.MeshCount; i++)
            {
                List<Vertex> vertices = new List<Vertex>();
                List<uint> indices = new List<uint>();
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                ProcessMesh(transform, mesh, scene, vertices, indices);
                meshes.Add(new Mesh(vertices, indices, (uint)mesh.MaterialIndex));
            }

            for (var i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(transform, node.Children[i], scene, meshes);
            }
        }

        private void ProcessMesh(Assimp.Matrix4x4 transform, Assimp.Mesh mesh, Scene scene, List<Vertex> outVertices,
            List<uint> outIndices)
        {
            for (var i = 0; i < mesh.VertexCount; i++)
            {
                Assimp.Vector3D position = transform * mesh.Vertices[i];
                Assimp.Vector3D normal = transform * (mesh.HasNormals ? mesh.Normals[i] : new Vector3D(0, 0, 0));
                Assimp.Vector3D textureCoords = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0, 0, 0);
                Assimp.Vector3D tangent = mesh.HasTangentBasis ? transform * mesh.Tangents[i] : new Vector3D(0, 0, 0);
                Assimp.Vector3D biTangent = mesh.BiTangents.Count > 0 ? transform * mesh.BiTangents[i] : new Vector3D(0, 0, 0);

                outVertices.Add(new Vertex()
                {
                    Position = new[] { position.X, position.Y, position.Z },
                    TexCoords = new[] { textureCoords.X, textureCoords.Y },
                    Normals = new[] { normal.X, normal.Y, normal.Z },
                    Tangent = new[] { tangent.X, tangent.Y, tangent.Z },
                    Bitangent = new[] { biTangent.X, biTangent.Y, biTangent.Z },
                });
            }

            for (var faceId = 0; faceId < mesh.FaceCount; faceId++)
            {
                var face = mesh.Faces[faceId];
                for (var indexId = 0; indexId < face.IndexCount; indexId++)
                {
                    outIndices.Add((uint)face.Indices[indexId]);
                }
            }
        }
    }
}
