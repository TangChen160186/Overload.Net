using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OvDebug;

namespace OvRendering.OvRendering.Resources.Loaders
{
    public class ShaderLoader
    {
        private ShaderLoader() { }

        public static Shader? Create(string filePath)
        {
            _FILE_TRACE = filePath;
            var (vs, fs) = ParseShader(filePath);
            var program = CreateProgram(vs, fs);
            if (program != 0)
            {
                return new Shader(filePath, (uint)program);
            }

            return null;
        }

        public static Shader? CreateFromSource(string vertexShader, string fragmentShader)
        {
            var program = CreateProgram(vertexShader, fragmentShader);
            if (program != 0)
            {
                return new Shader("", (uint)program);
            }

            return null;
        }

        public static void Recompile(Shader shader, string filePath)
        {
            _FILE_TRACE = filePath;
            var (vs, fs) = ParseShader(filePath);
            var program = CreateProgram(vs, fs);
            if (program != 0)
            {
                GL.DeleteShader(shader.Id);
                shader.Id = (uint)program;
                shader.QueryUniforms();
                OvLogger.Default.Info("[COMPILE] \"" + _FILE_TRACE + "\": Success");
            }
            OvLogger.Default.Error("[COMPILE] \"" + _FILE_TRACE + "\": Failed! Previous shader version keep");
        }

        public static bool Destroy(ref Shader? shader)
        {
            if (shader != null)
            {
                shader.Dispose();
                shader = null;
                return true;
            }
            return false;
        }

        private static (string, string) ParseShader(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var index = -1;
            var res = new string[2];
            foreach (var line in lines)
            {
                if (line.Contains("#shader"))
                {
                    if (line.Contains("vertex")) index = 0;
                    else if (line.Contains("fragment")) index = 1;
                }
                else if (index != -1)
                {
                    res[index] += line;
                    res[index] += Environment.NewLine;
                }
            }
            return (res[0], res[1]);
        }

        private static int CreateProgram(string vertexShader, string fragmentShader)
        {
            var program = GL.CreateProgram();
            var vs = CompileShader(ShaderType.VertexShader, vertexShader);
            var fs = CompileShader(ShaderType.FragmentShader, fragmentShader);
            if (vs == 0 || fs == 0)
                return 0;
            GL.AttachShader(program, vs);
            GL.AttachShader(program, fs);
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var status);
            if (status == 0)
            {
                GL.GetProgramInfoLog(program, out var errorInfo);
                OvLogger.Default.Error("[LINK] + \"" + _FILE_TRACE + "\":" + Environment.NewLine + errorInfo);
                GL.DeleteProgram(program);
                return 0;
            }
            GL.ValidateProgram(program);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
            return program;
        }

        private static int CompileShader(ShaderType type, string source)
        {
            var id = GL.CreateShader(type);
            GL.ShaderSource(id, source);
            GL.CompileShader(id);

            GL.GetShader(id, ShaderParameter.CompileStatus, out var status);
            if (status == 0)
            {
                GL.GetShaderInfoLog(id, out var errorInfo);
                var shaderTypeString = type == ShaderType.VertexShader ? "VERTEX SHADER" : "FRAGMENT SHADER";
                var errorHeader = "[" + shaderTypeString + "] \"";
                OvLogger.Default.Error(errorHeader + _FILE_TRACE + "\":" + Environment.NewLine + errorInfo);
                GL.DeleteShader(id);
                return 0;
            }

            return id;
        }

        private static string _FILE_TRACE = "";
    }
}
