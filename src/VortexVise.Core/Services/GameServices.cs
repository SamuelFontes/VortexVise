using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;

namespace VortexVise.Core.Services
{
    public class GameServices(IAssetService assetService, ICollisionService collisionService, IInputService inputService, IRendererService rendererService, ISoundService soundService)
    {
        public IAssetService AssetService { get; set; } = assetService;
        public ICollisionService CollisionService { get; set; } = collisionService;
        public IInputService InputService { get; set; } = inputService;
        public IRendererService RendererService { get; set; } = rendererService;
        public ISoundService SoundService { get; set; } = soundService;
    }
}
