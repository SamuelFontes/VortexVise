using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Models
{
    public interface IFontAsset
    {
        public void Load(string path); 
        public void Unload();
    }
}
