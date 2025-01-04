using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Models
{
    public interface IFontAsset
    {
        public string Path { get; set; }
        public void Load(); 
        public void Unload();
    }
}
