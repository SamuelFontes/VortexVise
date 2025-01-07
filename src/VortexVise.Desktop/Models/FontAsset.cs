using VortexVise.Core.Interfaces;
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
