using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Core.Native;
using OpenTK.Graphics.OpenGL4;

namespace OvRendering.OvRendering.Buffers
{
    public class VertexBuffer<T> : IDisposable
        where T : struct
    {
        private readonly uint _bufferId;
        public uint BufferId => _bufferId;

        public VertexBuffer(T[] data)
        {
            GL.GenBuffers(1, out _bufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _bufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Marshal.SizeOf<T>() * data.Length), data, BufferUsageHint.StaticDraw);
        }


        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _bufferId);
        }
        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

        ~VertexBuffer()
        {
            ReleaseUnmanagedResources();
        }
    }
}
