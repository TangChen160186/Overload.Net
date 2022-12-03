using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OvRendering.OvRendering.Resources;

namespace OvRendering.OvRendering.Buffers
{
    public class UniformBuffer : IDisposable
    {
        private readonly uint _bufferId;

        public uint BufferId => _bufferId;

        public UniformBuffer(int size, uint bindingPoint = 0, uint offset = 0, BufferUsageHint bufferUsageHint = BufferUsageHint.DynamicDraw)
        {
            GL.GenBuffers(1, out _bufferId);
            GL.BindBuffer(BufferTarget.UniformBuffer, _bufferId);
            GL.BufferData(BufferTarget.UniformBuffer, size, IntPtr.Zero, bufferUsageHint);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, bindingPoint, _bufferId, new IntPtr(offset), size);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _bufferId);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void SetSubData<T>(T data, int offsetInOut)
            where T : struct
        {
            Bind();
            GL.BufferSubData(BufferTarget.UniformBuffer, new IntPtr(offsetInOut), Marshal.SizeOf<T>(), ref data);
            Unbind();
        }


        public static void BindBlockToShader(Shader shader, uint uniformBlockLocation = 0, uint bindingPoint = 0)
        {
            GL.UniformBlockBinding(shader.Id, uniformBlockLocation, bindingPoint);
        }

        public static void BindBlockToShader(Shader shader, string name, uint bindingPoint = 0)
        {
            GL.UniformBlockBinding(shader.Id, GetBlockLocation(shader, name), bindingPoint);
        }

        public static uint GetBlockLocation(Shader shader, string name)
        {
            return (uint)GL.GetUniformLocation(shader.Id, name);
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

        ~UniformBuffer()
        {
            ReleaseUnmanagedResources();
        }
    }
}
