using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OvMath;
using OvRendering.OvRendering.Data;
using OvRendering.OvRendering.Geometry;
using OvRendering.OvRendering.LowRender;
using OvRendering.OvRendering.Resources;
using OvRendering.OvRendering.Settings;

namespace OvRendering.OvRendering.Core
{
    #region FrameInfo

    public struct FrameInfo
    {
        /// <summary>
        /// 渲染次数
        /// </summary>
        public uint BatchCount;
        /// <summary>
        /// 实例数量
        /// </summary>
        public uint InstanceCount;
        /// <summary>
        /// 多边形数量
        /// </summary>
        public uint PolyCount;
    };
    #endregion
    public class Render
    {
        private FrameInfo _frameInfo;
        public FrameInfo FrameInfo => _frameInfo;

        public byte State { get; set; }
        public void Clear(Camera camera, ClearBufferMask clearBufferMask)
        {
            float[] previousClearColor = new float[4];
            GL.GetFloat(GetPName.ColorClearValue, previousClearColor);
            var cameraClearColor = camera.ClearColor;

            GL.ClearColor(cameraClearColor.X, cameraClearColor.Y, cameraClearColor.Z, 1.0f);

            GL.Clear(clearBufferMask);
            GL.ClearColor(previousClearColor[0], previousClearColor[1], previousClearColor[2], previousClearColor[3]);
        }
        public void ClearFrameInfo()
        {
            _frameInfo.BatchCount = 0;
            _frameInfo.InstanceCount = 0;
            _frameInfo.PolyCount = 0;
        }
        public void Draw(IMesh mesh, PrimitiveType primitiveType = PrimitiveType.Triangles, uint instances = 1)
        {
            if (instances > 0)
            {
                ++_frameInfo.BatchCount;
                _frameInfo.InstanceCount += instances;
                _frameInfo.PolyCount += instances * (mesh.IndexCount / 3);
                mesh.Bind();
                if (mesh.IndexCount > 0)
                {
                    if (instances == 1)
                    {
                        GL.DrawElements(primitiveType, (int)mesh.IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                    else
                    {
                        GL.DrawElementsInstanced(primitiveType, (int)mesh.IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero, (int)instances);
                    }
                }
                else
                {
                    if (instances == 1)
                    {
                        GL.DrawArrays(primitiveType, 0, (int)mesh.VertexCount);
                    }
                    else
                    {
                        GL.DrawArraysInstanced(primitiveType, 0, (int)mesh.VertexCount, (int)instances);
                    }
                }
                mesh.Unbind();
            }
        }
        public List<Mesh> GetMeshesInFrustum(Model model, BoundingSphere modelBoundingSphere, FTransform modelTransform, Frustum frustum, ECullingOptions cullingOptions)
        {
            // 有点绕
            List<Mesh> result = new List<Mesh>();
            bool frustumPerModel =
                (cullingOptions & ECullingOptions.FrustumPerModel) == ECullingOptions.FrustumPerModel;
            // 不是剔除整个模型 或者 模型与视锥体有相交的部分
            if (!frustumPerModel || frustum.BoundingSphereInFrustum(modelBoundingSphere, modelTransform))
            {
                bool frustumPerMesh =
                    (cullingOptions & ECullingOptions.FrustumPerMesh) == ECullingOptions.FrustumPerMesh;

                var meshes = model.Meshes;
                foreach (var mesh in meshes)
                {
                    if (meshes.Count == 1 || !frustumPerMesh ||
                        frustum.BoundingSphereInFrustum(mesh.BoundingSphere, modelTransform))
                    {
                        result.Add(mesh);
                    }
                }
                return result;
            }

            return result;
        }
        public byte FetchGlState()
        {
            byte result = 0;
            bool[] cMask = new bool[4];
            GL.GetBoolean(GetPName.ColorWritemask, cMask);
            if (GL.GetBoolean(GetPName.DepthWritemask)) result |= 0b0000_0001;
            if (cMask[0]) result |= 0b0000_0010;
            if (GL.IsEnabled(EnableCap.Blend)) result |= 0b0000_0100;
            if (GL.IsEnabled(EnableCap.CullFace)) result |= 0b0000_1000;
            if (GL.IsEnabled(EnableCap.DepthTest)) result |= 0b0001_0000;
            var cullFace = (CullFaceMode)GL.GetInteger(GetPName.CullFace);
            switch (cullFace)
            {
                case CullFaceMode.Front:
                    result |= 0b0010_0000;
                    break;
                case CullFaceMode.Back:
                    result |= 0b0100_0000;
                    break;
                case CullFaceMode.FrontAndBack:
                    result |= 0b0110_0000;
                    break;
            }

            return result;
        }
        public void ApplyStateMask(byte mask)
        {
            if (State != mask)
            {
                if ((mask & 0x01) != (State & 0x01)) GL.DepthMask((mask & 0x01) == 0x01);
                if ((mask & 0x02) != (State & 0x02))
                {
                    var flag = (mask & 0x02) == 0x02;
                    GL.ColorMask(flag, flag, flag, flag);
                }
                if ((mask & 0x04) != (State & 0x04))
                {
                    var flag = (mask & 0x04) == 0x04;
                    if (flag)
                    {
                        GL.Enable(EnableCap.Blend);
                    }
                    else
                    {
                        GL.Disable(EnableCap.Blend);
                    }
                }
                if ((mask & 0x08) != (State & 0x08))
                {
                    var flag = (mask & 0x08) == 0x08;
                    if (flag)
                    {
                        GL.Enable(EnableCap.CullFace);
                        if ((mask & 0x20) != (State & 0x20) || (mask & 0x40) != (State & 0x40))
                        {
                            switch (mask & 0x20)
                            {
                                case 0x20 when (mask & 0x40) == 0x40:
                                    GL.CullFace(CullFaceMode.FrontAndBack);
                                    break;
                                case 0x20:
                                    GL.CullFace(CullFaceMode.Back);
                                    break;
                                default:
                                    GL.CullFace(CullFaceMode.Front);
                                    break;
                            }
                        }

                    }
                    else
                    {
                        GL.Disable(EnableCap.CullFace);
                    }

                }
                if ((mask & 0x10) != (State & 0x010))
                {
                    var flag = (mask & 0x010) == 0x010;
                    if (flag)
                    {
                        GL.Enable(EnableCap.DepthTest);
                    }
                    else
                    {
                        GL.Disable(EnableCap.DepthTest);
                    }
                }
                State = mask;
            }
        }

    }
}
