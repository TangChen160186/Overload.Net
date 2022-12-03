using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OvMath;
using OvRendering.OvRendering.LowRender;
using OvRendering.OvRendering.Settings;

namespace OvCore.OvCore.Ecs.Components
{
    [DataContract]
    public class CCamera:AComponent
    {
        public override string Name => nameof(CCamera);
        public Camera Camera { get; private set; }
        [DataMember]
        public float Fov
        {
            get => Camera.Fov;
            set => Camera.Fov = value;
        }
        [DataMember]
        public float Size
        {
            get=> Camera.Size;
            set => Camera.Size = value;
        }
        [DataMember]
        public float Near
        {
            get=>Camera.Near;
            set => Camera.Near = value;
        }
        [DataMember]
        public float Far
        {
            get => Camera.Far; 
            set => Camera.Far = value;
        }
        [DataMember]
        public Vector3 ClearColor
        {
            get=>Camera.ClearColor; 
            set => Camera.ClearColor = value;
        }
        [DataMember]
        public bool FrustumGeometryCulling
        {
            get=>Camera.FrustumGeometryCulling; 
            set => Camera.FrustumGeometryCulling = value;
        }
        [DataMember]
        public bool FrustumLightCulling
        {
            get => Camera.FrustumLightCulling; 
            set => Camera.FrustumLightCulling = value;
        }
        [DataMember]
        public EProjectionMode ProjectionMode
        {
            get=>Camera.ProjectionMode; 
            set => Camera.ProjectionMode = value;
        }

        public CCamera(Actor actor) : base(actor)
        {
            Camera = new Camera();
            ClearColor = new Vector3(0.1921569f, 0.3019608f, 0.4745098f);
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Camera = new Camera();
        }


    }
}
