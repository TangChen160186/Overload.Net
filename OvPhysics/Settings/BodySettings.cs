using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.Math;

namespace OvPhysics.Settings
{
    public struct BodySettings
    {
        public Vector3 LinearVelocity = Vector3.Zero;
        public Vector3 AngularVelocity = Vector3.Zero;

        public Vector3 LinearFactor = Vector3.One;
        public Vector3 AngularFactor = Vector3.One;
        /// <summary>
        /// 物理学中表示:(因弹性体的)复原
        /// </summary>
        public float Restitution = 0.0f;
        /// <summary>
        /// 摩檫力
        /// </summary>
        public float Friction = 0.0f;
        /// <summary>
        /// 是否为触发器
        /// </summary>
        public bool IsTrigger = false;
        /// <summary>
        /// 是否为静态物体
        /// </summary>
        public bool IsKinematic = false;

        public BodySettings()
        {
        }
    }
}
