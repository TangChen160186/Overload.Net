using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;
using OpenTK.Mathematics;
using OvAudio.OvAudio.Resources;
using OvAudio.OvAudio.Tracking;
using OvDebug;

namespace OvAudio.OvAudio.Core
{
    public class AudioPlayer : IDisposable
    {
        private readonly AudioEngine _audioEngine;
        public AudioPlayer(AudioEngine audioAudioEngine)
        {
            _audioEngine = audioAudioEngine;
        }

        public SoundTracker? PlaySound(Sound soundSrc, bool autoPlay = true, bool looped = false, bool track = false)
        {
            SoundTracker? result = null;

            var sound = _audioEngine.IrrklangEngine.Play2D(Path.Combine(_audioEngine.WorkingDirectory, soundSrc.Path),
                looped, autoPlay, StreamMode.AutoDetect, track);
            if (track)
            {
                if (sound != null)
                {
                    result = new SoundTracker(sound);
                }
                else
                {
                    OvLogger.Default.Error("Unable to play \"" + soundSrc.Path + "\"");
                }
            }

            return result;
        }

        public SoundTracker? PlaySpatialSound(Sound soundSrc, Vector3 position, bool autoPlay = true, bool looped = false, bool track = false)
        {
            SoundTracker? result = null;
            var sound = _audioEngine.IrrklangEngine.Play3D(Path.Combine(_audioEngine.WorkingDirectory, soundSrc.Path),
                position.X, position.Y, position.Z, looped, autoPlay, StreamMode.AutoDetect, track);

            if (track)
            {
                if (sound != null)
                {
                    result = new SoundTracker(sound);
                }
                else
                {
                    OvLogger.Default.Error("Unable to play \"" + soundSrc.Path + "\"");
                }
            }

            return result;
        }

        public void Dispose()
        {
            _audioEngine.Dispose();
        }
    }
}
