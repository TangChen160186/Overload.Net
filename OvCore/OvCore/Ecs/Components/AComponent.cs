using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace OvCore.OvCore.Ecs.Components
{
    [DataContract]

    public abstract class AComponent:IDisposable
    {
        public abstract string Name { get; }
        public Actor Owner { get; private set; }
        protected AComponent(Actor actor)
        {
            Owner = actor;
        }

        public virtual void OnAwake()
        {

        }
        public virtual void OnStart()
        {

        }
        public virtual void OnEnable()
        {

        }
        public virtual void OnDisable()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public virtual void OnUpdate(float deltaTime)
        {

        }
        public virtual void OnFixedUpdate(float deltaTime)
        {

        }
        public virtual void OnLateUpdate(float deltaTime)
        {

        }
        public virtual void OnCollisionEnter(CPhysicalObject otherObject)
        {

        }

        public virtual void OnCollisionStay(CPhysicalObject otherObject)
        {

        }
        public virtual void OnCollisionExit(CPhysicalObject otherObject)
        {

        }

        public virtual void OnTriggerEnter(CPhysicalObject otherObject)
        {

        }

        public virtual void OnTriggerStay(CPhysicalObject otherObject)
        {

        }
        public virtual void OnTriggerExit(CPhysicalObject otherObject)
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        internal virtual void Init(Actor owner)
        {
            Owner = owner;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
