using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OvRendering.OvRendering.Resources
{
    public class Texture2D : IDisposable
    {
        public uint Id { get; internal set; }
        public uint Width { get; internal set; }
        public uint Height { get; internal set; }
        public uint BitsPerPixel { get; internal set; }
        public string Path { get; internal set; }
        public bool IsMinMapped { get; internal set; }

        public TextureMagFilter TextureMagFilter { get; internal set; }
        public TextureMinFilter TextureMinFilter { get; internal set; }
        public void Bind(uint slot = 0)
        {
            GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + slot));
            GL.BindTexture(TextureTarget.Texture2D, Id);
        }
        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        internal Texture2D(uint id, uint width, uint height, uint bitsPerPixel, string path, TextureMagFilter textureMagFilter, TextureMinFilter textureMinFilter, bool isMinMapped)
        {
            Id = id;
            Width = width;
            Height = height;
            BitsPerPixel = bitsPerPixel;
            Path = path;
            TextureMagFilter = textureMagFilter;
            TextureMinFilter = textureMinFilter;
            IsMinMapped = isMinMapped;
        }

        private void ReleaseUnmanagedResources()
        {
            GL.DeleteTexture(Id);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Texture2D()
        {
            ReleaseUnmanagedResources();
        }
    }
}
