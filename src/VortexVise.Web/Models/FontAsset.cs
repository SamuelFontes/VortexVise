using Raylib_cs;
using VortexVise.Core.Interfaces;

namespace VortexVise.Web.Models
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
