using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OvRendering.OvRendering.Buffers
{
    public class FrameBuffer : IDisposable
    {
        private readonly uint _bufferId;
        public uint BufferId => _bufferId;

        private readonly uint _renderTexture;
        public uint RenderTexture => _renderTexture;

        private readonly uint _depthStencilBuffer;
        public uint DepthStencilBuffer => _depthStencilBuffer;
        public FrameBuffer(int width = 0, int height = 0)
        {
            GL.GenFramebuffers(1, out _bufferId);
            GL.GenTextures(1, out _renderTexture);
            GL.GenRenderbuffers(1, out _depthStencilBuffer);

            GL.BindTexture(TextureTarget.Texture2D, _renderTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            Bind();
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _renderTexture, 0);
            Unbind();

            Resize(width, height);
        }


        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _bufferId);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        public void Resize(int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, _renderTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthStencilBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            Bind();
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _depthStencilBuffer);
            Unbind();
        }

        private void ReleaseUnmanagedResources()
        {
            GL.DeleteBuffer(_bufferId);
            GL.DeleteTexture(_renderTexture);
            GL.DeleteRenderbuffer(_depthStencilBuffer);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~FrameBuffer()
        {
            ReleaseUnmanagedResources();
        }
    }
}
