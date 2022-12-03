using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;
using OvAudio.OvAudio.Core;
using OvAudio.OvAudio.Resources;
using OvAudio.OvAudio.Tracking;
using OvMath;

namespace OvAudio.OvAudio.Entities
{
    /// <summary>
    ///  Wrap Irrklang ISound
    /// </summary>
    public class AudioSource : IDisposable
    {
        public static event EventHandler<AudioSource>? CreatedEvent;
        public static event EventHandler<AudioSource>? DestroyedEvent;

        private readonly AudioPlayer _audioPlayer;
        public SoundTracker? TrackedSound { get; private set; }

        private  FTransform _transform;

        public FTransform Transform
        {
            get => _transform;
            set
            {
                _transform =value;
                InternalTransform = true;
            }
        }

        public bool InternalTransform { get; private set; } // 判断是声音位置是外部输入还是内部初始化的，外部通常为组件的CTransform组件,内部默认为(0,0,0)


        #region Setting Pamaters
        private float _volume = 1;
        private float _pan = 0;
        private bool _looped = false;
        private float _pitch = 1;
        private float _attenuationThreshold = 1;


        #endregion

        /// <summary>
        /// 音量大小
        /// </summary>
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                if (HasTrackedSound())
                {
                    TrackedSound!.Track.Volume = value;
                }
            }
        }
        /// <summary>
        /// 音量声道 -1 左，1右，0中
        /// </summary>
        public float Pan
        {
            get => _pan;
            set
            {
                _pan = value;
                if (HasTrackedSound())
                {
                    TrackedSound!.Track.Pan = value * -1;
                }
            }
        }
        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool Looped
        {
            get => _looped;
            set
            {
                _looped = value;
                if (HasTrackedSound())
                {
                    TrackedSound!.Track.Looped = value;
                }
            }
        }
        /// <summary>
        /// 播放速度，用speed更加合理吧
        /// </summary>
        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                if (HasTrackedSound())
                {
                    TrackedSound!.Track.PlaybackSpeed = (value < 0.01f ? 0.01f : value);
                }
            }
        }
        /// <summary>
        /// true:播放3D音频,会根据listener 和 audioSource相对位置和方位调整音量大小，如果靠近右边，那么右声道大，否则相反
        /// false:音量
        /// </summary>
        public bool IsSpatial { get; set; } = false;

        /// <summary>
        /// audioSource 和 listener超过这个阙值，声音开始衰减，默认为1
        /// </summary>
        public float AttenuationThreshold
        {
            get => _attenuationThreshold;
            set
            {
                _attenuationThreshold = value;
                if (HasTrackedSound())
                {
                    TrackedSound!.Track.MinDistance = value;
                }
            }
        }

        /// <summary>
        /// Returns true if the audio source sound has finished
        /// </summary>
        public bool IsFinished
        {
            get
            {
                if (HasTrackedSound())
                {
                    return TrackedSound!.Track.Finished;
                }
                return true;
            }
        }

        public AudioSource(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
            _transform = new FTransform();
            InternalTransform = true;

            Setup();
        }

        public AudioSource(AudioPlayer audioPlayer, FTransform transform)
        {
            _audioPlayer = audioPlayer;
            _transform = transform;
            InternalTransform = false;
            Setup();
        }



        /// <summary>
        /// 更新声音源的位置
        /// </summary>
        public void UpdateTrackedSoundPosition()
        {
            if (HasTrackedSound() && IsSpatial)
            {
                var pos = Transform.WorldPosition;
                TrackedSound!.Track.Position = new Vector3D(pos.X, pos.Y, pos.Z);
            }
        }
        /// <summary>
        /// 每次播放前应该应用该设置
        /// </summary>
        public void ApplySourceSettingsToTrackedSound()
        {
            if (HasTrackedSound())
            {
                TrackedSound!.Track.Volume = Volume;
                TrackedSound.Track.Pan = Pan;
                TrackedSound.Track.Looped = Looped;
                TrackedSound.Track.PlaybackSpeed = Pitch;
                TrackedSound.Track.MinDistance = AttenuationThreshold;
            }

        }

        public void Play(Sound _sound)
        {
            StopAndDestroyTrackedSound();

            if (IsSpatial)
            {
                TrackedSound =
                    _audioPlayer.PlaySpatialSound(_sound, Transform.WorldPosition, false, _looped, true);
            }
            else
            {
                TrackedSound = _audioPlayer.PlaySound(_sound, false, _looped, true);
            }

            if (HasTrackedSound())
            {
                TrackedSound!.Track.Volume = Volume;
                TrackedSound.Track.Pan = Pan;
                TrackedSound.Track.PlaybackSpeed = Pitch;
                TrackedSound.Track.MinDistance = AttenuationThreshold;
                TrackedSound.Track.Paused = false;
            }
        }
        /// <summary>
        /// 重新播放
        /// </summary>
        public void Resume()
        {
            if (HasTrackedSound())
            {
                TrackedSound!.Track.Paused = false;
            }
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            if (HasTrackedSound())
            {
                TrackedSound!.Track.Paused = true;
            }
        }
        /// <summary>
        /// 将停止声音并释放其资源。
        /// 如果只想暂停声音，请使用setIsPaused()。
        /// 调用stop()后，isFinished()通常返回true。
        /// </summary>
        public void Stop()
        {
            if (HasTrackedSound())
            {
                TrackedSound!.Track.Stop();
                TrackedSound.Track.Dispose();
            }
        }

        /// <summary>
        /// 在停止上一个ISound,并且释放资源
        /// </summary>
        public void StopAndDestroyTrackedSound()
        {
            if (HasTrackedSound())
            {
                TrackedSound!.Track.Stop();
                TrackedSound.Track.Dispose();
            }
        }
        public bool HasTrackedSound()
        {
            return TrackedSound != null;
        }


        private void Setup()
        {
            CreatedEvent?.Invoke(this, this);
        }

        public void Dispose()
        {
            DestroyedEvent?.Invoke(this, this);
            StopAndDestroyTrackedSound();
        }
    }
}
