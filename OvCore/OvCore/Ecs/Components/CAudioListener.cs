using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OvAudio.OvAudio.Entities;
using OvMath;

namespace OvCore.OvCore.Ecs.Components
{
    public class CAudioListener:AComponent
    {
        public override string Name => nameof(CAudioListener);
        private AudioListener _audioListener;
        public CAudioListener(Actor actor) : base(actor)
        {
            _audioListener = new AudioListener(Owner.Transform.Transform);
            _audioListener.Enabled = false;
        }

        public override void OnEnable()
        {
            _audioListener.Enabled = true;
        }

        public override void OnDisable()
        {
            _audioListener.Enabled = false;
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
            _audioListener.Transform = owner.Transform.Transform;
        }

        [OnDeserializing]
        public void OnDeserializing(StreamingContext context)
        {
            _audioListener = new AudioListener(new FTransform());
        }
    }
}
