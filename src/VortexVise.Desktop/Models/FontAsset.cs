using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Models
{
    internal class FontAsset : IFontAsset
    {
        public Font Font { get; set; }
        public void Load(string path)
        {
            Font = Raylib.LoadFont(path);
        }

        public void Unload()
        {
            Raylib.UnloadFont(Font);
        }
    }
}
