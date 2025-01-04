using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Interfaces
{
    public interface IMusicAsset
    {
        public bool IsPlaying { get; set; }
        public bool IsLoaded { get; set; }
        public float Volume { get; set; }
        public void Load(string path);
        public void Unload();
        public void SetVolume(float volume);
        public void Play();
        public void Update();
        public void Stop();
    }
}
