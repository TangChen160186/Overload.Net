using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvMath;

namespace OvAudio.OvAudio.Entities
{
    /// <summary>
    /// Represents the ears of your application.
    /// You can have multiple ones but only the last created will be considered by the AudioEngine
    /// </summary>
    public class AudioListener : IDisposable
    {
        private FTransform _transform;

        public FTransform Transform
        {
            get => _transform;
            set
            {
                _transform = value;
                InternalTransform = true;
            }
        }

        public bool InternalTransform { get; private set; }
        public bool Enabled { get; set; } = true;

        public static event EventHandler<AudioListener>? CreatedEvent;
        public static event EventHandler<AudioListener>? DestroyedEvent;

        public AudioListener()
        {
            Transform = new FTransform();
            InternalTransform = true;
            Setup();
        }

        public AudioListener(FTransform transform)
        {
            Transform = transform;
            InternalTransform = false;
            Setup();
        }

        private void Setup()
        {
            CreatedEvent?.Invoke(this, this);
        }

        public void Dispose()
        {
            DestroyedEvent?.Invoke(this, this);
        }
    }
}
