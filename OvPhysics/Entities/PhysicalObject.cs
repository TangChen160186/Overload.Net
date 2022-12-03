using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using BulletSharp.SoftBody;
using Microsoft.VisualBasic;
using OpenTK.Mathematics;
using OvMath;
using OvPhysics.Settings;
using Conversion = OvPhysics.Tools.Conversion;

namespace OvPhysics.Entities
{
    /// <summary>
    /// 检测模式
    /// </summary>
    public enum ECollisionDetectionMode
    {
        Discrete,
        Continuous,
    }

    /// <summary>
    /// RigidBody状态
    /// </summary>
    public enum EActivationState
    {
        None = 0, // Undefined 
        Active = 1,//ActiveTag，处于这状态的物体会被模拟
        Sleeping = 2, // IslandSleeping
        LookingForSleep = 3,// WantsDeactivation
        AlwaysActive = 4,// DisableDeactivation
        AlwaysSleeping = 5// DisableSimulation 处于这状态的物体不会被模拟
    }
    public abstract class PhysicalObject : IDisposable
    {
        public EventHandler<PhysicalObject>? CollisionStartEvent;
        public EventHandler<PhysicalObject>? CollisionStayEvent;
        public EventHandler<PhysicalObject>? CollisionStopEvent;

        public EventHandler<PhysicalObject>? TriggerStartEvent;
        public EventHandler<PhysicalObject>? TriggerStayEvent;
        public EventHandler<PhysicalObject>? TriggerStopEvent;

        public FTransform Transform { get; }
        public bool InternalTransform { get; }

        /// <summary>
        /// 物体质量
        /// </summary>
        private float _mass = 1;
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                ApplyInertia();// 设置RigidBody的质量
            }
        }
        /// <summary>
        /// 物体是否为运动学,只能通过transform移动，不受物理世界影响，但是能影响其他，比如撞其他物体
        /// </summary>
        private bool _isKinematic = false;
        public bool IsKinematic
        {
            get => _isKinematic;
            set
            {
                _isKinematic = value;
                if (value)
                {
                    ClearForces();
                    LinerVelocity = Vector3.Zero;
                    AngularVelocity = Vector3.Zero; // 就像本来运动的物体如果我们设置为运动学状态此时瞬间速度为0，但是后续可以给他添加速度
                }
                RecreateBody(); // 重新创建rigidBody
            }
        }

        /// <summary>
        /// 是否为触发器
        /// </summary>
        private bool _isTrigger = false;
        public bool IsTrigger
        {
            get => _isTrigger;
            set
            {
                _isTrigger = value;
                if (value) AddFlag(CollisionFlags.NoContactResponse);
                else RemoveFlag(CollisionFlags.NoContactResponse);
            }
        }

        private bool _isEnable = true;
        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                if (!value) UnConsider();
                else Consider();
            }
        }
        /// <summary>
        /// 是否添加到物理世界_world中
        /// </summary>
        public bool Considered { get; set; } = false;

        private ECollisionDetectionMode _collisionDetectionMode = ECollisionDetectionMode.Discrete;
        private ECollisionDetectionMode CollisionDetectionMode
        {
            get => _collisionDetectionMode;
            set
            {
                _collisionDetectionMode = value;
                switch (CollisionDetectionMode)
                {
                    case ECollisionDetectionMode.Discrete:
                        Body.CcdMotionThreshold = float.MaxValue;
                        Body.CcdSweptSphereRadius = 0f;
                        break;
                    case ECollisionDetectionMode.Continuous:
                        Body.CcdMotionThreshold = 1e-7;
                        Body.CcdSweptSphereRadius = 0.5f;
                        break;
                }
            }
        }
        //user data
        private object? _userData;

        public T? UserData<T>()
            where T : class
        {
            return _userData as T;
        }

        public void SetData<T>(T? data)
        {
            _userData = data;
        }

        private Vector3 _previousVectorScale = Vector3.Zero;
        public static event EventHandler<PhysicalObject>? CreateEvent;
        public static event EventHandler<PhysicalObject>? DestroyEvent;
        public static event EventHandler<RigidBody>? ConsiderEvent;
        public static event EventHandler<RigidBody>? UnConsiderEvent;

        private MotionState _motion;
        public RigidBody Body { get; internal set; }
        protected CollisionShape Shape; // 由子类构造

        #region Rigidbody Parameter

        public float Bounciness
        {
            get => (float)Body.Restitution;
            set => Body.Restitution = value;
        }
        public float Friction
        {
            get => (float)Body.Friction;
            set => Body.Friction = value;
        }
        public Vector3 LinerVelocity
        {
            get => Conversion.ToTkVector3(Body.LinearVelocity);
            set => Body.LinearVelocity = Conversion.ToBtVector3(value);
        }
        public Vector3 AngularVelocity
        {
            get => Conversion.ToTkVector3(Body.AngularFactor);
            set => Body.AngularVelocity = Conversion.ToBtVector3(value);
        }
        public Vector3 LinearFactor
        {
            get => Conversion.ToTkVector3(Body.LinearFactor);
            set => Body.LinearFactor = Conversion.ToBtVector3(value);
        }
        public Vector3 AngularFactor
        {
            get => Conversion.ToTkVector3(Body.AngularFactor);
            set => Body.AngularFactor = Conversion.ToBtVector3(value);

        }
        public EActivationState ActivationState
        {
            get => (EActivationState)(int)(Body.ActivationState);
            set => Body.ActivationState = (ActivationState)(int)value;
        }

        #endregion

        protected abstract void SetLocalScaling(Vector3 scaling);
        public PhysicalObject()
        {
            Transform = new FTransform();
            InternalTransform = true;
            CollisionStartEvent += (s, e) =>
            {
                UpdateBtTransform();
            };
        }

        public PhysicalObject(FTransform transform)
        {
            Transform = transform;
            InternalTransform = false;
        }

        protected void Init()
        {
            CreateEvent?.Invoke(this, this);
            CreateBody(new BodySettings());
        }


        /// <summary>
        /// 添加一个力,不会受质量影响
        /// 可知在body原有作用力的基础上再加force*m_linearFactor，至于这个m_linearFactor就是对施加的力三个方向各缩放一定的倍数。
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Vector3 force)
        {
            Body.ApplyCentralForce(Conversion.ToBtVector3(force));
        }

        /// <summary>
        /// 添加一个力,会受质量影响
        /// 可知在body原有作用力的基础上再加force*m_linearFactor/mass，至于这个m_linearFactor就是对施加的力三个方向各缩放一定的倍数。
        /// impulse * 质量的倒数 = 增加的速度（线速度或者角速度）
        /// </summary>
        /// <param name="impulse"></param>
        public void AddImpulse(Vector3 impulse)
        {
            Body.ApplyCentralImpulse(Conversion.ToBtVector3(impulse));
        }

        /// <summary>
        /// 清除施加的Force和Torque，但body还是会受重力的影响。
        /// </summary>
        public void ClearForces()
        {
            Body.ClearForces();
        }
        public void AddFlag(CollisionFlags flag)
        {
            Body.CollisionFlags |= flag;
        }
        public void RemoveFlag(CollisionFlags flag)
        {
            Body.CollisionFlags |= ~flag;
        }
        /// <summary>
        /// 当物体缩放了，那么碰撞体也相应的缩放
        /// </summary>
        internal void UpdateBtTransform()
        {
            Body.WorldTransform = Conversion.ToBtTransform(Transform);
            if (Vector3.Distance(Transform.WorldScale, _previousVectorScale) >= 0.01f)
            {
                _previousVectorScale = Transform.WorldScale;
                SetLocalScaling(new Vector3(MathHelper.Abs(_previousVectorScale.X), MathHelper.Abs(_previousVectorScale.Y), MathHelper.Abs(_previousVectorScale.Z)));
                RecreateBody();
            }
        }
        /// <summary>
        /// 更具刚体位置和旋转更新物体
        /// </summary>
        internal void UpdateFTransform()
        {
            if (!IsKinematic)
            {
                var result = Body.WorldTransform;
                result.Decompose(out var scale, out var rotation,
                    out var translation);
                Transform.LocalPosition = Conversion.ToTkVector3(translation);
                Transform.LocalRotation = Conversion.ToTkQuaternion(rotation);
            }
        }
        protected void RecreateBody()
        {
            CreateBody(DestroyBody());
        }
        protected void ApplyInertia()
        {
            // bullet会根据质量和当前的碰撞形状计算惯性
            Body.SetMassProps(IsKinematic ? 0f : MathHelper.Max(0.0000001f, Mass), IsKinematic ? BulletSharp.Math.Vector3.Zero : CalculateInertia());
        }
        public void Consider()
        {
            if (!Considered)
            {
                Considered = true;
                ConsiderEvent?.Invoke(this, Body);
            }
        }
        public void UnConsider()
        {
            if (Considered)
            {
                Considered = false;
                UnConsiderEvent?.Invoke(this, Body);
            }
        }
        private void CreateBody(BodySettings bodySettings)
        {
            _motion = new DefaultMotionState(Conversion.ToBtTransform(Transform));
            Body = new RigidBody(new RigidBodyConstructionInfo(0, _motion, Shape, BulletSharp.Math.Vector3.Zero));
            ApplyInertia(); //质量设置

            Body.Restitution = bodySettings.Restitution;
            Body.Friction = bodySettings.Friction;
            Body.LinearVelocity = bodySettings.LinearVelocity;
            Body.AngularVelocity = bodySettings.AngularVelocity;
            Body.LinearFactor = bodySettings.LinearFactor;
            Body.AngularFactor = bodySettings.AngularFactor;
            Body.UserObject = this;

            AddFlag(CollisionFlags.CustomMaterialCallback);
            if (bodySettings.IsTrigger) AddFlag(CollisionFlags.NoContactResponse);
            ActivationState = EActivationState.AlwaysActive;
            if (_isEnable) Consider();
        }
        private BodySettings DestroyBody()
        {
            BodySettings result = new BodySettings()
            {
                LinearVelocity = Body.LinearVelocity,
                AngularVelocity = Body.AngularVelocity,
                LinearFactor = Body.LinearFactor,
                AngularFactor = Body.AngularFactor,
                Restitution = Bounciness,
                Friction = Friction,
                IsTrigger = IsTrigger,
                IsKinematic = IsKinematic,
            };
            UnConsider();
            Body.Dispose();
            _motion.Dispose();
            return result;
        }
        public T GetUserData<T>()
        {
            throw new NotImplementedException();

        }
        public void SetUserData<T>(T userData)
        {

        }
        private BulletSharp.Math.Vector3 CalculateInertia()
        {
            BulletSharp.Math.Vector3 result = BulletSharp.Math.Vector3.Zero;
            if (Mass != 0)
            {
                Shape.CalculateLocalInertia(Mass, out result);
            }

            return result;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Shape.Dispose();
                _motion.Dispose();
                Body.Dispose();
                DestroyBody();
                DestroyEvent?.Invoke(this, this);
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
