using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OvDebug;

namespace OvRendering.OvRendering.Resources
{
    public class Shader : IDisposable
    {
        public uint Id { get; internal set; }
        public string Path { get; }

        private readonly List<UniformInfo> _uniforms = new();
        // 缓存
        private readonly Dictionary<string, int> _uniformLocationCache = new();

        internal Shader(string path, uint id)
        {
            Id = id;
            Path = path;
            QueryUniforms();
        }

        public void Bind()
        {
            GL.UseProgram(Id);
        }
        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public void SetUniformInt(string name, int value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }
        public void SetUniformFloat(string name, float value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }
        public void SetUniformVec2(string name, Vector2 vec2)
        {
            GL.Uniform2(GetUniformLocation(name), vec2);
        }
        public void SetUniformVec3(string name, Vector3 vec3)
        {
            GL.Uniform3(GetUniformLocation(name), vec3);
        }
        public void SetUniformVec4(string name, Vector4 vec4)
        {
            GL.Uniform4(GetUniformLocation(name), vec4);
        }
        public void SetUniformMat4(string name, Matrix4 mat4)
        {
            GL.UniformMatrix4(GetUniformLocation(name), true, ref mat4);
        }

        public int GetUniformInt(string name)
        {
            GL.GetUniform(Id, GetUniformLocation(name), out int value);
            return value;
        }

        public float GetUniformFloat(string name)
        {
            GL.GetUniform(Id, GetUniformLocation(name), out float value);
            return value;
        }

        public Vector2 GetUniformVec2(string name)
        {
            float[] value = new float[2];
            GL.GetUniform(Id, GetUniformLocation(name), value);
            return new Vector2(value[0], value[1]);
        }

        public Vector3 GetUniformVec3(string name)
        {
            float[] value = new float[3];
            GL.GetUniform(Id, GetUniformLocation(name), value);
            return new Vector3(value[0], value[1], value[2]);
        }
        public Vector4 GetUniformVec4(string name)
        {
            float[] value = new float[4];
            GL.GetUniform(Id, GetUniformLocation(name), value);
            return new Vector4(value[0], value[1], value[2], value[3]);
        }

        public Matrix4 GetUniformMat4(string name)
        {
            float[] value = new float[16];
            GL.GetUniform(Id, GetUniformLocation(name), value);
            return new Matrix4(value[0], value[1], value[2], value[3], value[4], value[5], value[6], value[7], value[8],
                value[9], value[10], value[11], value[12], value[13], value[14], value[15]);
        }

        public bool IsEngineUboMember(string uniformName)
        {
            return uniformName.Contains("ubo_");
        }
        private int GetUniformLocation(string name)
        {
            if (_uniformLocationCache.ContainsKey(name))
            {
                return _uniformLocationCache[name];
            }
            var location = GL.GetUniformLocation(Id, name);
            if (location == -1)
                OvLogger.Default.Warn("Uniform: '" + name + "' doesn't exist");
            _uniformLocationCache.Add(name, location);

            return location;
        }

        public void QueryUniforms()
        {
            _uniforms.Clear();
            GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out int numActiveUniforms);
            for (var i = 0; i < numActiveUniforms; i++)
            {
                GL.GetActiveUniform((int)Id, i, 256, out _, out _, out ActiveUniformType uniformType, out string name);

                if (!IsEngineUboMember(name))
                {
                    bool hasValue = false;
                    object? defaultValue = null;
                    switch (uniformType)
                    {
                        case ActiveUniformType.Bool:
                            hasValue = true;
                            defaultValue = GetUniformInt(name);
                            break;
                        case ActiveUniformType.Int:
                            hasValue = true;
                            defaultValue = GetUniformInt(name);
                            break;
                        case ActiveUniformType.Float:
                            hasValue = true;
                            defaultValue = GetUniformFloat(name);
                            break;
                        case ActiveUniformType.FloatVec2:
                            hasValue = true;
                            defaultValue = GetUniformVec2(name);
                            break;
                        case ActiveUniformType.FloatVec3:
                            hasValue = true;
                            defaultValue = GetUniformVec3(name);
                            break;
                        case ActiveUniformType.FloatVec4:
                            hasValue = true;
                            defaultValue = GetUniformVec4(name);
                            break;
                        //case ActiveUniformType.FloatMat4:
                        //    break;
                        case ActiveUniformType.Sampler2D:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (hasValue)
                    {
                        _uniforms.Add(new UniformInfo()
                        {
                            DefaultValue = defaultValue,
                            Location = GetUniformLocation(name),
                            Name = name,
                            UniformType = uniformType
                        });
                    }

                }
            }

        }

        public UniformInfo? GetUniformInfo(string name)
        {
            var index = _uniforms.FindIndex(u => u.Name == name);
            if (index != -1)
            {
                return _uniforms[index];
            }

            return null;
        }
        private void ReleaseUnmanagedResources()
        {
            GL.DeleteShader(Id);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            ReleaseUnmanagedResources();
        }
    }
}
