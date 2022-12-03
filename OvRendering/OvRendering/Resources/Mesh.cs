using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OvRendering.OvRendering.Buffers;
using OvRendering.OvRendering.Geometry;

namespace OvRendering.OvRendering.Resources
{
    public class Mesh : IMesh
    {
        private VertexArray _vertexArray = null!;
        private VertexBuffer<float> _vertexBuffer = null!;
        private IndexBuffer _indexBuffer = null!;
        private BoundingSphere _boundingSphere;

        public uint VertexCount { get; }
        public uint IndexCount { get; }
        public uint MaterialIndex { get; }
        /// <summary>
        /// 当前Mesh的包围球
        /// </summary>
        public BoundingSphere BoundingSphere => _boundingSphere;

        public Mesh(List<Vertex> vertices, List<uint> indices, uint materialIndex)
        {
            VertexCount = (uint)vertices.Count;
            IndexCount = (uint)indices.Count;
            MaterialIndex = materialIndex;

            CreateBuffers(vertices, indices);
            ComputeBoundingSphere(vertices);
        }


        public void Bind()
        {
            _vertexArray.Bind();
        }
        public void Unbind()
        {
            _vertexArray.Unbind();
        }

        public uint GetMaterialIndex() => MaterialIndex;

        private void CreateBuffers(List<Vertex> vertices, List<uint> indices)
        {
            List<float> vertexData = new List<float>();
            List<uint> rawIndices = new List<uint>();
            foreach (var vertex in vertices)
            {
                vertexData.Add(vertex.Position[0]);
                vertexData.Add(vertex.Position[1]);
                vertexData.Add(vertex.Position[2]);

                vertexData.Add(vertex.TexCoords[0]);
                vertexData.Add(vertex.TexCoords[1]);

                vertexData.Add(vertex.Normals[0]);
                vertexData.Add(vertex.Normals[1]);
                vertexData.Add(vertex.Normals[2]);

                vertexData.Add(vertex.Tangent[0]);
                vertexData.Add(vertex.Tangent[1]);
                vertexData.Add(vertex.Tangent[2]);

                vertexData.Add(vertex.Bitangent[0]);
                vertexData.Add(vertex.Bitangent[1]);
                vertexData.Add(vertex.Bitangent[2]);
            }

            _vertexArray = new VertexArray();
            _vertexBuffer = new VertexBuffer<float>(vertexData.ToArray());
            _indexBuffer = new IndexBuffer(indices.ToArray());

            var vertexSize = sizeof(float) * 14;
            _vertexArray.BindAttribute(0, _vertexBuffer, VertexAttribPointerType.Float, 3, vertexSize, IntPtr.Zero);
            _vertexArray.BindAttribute(1, _vertexBuffer, VertexAttribPointerType.Float, 2, vertexSize, new IntPtr(3));
            _vertexArray.BindAttribute(2, _vertexBuffer, VertexAttribPointerType.Float, 3, vertexSize, new IntPtr(5));
            _vertexArray.BindAttribute(3, _vertexBuffer, VertexAttribPointerType.Float, 3, vertexSize, new IntPtr(8));
            _vertexArray.BindAttribute(4, _vertexBuffer, VertexAttribPointerType.Float, 3, vertexSize, new IntPtr(11));
        }

        private void ComputeBoundingSphere(List<Vertex> vertices)
        {
            _boundingSphere.Position = Vector3.Zero;
            _boundingSphere.Radius = 0.0f;

            if (vertices.Any())
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float minZ = float.MaxValue;

                float maxX = float.MinValue;
                float maxY = float.MinValue;
                float maxZ = float.MinValue;

                foreach (var vertex in vertices)
                {
                    minX = MathHelper.Min(minX, vertex.Position[0]);
                    minY = MathHelper.Min(minY, vertex.Position[1]);
                    minZ = MathHelper.Min(minZ, vertex.Position[2]);

                    maxX = MathHelper.Max(maxX, vertex.Position[0]);
                    maxY = MathHelper.Max(maxY, vertex.Position[1]);
                    maxZ = MathHelper.Max(maxZ, vertex.Position[2]);
                }

                _boundingSphere.Position = new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2.0f;

                _boundingSphere.Radius = vertices.Max(v =>
                    Vector3.Distance(new Vector3(v.Position[0], v.Position[1], v.Position[2]),
                        BoundingSphere.Position));
            }
        }

        public void Dispose()
        {
            _vertexArray.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}
