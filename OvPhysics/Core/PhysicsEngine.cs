using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK.Mathematics;
using OvPhysics.Entities;
using OvPhysics.Settings;
using OvPhysics.Tools;

namespace OvPhysics.Core
{
    public class PhysicsEngine : IDisposable
    {
        private readonly DynamicsWorld _world;

        private static readonly Dictionary<(PhysicalObject, PhysicalObject), bool> CollisionEvents = new();
        private readonly List<PhysicalObject> _physicalObjects = new();


        /// <summary>
        /// 重力
        /// </summary>
        public Vector3 Gravity
        {
            get => Conversion.ToTkVector3(_world.Gravity);
            set => _world.Gravity = Conversion.ToBtVector3(value);
        }


        public PhysicsEngine(PhysicsSettings settings)
        {
            CollisionConfiguration collisionConfig = new DefaultCollisionConfiguration(); //碰撞配置，物理世界存在柔体的话，使用btSoftBodyRigidBodyCollisionConfiguration，不存在则使用btDefaultCollisionConfiguration
            Dispatcher dispatcher = new CollisionDispatcher(collisionConfig); //Narrow-Phase，碰撞调度，精确检测碰撞
            BroadphaseInterface broadPhase = new DbvtBroadphase(); //Broad-Phase，使用AABB的BVH快速检测碰撞
            ConstraintSolver solver = new SequentialImpulseConstraintSolver(); //解算器
            _world = new DiscreteDynamicsWorld(dispatcher, broadPhase, solver, collisionConfig);      //创建物理世界，柔体需要使用btSoftRigidDynamicsWorld
            _world.Gravity = Conversion.ToBtVector3(settings.Gravity); // 重力设置

            ListenToPhysicalObjects();
            SetCollisionCallback();
        }

        private void PreUpdate()
        {
            foreach (var physicalObject in _physicalObjects)
            {
                physicalObject.UpdateBtTransform(); // 更新碰撞体大小
            }
            ResetCollisionEvents();  // 开始模拟之前默认所有上一次发生碰撞的物体之间是没有碰撞的
        }

        private void PostUpdate()
        {
            foreach (var physicalObject in _physicalObjects)
            {
                physicalObject.UpdateFTransform();
            }
            CheckCollisionStopEvents();
        }
        public bool Update(float deltaTime)
        {
            PreUpdate();
            if (_world.StepSimulation(deltaTime, 10) > 0)
            {
                PostUpdate();
                return true;
            }

            return false;
        }

        public RayCastHit? RayCast(Vector3 origin, Vector3 direction, float distance)
        {
            if (direction == Vector3.Zero) return null;
            var originPos = Conversion.ToBtVector3(origin);
            var targetPos = Conversion.ToBtVector3(origin + direction * distance);
            RayCastHit? hit = null;

            var closestRayCallback = new ClosestRayResultCallback(ref originPos, ref targetPos);
            _world.RayTest(originPos, targetPos, closestRayCallback);

            if (closestRayCallback.HasHit)
            {
                RayCastHit temp = new RayCastHit
                {
                    FirstResultObject = (closestRayCallback.CollisionObject.UserObject as PhysicalObject)!
                };
                AllHitsRayResultCallback rayCallback = new AllHitsRayResultCallback(originPos, targetPos);
                foreach (var rayCallbackCollisionObject in rayCallback.CollisionObjects)
                {
                    temp.ResultObjects.Add(
                        (rayCallbackCollisionObject.UserObject as PhysicalObject)!);
                }
                hit = temp;
            }

            return hit;
        }

        private void ListenToPhysicalObjects()
        {
            PhysicalObject.CreateEvent += Consider;
            PhysicalObject.DestroyEvent -= UnConsider;

            PhysicalObject.ConsiderEvent += Consider;
            PhysicalObject.UnConsiderEvent -= UnConsider;
        }

        private void Consider(object? sender, PhysicalObject toConsider)
        {
            _physicalObjects.Add(toConsider);
        }
        private void UnConsider(object? sender, PhysicalObject toUnConsider)
        {
            _physicalObjects.Remove(toUnConsider);
            var key = CollisionEvents.Keys.ToList();
            foreach (var key2 in key.Where(key2 => key2.Item1 == toUnConsider || key2.Item2 == toUnConsider))
            {
                CollisionEvents.Remove(key2);
            }
        }

        private void Consider(object? sender, RigidBody toConsider)
        {
            _world.AddRigidBody(toConsider);
        }
        private void UnConsider(object? sender, RigidBody toUnConsider)
        {
            _world.RemoveRigidBody(toUnConsider);
        }
        /// <summary>
        /// 重置碰撞结果为false
        /// </summary>
        private void ResetCollisionEvents()
        {
            foreach (var collisionEvent in CollisionEvents)
            {
                CollisionEvents[collisionEvent.Key] = false;
            }
        }
        /// <summary>
        /// 检查哪些物体之间停止碰撞或者停止触发，调用他们的CollisionStopEvent或者TriggerStopEvent，然后将他从事件中删除
        /// </summary>
        private void CheckCollisionStopEvents()
        {
            List<(PhysicalObject, PhysicalObject)> cache = new List<(PhysicalObject, PhysicalObject)>();
            foreach (var (key, value) in CollisionEvents)
            {
                var item1 = key.Item1;
                var item2 = key.Item2;
                if (!value)
                {
                    if (!item1.IsTrigger && !item2.IsTrigger)
                    {
                        item1.CollisionStopEvent?.Invoke(item2, item2);
                        item2.CollisionStopEvent?.Invoke(item1, item1);
                    }
                    else
                    {
                        if (item1.IsTrigger) item1.TriggerStopEvent?.Invoke(item2, item2);
                        else item2.TriggerStopEvent?.Invoke(item1, item1);
                    }
                    cache.Add(key);
                }
            }

            foreach (var obj in cache)
            {
                CollisionEvents.Remove(obj);
            }
        }

        private static void CollisionCallback(ManifoldPoint cp, CollisionObjectWrapper obj1, int id1, int index1,
            CollisionObjectWrapper obj2, int id2, int index2)
        {
            var object1 = ReinterpretCast<object, PhysicalObject>(obj1.CollisionObject.UserObject);
            var object2 = ReinterpretCast<object, PhysicalObject>(obj2.CollisionObject.UserObject);

            if (object1 != null && object2 != null)
            {
                if (!object1.IsTrigger || !object2.IsTrigger)
                {
                    if (!CollisionEvents.ContainsKey((object1, object2)))
                    {
                        if (object1.IsTrigger)
                        {
                            object1.TriggerStartEvent?.Invoke(object2, object2);
                        }
                        else
                        {
                            if (!object2.IsTrigger)
                            {
                                object1.CollisionStartEvent?.Invoke(object2, object2);
                            }
                        }

                        if (object2.IsTrigger)
                        {
                            object2.TriggerStartEvent?.Invoke(object1, object1);
                        }
                        else
                        {
                            if (!object1.IsTrigger)
                            {
                                object2.CollisionStartEvent?.Invoke(object1, object1);
                            }
                        }

                        if (object1.IsTrigger)
                        {
                            object1.TriggerStayEvent?.Invoke(object2, object2);
                        }
                        else
                        {
                            if (!object2.IsTrigger)
                            {
                                object1.CollisionStayEvent?.Invoke(object2, object2);
                            }
                        }

                        if (object2.IsTrigger)
                        {
                            object2.TriggerStayEvent?.Invoke(object1, object1);
                        }
                        else
                        {
                            if (!object1.IsTrigger)
                            {
                                object2.CollisionStayEvent?.Invoke(object1, object1);
                            }
                        }
                        CollisionEvents.Add((object1, object2), true);
                    }
                    else
                    {
                        if (!CollisionEvents[(object1, object2)])
                        {
                            if (object1.IsTrigger)
                            {
                                object1.TriggerStayEvent?.Invoke(object2, object2);
                            }
                            else
                            {
                                if (!object2.IsTrigger)
                                {
                                    object1.CollisionStayEvent?.Invoke(object2, object2);
                                }
                            }

                            if (object2.IsTrigger)
                            {
                                object2.TriggerStayEvent?.Invoke(object1, object1);
                            }
                            else
                            {
                                if (!object1.IsTrigger)
                                {
                                    object2.CollisionStayEvent?.Invoke(object1, object1);
                                }
                            }
                            CollisionEvents[(object1, object2)] = true;
                        }
                    }
                }

            }
        }

        private void SetCollisionCallback()
        {
            ManifoldPoint.ContactAdded += CollisionCallback;
        }

        private static unsafe TDest? ReinterpretCast<TSource, TDest>(TSource? source)
        {
            var sourceRef = __makeref(source);
            var dest = default(TDest);
            var destRef = __makeref(dest);
            *(IntPtr*)&destRef = *(IntPtr*)&sourceRef;
            return __refvalue(destRef, TDest);
        }

        public void Dispose()
        {
            _world.Dispose();
        }
    }
}
