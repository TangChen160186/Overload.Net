using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;

namespace OvAudio.OvAudio.Tracking
{
    public class SoundTracker
    {
        /// <summary>
        /// FinishedEvent is called when the track get stopped/finished
        /// </summary>
        public event EventHandler? StopEvent;

        public ISound Track { get; }

        private readonly SoundStopEventBinder _soundStopEventBinder;
        public SoundTracker(ISound track)
        {
            Track = track;
            _soundStopEventBinder = new SoundStopEventBinder();
            _soundStopEventBinder.SoundFinishedEvent += StopEvent;
        }
    }
}
