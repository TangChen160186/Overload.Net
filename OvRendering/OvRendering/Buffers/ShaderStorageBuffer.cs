using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OvRendering.OvRendering.Buffers
{
    public class ShaderStorageBuffer : IDisposable
    {
        private readonly uint _bufferId;
        public uint BufferId => _bufferId;

        private uint _bindingPoint = 0;
        public ShaderStorageBuffer(BufferUsageHint bufferUsageHint)
        {
            GL.GenBuffers(1, out _bufferId);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufferId);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, 0, IntPtr.Zero, bufferUsageHint);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _bufferId);
        }

        public void Bind(uint bindingPoint)
        {
            _bindingPoint = bindingPoint;
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, bindingPoint, _bufferId);
        }

        public void Unbind()
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, _bindingPoint, 0);
        }

        public void SendBlocks<T>(T data, int size)
            where T : struct
        {
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufferId);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, size, ref data, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
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

        ~ShaderStorageBuffer()
        {
            ReleaseUnmanagedResources();
        }
    }
}
