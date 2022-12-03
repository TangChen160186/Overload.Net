using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OvRendering.OvRendering.Buffers
{
    public class VertexArray : IDisposable
    {
        public uint BufferId => _bufferId;

        private readonly uint _bufferId;
        public VertexArray()
        {
            GL.GenVertexArrays(1, out _bufferId);
            GL.BindVertexArray(_bufferId);
        }

        public void BindAttribute<T>(uint attribute, VertexBuffer<T>? vertexBuffer, VertexAttribPointerType type, int count, int stride, IntPtr offset)
            where T : struct
        {
            Bind();
            vertexBuffer.Bind();
            GL.EnableVertexAttribArray(attribute);
            GL.VertexAttribPointer(attribute, count, type, false, stride, offset);
        }

        public void Bind()
        {
            GL.BindVertexArray(_bufferId);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }


        private void ReleaseUnmanagedResources()
        {
            GL.DeleteVertexArray(_bufferId);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~VertexArray()
        {
            ReleaseUnmanagedResources();
        }
    }
}
