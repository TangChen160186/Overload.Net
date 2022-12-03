using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OvAudio.OvAudio.Core;
using OvAudio.OvAudio.Entities;
using OvAudio.OvAudio.Resources;
using OvCore.OvCore.Global;
using OvMath;

namespace OvCore.OvCore.Ecs.Components
{
    [DataContract]
    public class CAudioSource : AComponent
    {
        public override string Name => nameof(CAudioSource);
        private AudioSource _audioSource;
        [DataMember]
        public Sound? Sound { get; set; } = null;
        [DataMember]
        public bool IsAutoPlay { get; set; }
        [DataMember]
        public float Volume
        {
            get => _audioSource.Volume;
            set => _audioSource.Volume = value;
        }
        [DataMember]
        public float Pan
        {
            get => _audioSource.Pan;
            set => _audioSource.Pan = value;
        }
        [DataMember]
        public bool IsLopped
        {
            get => _audioSource.Looped;
            set => _audioSource.Looped = value;
        }
        [DataMember]
        public float Pitch
        {
            get => _audioSource.Pitch;
            set => _audioSource.Pitch = value;
        }
        [DataMember]
        public bool Spatial
        {
            get => _audioSource.IsSpatial;
            set => _audioSource.IsSpatial = value;
        }
        [DataMember]
        public float AttenuationThreshold
        {
            get => _audioSource.AttenuationThreshold;
            set => _audioSource.AttenuationThreshold = value;
        }

        public CAudioSource(Actor actor) : base(actor)
        {
            _audioSource = new AudioSource(ServiceLocator.Get<AudioPlayer>()!, Owner.Transform.Transform);
        }
        public bool IsFinished => _audioSource.IsFinished;

        public void Play()
        {
            if (Owner.IsActive && Sound != null)
            {
                _audioSource.Play(Sound);
            }
        }
        public void Pause()
        {
            if (Owner.IsActive)
            {
                _audioSource.Pause();
            }
        }
        public void Resume()
        {
            if (Owner.IsActive)
            {
                _audioSource.Resume();
            }
        }
        public void Stop()
        {
            if (Owner.IsActive)
            {
                _audioSource.Stop();
            }
        }

        public override void OnEnable()
        {
            if(IsAutoPlay) Play();
        }

        public override void OnDisable()
        {
            _audioSource.Stop();
        }

        internal override void Init(Actor owner)
        {
            base.Init(owner);
            _audioSource.Transform = owner.Transform.Transform;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            _audioSource = new AudioSource(ServiceLocator.Get<AudioPlayer>()!, Owner.Transform.Transform);
        }
    }
}
