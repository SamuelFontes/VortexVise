using System.Drawing;

namespace VortexVise.Core.Interfaces
{
    public interface ICollisionService
    {
        public bool CheckCollisionRecs(Rectangle rec1, Rectangle rec2);
        public Rectangle GetCollisionRec(Rectangle rec1, Rectangle rec2);
    }
}
