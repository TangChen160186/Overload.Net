using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvAudio.OvAudio.Resources.Loaders
{
    public class SoundLoader
    {
        private SoundLoader() { }

        public static Sound Create(string filePath)
        {
            return new Sound(filePath);
        }

        public static void Reload(Sound sound, string filePath)
        {
            sound.Path = filePath;
        }

        public static bool Destroy(ref Sound? sound)
        {
            if (sound != null)
            {
                sound = null;
                return true;
            }

            return false;
        }
    }
}
