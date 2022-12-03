using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;
using OpenTK.Mathematics;
using OvAudio.OvAudio.Entities;

namespace OvAudio.OvAudio.Core
{
    public class AudioEngine : IDisposable
    {
        private readonly List<AudioSource> _audioSources = new();
        private readonly List<AudioSource> _suspendedAudioSources = new();
        private readonly List<AudioListener> _audioListeners = new();

        public string WorkingDirectory { get; }
        public bool IsSuspended { get; private set; } = false;


        public ISoundEngine IrrklangEngine { get; }

        public AudioEngine(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;

            IrrklangEngine = new ISoundEngine();
            AudioSource.CreatedEvent += Consider;
            AudioListener.CreatedEvent += Consider;
            AudioSource.DestroyedEvent += UnConsider;
            AudioListener.DestroyedEvent += UnConsider;
        }

        public void Update()
        {
            foreach (var audioSource in _audioSources)
            {
                audioSource.UpdateTrackedSoundPosition();
            }
            var listener = GetListenerInformation();
            if (listener.HasValue)
            {
                var item1 = listener.Value.Item1;
                var item2 = listener.Value.Item2;
                IrrklangEngine.SetListenerPosition(new Vector3D(item1.X, item1.Y, item1.Z),
                    new Vector3D(item2.X, item2.Y, item2.Z));
            }
            else
            {
                IrrklangEngine.SetListenerPosition(new Vector3D(0, 0, 0), new Vector3D(0, 0, -1.0f));
            }
        }

        public void Suspend()
        {
            foreach (var audioSource in _audioSources)
            {
                if (audioSource.HasTrackedSound() && !audioSource.TrackedSound!.Track.Paused)
                {
                    _suspendedAudioSources.Add(audioSource);
                    audioSource.Pause();
                }
            }

            IsSuspended = true;
        }

        public void UnSuspend()
        {
            foreach (var audioSource in _suspendedAudioSources)
            {
                audioSource.Resume();
            }
            _suspendedAudioSources.Clear();
            IsSuspended = false;
        }


        public (Vector3, Vector3)? GetListenerInformation(bool considerDisabled = false)
        {
            foreach (var audioListener in _audioListeners)
            {
                if (audioListener.Enabled || considerDisabled)
                {
                    var transform = _audioListeners.Last().Transform;
                    return (transform.WorldPosition, transform.WorldForward * -1f);
                }
            }

            return null;
        }
        private void Consider(object? sender, AudioSource audioSource)
        {
            _audioSources.Add(audioSource);
        }
        private void Consider(object? sender, AudioListener audioListener)
        {
            _audioListeners.Add(audioListener);
        }

        private void UnConsider(object? sender, AudioSource audioSource)
        {
            _audioSources.Remove(audioSource);
            if (IsSuspended)
            {
                _suspendedAudioSources.Remove(audioSource);
            }
            audioSource.Dispose();
        }
        private void UnConsider(object? sender, AudioListener audioListener)
        {
            _audioListeners.Remove(audioListener);
            audioListener.Dispose();
        }

        public void Dispose()
        {
            IrrklangEngine.Dispose();
        }
    }
}
