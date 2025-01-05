using VortexVise.Core.Interfaces;

namespace VortexVise.Core.Services
{
    public class GameServices(IAssetService assetService, ICollisionService collisionService, IInputService inputService, IRendererService rendererService, ISoundService soundService, IWindowService windowService)
    {
        public IAssetService AssetService { get; set; } = assetService;
        public ICollisionService CollisionService { get; set; } = collisionService;
        public IInputService InputService { get; set; } = inputService;
        public IRendererService RendererService { get; set; } = rendererService;
        public ISoundService SoundService { get; set; } = soundService;
        public IWindowService WindowService { get; set; } = windowService;
    }
}
