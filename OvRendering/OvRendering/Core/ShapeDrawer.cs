using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OvRendering.OvRendering.Geometry;
using OvRendering.OvRendering.Resources;
using OvRendering.OvRendering.Resources.Loaders;

namespace OvRendering.OvRendering.Core
{
    public class ShapeDrawer : IDisposable
    {
        private Shader _lineShader = null!;
        private Shader _gridShader = null!;
        private Mesh _lineMesh = null!;
        private Render _render = null!;


        public ShapeDrawer(Render render)
        {
            _render = render;
            List<Vertex> vertices = new List<Vertex>();
            vertices.Add(new Vertex());
            vertices.Add(new Vertex());

            _lineMesh = new Mesh(vertices, new List<uint>() { 0, 1 }, 0);
            string vertexShader = @"(
#version 430 core

            uniform vec3 start;
            uniform vec3 end;
            uniform mat4 viewProjection;

            void main()
            {
                vec3 position = gl_VertexID == 0 ? start : end;
                gl_Position = viewProjection * vec4(position, 1.0);
            }

            )";


            string fragmentShader = @"(
#version 430 core

            uniform vec3 color;

                out vec4 FRAGMENT_COLOR;

            void main()
            {
                FRAGMENT_COLOR = vec4(color, 1.0);
            }
            )";

            _lineShader = ShaderLoader.CreateFromSource(vertexShader, fragmentShader)!;
            vertexShader = @"(
#version 430 core

            uniform vec3 start;
            uniform vec3 end;
            uniform mat4 viewProjection;

                out vec3 fragPos;

            void main()
            {
                vec3 position = gl_VertexID == 0 ? start : end;
                fragPos = position;
                gl_Position = viewProjection * vec4(position, 1.0);
            }

            )";


            fragmentShader = @"(
#version 430 core

            uniform vec3 color;
            uniform vec3 viewPos;
            uniform float linear;
            uniform float quadratic;
            uniform float fadeThreshold;

                out vec4 FRAGMENT_COLOR;

                in vec3 fragPos;

            float AlphaFromAttenuation()
            {
                vec3 fakeViewPos = viewPos;
                fakeViewPos.y = 0;

                const float distanceToLight = max(max(length(viewPos - fragPos) - fadeThreshold, 0) - viewPos.y, 0);
                const float attenuation = (linear * distanceToLight + quadratic * (distanceToLight * distanceToLight));
                return 1.0 / attenuation;
            }

            void main()
            {
                FRAGMENT_COLOR = vec4(color, AlphaFromAttenuation());
            }
            )";

            _gridShader = ShaderLoader.CreateFromSource(vertexShader, fragmentShader)!;
        }

        public void SetViewProjection(Matrix4 viewProjection)
        {
            _lineShader.Bind();
            _lineShader.SetUniformMat4("viewProjection", viewProjection);
            _lineShader.Unbind();

            _gridShader.Bind();
            _gridShader.SetUniformMat4("viewProjection", viewProjection);
            _gridShader.Unbind();
        }

        public void DrawLine(Vector3 start, Vector3 end, Vector3 color, float lineWidth)
        {
            _lineShader.Bind();

            _lineShader.SetUniformVec3("start", start);
            _lineShader.SetUniformVec3("end", end);
            _lineShader.SetUniformVec3("color", color);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.LineWidth(lineWidth);
            _render.Draw(_lineMesh, PrimitiveType.Lines);
            GL.LineWidth(1);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _lineShader.Unbind();
        }

        public void DrawGrid(Vector3 viewPos, Vector3 color, int gridSize = 50, float linear = 0f, float quadratic = 0f,
            float fadeThreshold = 0, float lineWidth = 1)
        {
            _gridShader.Bind();
            _gridShader.SetUniformVec3("color", color);
            _gridShader.SetUniformVec3("viewPos", viewPos);
            _gridShader.SetUniformFloat("linear", linear);
            _gridShader.SetUniformFloat("quadratic", quadratic);
            _gridShader.SetUniformFloat("fadeThreshold", fadeThreshold);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.LineWidth(lineWidth);
            GL.Enable(EnableCap.Blend);
            for (int i = -gridSize; i < gridSize; ++i)
            {
                _gridShader.SetUniformVec3("start", new Vector3(-(float)gridSize + (float)MathHelper.Floor(viewPos.X), 0, i + (float)MathHelper.Floor(viewPos.Z)));
                _gridShader.SetUniformVec3("end", new Vector3(gridSize + (float)MathHelper.Floor(viewPos.X), 0, i + (float)MathHelper.Floor(viewPos.Z)));
                _render.Draw(_lineMesh, PrimitiveType.Lines);
                _gridShader.SetUniformVec3("start", new Vector3(i + (float)MathHelper.Floor(viewPos.X), 0, -(float)gridSize + (float)MathHelper.Floor(viewPos.Z)));
                _gridShader.SetUniformVec3("end", new Vector3(i + (float)MathHelper.Floor(viewPos.X), 0, gridSize + (float)MathHelper.Floor(viewPos.Z)));
                _render.Draw(_lineMesh, PrimitiveType.Lines);
            }


            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.LineWidth(1);
            GL.Disable(EnableCap.Blend);
            _gridShader.Unbind();
        }

        public void Dispose()
        {
            _lineShader.Dispose();
            _gridShader.Dispose();
            _lineMesh.Dispose();
        }
    }
}
