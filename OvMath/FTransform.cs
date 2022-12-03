using OpenTK.Mathematics;
using System.Reflection.Metadata;

namespace OvMath
{
    internal enum ENotification { TransformChanged, TransformDestroy }
    public class FTransform
    {
        private Vector3 _localPosition;
        private Quaternion _localRotation;
        private Vector3 _localScale;
        private Vector3 _worldPosition;
        private Quaternion _worldRotation;
        private Vector3 _worldScale;

        public Vector3 LocalPosition
        {
            get => _localPosition;
            set => GenerateMatrices(value, _localRotation, _localScale);
        }

        public Quaternion LocalRotation
        {
            get => _localRotation;
            set => GenerateMatrices(_localPosition, value, _localScale);
        }

        public Vector3 LocalScale
        {
            get => _localScale;
            set => GenerateMatrices(_localPosition, _localRotation, value);
        }

        public Vector3 WorldPosition
        {
            get => _worldPosition;
            set => GenerateMatrices(value, _worldRotation, _worldScale);
        }

        public Quaternion WorldRotation
        {
            get => _worldRotation;
            set => GenerateMatrices(_worldPosition, value, _worldScale);
        }

        public Vector3 WorldScale
        {
            get => _worldScale;
            set => GenerateMatrices(_worldPosition, _worldRotation, value);
        }

        public Matrix4 LocalMatrix { get; private set; }
        public Matrix4 WorldMatrix { get; private set; }

        public Vector3 WorldForward => _worldRotation * Vector3.UnitZ;
        public Vector3 WorldUp => _worldRotation * Vector3.UnitY;
        public Vector3 WorldRight => _worldRotation * Vector3.UnitX;
        public Vector3 LocalForward => _localRotation * Vector3.UnitZ;
        public Vector3 LocalUp => _localRotation * Vector3.UnitX;
        public Vector3 LocalRight => _localRotation * Vector3.UnitY;

        private FTransform? _parent;

        public FTransform? Parent
        {
            get => _parent;
            set => SetParent(value);
        }
        public bool HasParent => _parent != null;

        private event EventHandler<ENotification>? Notifier;

        public FTransform() : this(Vector3.Zero, Quaternion.Identity, Vector3.One)
        {
        }
        public FTransform(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            _parent = null;
            GenerateMatrices(localPosition, localRotation, localScale);
        }

        public void SetParent(FTransform? parent)
        {
            if (parent != null)
            {
                _parent = parent;
                parent.Notifier += this.NotificationHandler;
                UpdateWorldMatrix();
            }
            else
            {
                RemoveParent();
            }
        }
        public bool RemoveParent()
        {
            if (_parent != null)
            {
                _parent.Notifier -= this.NotificationHandler;
                _parent = null;
                UpdateWorldMatrix();
                return true;
            }
            return false;
        }

        public void GenerateMatrices(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // 缩放 -> 旋转 -> 平移
            LocalMatrix = Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(Quaternion.Normalize(rotation)) *
                          Matrix4.CreateTranslation(position);
            _localPosition = position;
            _localRotation = rotation;
            _localScale = scale;
            UpdateWorldMatrix();

        }
        public void UpdateWorldMatrix()
        {
            WorldMatrix = HasParent ? _parent!.WorldMatrix * LocalMatrix : LocalMatrix;
            PreDecomposeWorldMatrix();
            Notifier?.Invoke(this, ENotification.TransformChanged);
        }


        public void TranslateLocal(Vector3 translation)
        {
            LocalPosition += translation;
        }
        public void RotateLocal(Quaternion rotation)
        {
            LocalRotation *= rotation;
        }
        public void ScaleLocal(Vector3 scale)
        {
            LocalScale *= scale;
        }

        // 由父节订阅
        private void NotificationHandler(object? sender, ENotification notification)
        {
            switch (notification)
            {
                case ENotification.TransformChanged:
                    UpdateWorldMatrix();
                    break;
                case ENotification.TransformDestroy:
                    GenerateMatrices(_worldPosition, _worldRotation, _worldScale);
                    _parent!.Notifier -= NotificationHandler;
                    _parent = null;
                    UpdateWorldMatrix();
                    break;
            }
        }
        private void PreDecomposeWorldMatrix()
        {
            _worldPosition.X = WorldMatrix[3, 0];
            _worldPosition.Y = WorldMatrix[3, 1];
            _worldPosition.Z = WorldMatrix[3, 2];

            Vector3[] columns = new[]
            {
                new Vector3(WorldMatrix[0, 0], WorldMatrix[0, 1], WorldMatrix[0, 2]),
                new Vector3(WorldMatrix[1, 0], WorldMatrix[1, 1], WorldMatrix[1, 2]),
                new Vector3(WorldMatrix[2, 0], WorldMatrix[2, 1], WorldMatrix[2, 2]),
            };
            _worldScale.X = columns[0].Length;
            _worldScale.Y = columns[1].Length;
            _worldScale.Z = columns[2].Length;

            if (_worldScale.X != 0)
            {
                columns[0] /= _worldScale.X;
            }

            if (_worldScale.Y != 0)
            {
                columns[1] /= _worldScale.Y;
            }

            if (_worldScale.Z != 0)
            {
                columns[2] /= _worldScale.Z;
            }

            Matrix3 rotationMatrix = new Matrix3(new Vector3(columns[0].X, columns[0].X, columns[0].X),
                new Vector3(columns[1].Y, columns[1].Y, columns[1].Y),
                new Vector3(columns[2].Z, columns[2].Z, columns[2].Z));

            _worldRotation = Quaternion.FromMatrix(rotationMatrix);
        }
        private void PreDecomposeLocalMatrix()
        {
            _localPosition.X = LocalMatrix[0, 3];
            _localPosition.Y = LocalMatrix[1, 3];
            _localPosition.Z = LocalMatrix[2, 3];

            Vector3[] columns = new[]
            {
                new Vector3(LocalMatrix[0, 0], LocalMatrix[1, 0], LocalMatrix[2, 0]),
                new Vector3(LocalMatrix[0, 1], LocalMatrix[1, 1], LocalMatrix[2, 1]),
                new Vector3(LocalMatrix[0, 2], LocalMatrix[1, 2], LocalMatrix[2, 2]),
            };
            _localScale.X = columns[0].Length;
            _localScale.Y = columns[1].Length;
            _localScale.Z = columns[2].Length;

            if (_localScale.X != 0)
            {
                columns[0] /= _localScale.X;
            }

            if (_localScale.Y != 0)
            {
                columns[1] /= _localScale.Y;
            }

            if (_localScale.Z != 0)
            {
                columns[2] /= _localScale.Z;
            }

            Matrix3 rotationMatrix = new Matrix3(new Vector3(columns[0].X, columns[1].X, columns[2].X),
                new Vector3(columns[0].Y, columns[1].Y, columns[2].Y),
                new Vector3(columns[0].Z, columns[1].Z, columns[2].Z));

            _localRotation = Quaternion.FromMatrix(rotationMatrix);
        }

        private void TRS()
        {

        }
    }
}