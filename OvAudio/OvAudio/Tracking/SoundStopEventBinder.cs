using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;

namespace OvAudio.OvAudio.Tracking
{
    internal class SoundStopEventBinder : ISoundStopEventReceiver
    {
        void ISoundStopEventReceiver.OnSoundStopped(ISound sound, StopEventCause reason, object userData)
        {
            SoundFinishedEvent?.Invoke(null, EventArgs.Empty);
        }
        // 用于外部注册
        public event EventHandler? SoundFinishedEvent;
    }
}
