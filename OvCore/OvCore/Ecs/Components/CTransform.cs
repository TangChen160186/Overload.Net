using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OvMath;

namespace OvCore.OvCore.Ecs.Components
{
    [DataContract]
    public class CTransform : AComponent
    {
        public FTransform Transform { get; private set; }
        public override string Name => nameof(CTransform);
        [DataMember]
        public Vector3 LocalPosition
        {
            get => Transform.LocalPosition;
            set => Transform.LocalPosition = value;
        }
        [DataMember]
        public Quaternion LocalRotation
        {
            get => Transform.LocalRotation;
            set => Transform.LocalRotation = value;
        }
        [DataMember]
        public Vector3 LocalScale
        {
            get => Transform.LocalScale;
            set => Transform.LocalScale = value;
        }
        public Vector3 WorldPosition
        {
            get => Transform.WorldPosition;
            set => Transform.WorldPosition = value;
        }
        public Quaternion WorldRotation
        {
            get => Transform.WorldRotation;
            set => Transform.WorldRotation = value;
        }
        public Vector3 WorldScale
        {
            get => Transform.WorldScale;
            set => Transform.WorldScale = value;
        }
        public Matrix4 LocalMatrix => Transform.LocalMatrix;
        public Matrix4 WorldMatrix => Transform.WorldMatrix;
        public Vector3 WorldForward => Transform.WorldForward;
        public Vector3 WorldUp => Transform.WorldUp;
        public Vector3 WorldRight => Transform.WorldRight;
        public Vector3 LocalForward => Transform.LocalForward;
        public Vector3 LocalUp => Transform.LocalUp;
        public Vector3 LocalRight => Transform.LocalRight;
        public CTransform(Actor actor) : this(actor, Vector3.Zero, Quaternion.Identity, Vector3.One) { }
        public CTransform(Actor actor, Vector3 localPosition, Quaternion localRotation, Vector3 localScale) : base(actor)
        {
            Transform = new FTransform();
            Transform.GenerateMatrices(localPosition, localRotation, localScale);
        }

        public void SetParent(CTransform parent) => Transform.SetParent(parent.Transform);
        public void RemoveParent(CTransform parent) => Transform.RemoveParent();
        public bool HasParent() => Transform.HasParent;
        public void TranslateLocal(Vector3 translation) => Transform.TranslateLocal(translation);
        public void RotateLocal(Quaternion rotation) => Transform.RotateLocal(rotation);
        public void ScaleLocal(Vector3 scale) => Transform.ScaleLocal(scale);

        internal override void Init(Actor actor)
        {
            base.Init(actor);
            var light = new CDirectionalLight(new Actor());
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Transform = new FTransform();
        }
    }
}
