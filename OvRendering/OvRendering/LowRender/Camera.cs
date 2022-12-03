using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OvRendering.OvRendering.Data;
using OvRendering.OvRendering.Settings;

namespace OvRendering.OvRendering.LowRender
{
    public class Camera
    {
        public Frustum Frustum { get; } = new();
        public Matrix4 ViewMatrix { get; private set; }
        public Matrix4 ProjectionMatrix { get; private set; }
        public EProjectionMode ProjectionMode { get;  set; }

        public float Fov { get;  set; }
        public float Size { get;  set; }
        public float Near { get;  set; }
        public float Far { get;  set; }

        public Vector3 ClearColor { get;  set; }
        public bool FrustumGeometryCulling { get;  set; }
        public bool FrustumLightCulling { get;  set; }

        public Camera()
        {
            ProjectionMode = EProjectionMode.Perspective;
            Fov = 45;
            Size = 5;
            Near = 0.1f;
            Far = 100;
            ClearColor = new Vector3(0, 0, 0);
            FrustumGeometryCulling = false;
            FrustumLightCulling = false;
        }

        public void CacheMatrices(int windowWidth, int windowHeight, Vector3 position, Quaternion rotation)
        {
            CacheProjectionMatrix(windowWidth, windowHeight);
            CacheViewMatrix(position, rotation);
            CacheFrustum(ViewMatrix, ProjectionMatrix);
        }

        public void CacheProjectionMatrix(int windowWidth, int windowHeight)
        {
            ProjectionMatrix = CalculateProjectionMatrix(windowWidth, windowHeight);
        }

        public void CacheViewMatrix(Vector3 position, Quaternion rotation)
        {
            ViewMatrix = CalculateViewMatrix(position, rotation);
        }

        public void CacheFrustum(Matrix4 view, Matrix4 projection)
        {
            Frustum.CalculateFrustum(projection * view);
        }

        private Matrix4 CalculateProjectionMatrix(int windowWidth, int windowHeight)
        {
            return ProjectionMode switch
            {
                EProjectionMode.Orthographic => Matrix4.CreateOrthographic(windowWidth, windowHeight, Near, Far),
                EProjectionMode.Perspective => Matrix4.CreatePerspectiveFieldOfView(Fov,
                    windowWidth / (float)windowHeight, Near, Far),
                _ => Matrix4.Identity
            };
        }

        private Matrix4 CalculateViewMatrix(Vector3 position, Quaternion rotation)
        {
            var forward = rotation * Vector3.UnitZ;
            return Matrix4.LookAt(position,
                new Vector3(position.X + forward.X, position.Y + forward.Y, position.Z + forward.Z), Vector3.UnitY);
            ;
        }
    }

}
