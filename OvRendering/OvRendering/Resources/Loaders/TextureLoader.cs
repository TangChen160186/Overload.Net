using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace OvRendering.OvRendering.Resources.Loaders
{
    public class TextureLoader
    {
        private TextureLoader() { }
        public static Texture2D? Create(string filePath, TextureMagFilter textureMagFilter, TextureMinFilter textureMinFilter, bool generateMipmap)
        {
            GL.GenTextures(1, out uint textureId);
            //Load the image
            Image<Rgba32> image = Image.Load<Rgba32>(filePath);
            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            //This will correct that, making the texture display properly.
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            //Use the CopyPixelDataTo function from ImageSharp to copy all of the bytes from the image into an array that we can give to OpenGL.
            var pixels = new byte[4 * image.Width * image.Height];
            image.CopyPixelDataTo(pixels);

            if (pixels.Length > 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.Byte, pixels);
                if (generateMipmap)
                {
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMagFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                return new Texture2D(textureId, (uint)image.Width, (uint)image.Height, (uint)image.PixelType.BitsPerPixel, filePath, textureMagFilter, textureMinFilter, generateMipmap);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                return null;
            }
        }

        public static Texture2D CreateColor(uint data, TextureMagFilter textureMagFilter, TextureMinFilter textureMinFilter, bool generateMipmap)
        {
            GL.GenTextures(1, out uint textureId);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 1, 1, 0, PixelFormat.Rgba, PixelType.Byte, ref data);
            if (generateMipmap)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMagFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return new Texture2D(textureId, 1, 1, 32, "", textureMagFilter, textureMinFilter, generateMipmap);
        }

        public static Texture2D CreateFromMemory(byte[] pixels, uint width, uint height, TextureMagFilter textureMagFilter,
            TextureMinFilter textureMinFilter, bool generateMipmap)
        {
            GL.GenTextures(1, out uint textureId);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 1, 1, 0, PixelFormat.Rgba, PixelType.Byte, pixels);
            if (generateMipmap)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMagFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return new Texture2D(textureId, width, height, 32, "", textureMagFilter, textureMinFilter, generateMipmap);
        }

        public static void Reload(Texture2D texture, string filePath, TextureMagFilter textureMagFilter,
            TextureMinFilter textureMinFilter, bool generateMipmap)
        {
            var newTexture = Create(filePath, textureMagFilter, textureMinFilter, generateMipmap);
            if (newTexture != null)
            {
                GL.DeleteTexture(texture.Id);

                texture.Id = newTexture.Id;
                texture.Width = newTexture.Width;
                texture.Height = newTexture.Height;
                texture.BitsPerPixel = newTexture.BitsPerPixel;
                texture.TextureMagFilter = newTexture.TextureMagFilter;
                texture.TextureMinFilter = newTexture.TextureMinFilter;
                texture.IsMinMapped = newTexture.IsMinMapped;
            }
        }

        public static bool Destroy(ref Texture2D? texture)
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
                return true;
            }

            return false;
        }
    }
}
