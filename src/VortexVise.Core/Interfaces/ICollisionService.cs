using System.Drawing;

namespace VortexVise.Core.Interfaces
{
    public interface ICollisionService
    {
        public bool DetectCollision(Rectangle rec1, Rectangle rec2);
        public Rectangle GetCollision(Rectangle rec1, Rectangle rec2);
    }
}
