using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Interfaces
{
    public interface IPlayerCamera
    {
        public Vector2 CameraPosition { get; set; }
        public Vector2 CameraOffset { get; set; }

        public void Setup(int screenWidth, int screenHeight, int cameraWidth, int cameraHeight,float offsetX, float offsetY, int cameraPositionX = 0, int cameraPositionY = 0);
        public void SetTarget(Vector2 target);
        public void Unload();
    }
}
