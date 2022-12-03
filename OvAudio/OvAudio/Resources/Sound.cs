using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvAudio.OvAudio.Resources
{
    public class Sound : IDisposable
    {
        public string Path { get; internal set; }

        public Sound(string path)
        {
            Path = path;
        }

        public void Dispose()
        {
        }
    }
}
