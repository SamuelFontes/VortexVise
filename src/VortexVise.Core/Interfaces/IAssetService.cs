﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Interfaces
{
    public interface IAssetService
    {
        public ITextureAsset LoadTexture(string fileName); 
        void UnloadTexture(ITextureAsset texture);
    }
}
