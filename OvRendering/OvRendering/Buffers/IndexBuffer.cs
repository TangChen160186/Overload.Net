using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OvRendering.OvRendering.Buffers
{
    public class IndexBuffer : IDisposable
    {
        private readonly uint _bufferId;
        public uint BufferId => _bufferId;
        public IndexBuffer(uint[] data)
        {
            GL.GenBuffers(1, out _bufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bufferId);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        private void ReleaseUnmanagedResources()
        {
            GL.DeleteBuffer(_bufferId);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~IndexBuffer()
        {
            ReleaseUnmanagedResources();
        }
    }
}
