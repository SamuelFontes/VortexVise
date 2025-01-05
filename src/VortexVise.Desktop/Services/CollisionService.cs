using VortexVise.Core.Interfaces;
using ZeroElectric.Vinculum;

namespace VortexVise.Core.Services
{
    internal class CollisionService : ICollisionService
    {
        public bool CheckCollisionRecs(System.Drawing.Rectangle rec1, System.Drawing.Rectangle rec2)
        {
            var newrec1 = new Rectangle(rec1.X, rec1.Y, rec1.Width, rec1.Height);
            var newrec2 = new Rectangle(rec2.X, rec2.Y, rec2.Width, rec2.Height);
            return Raylib.CheckCollisionRecs(newrec1, newrec2);
        }

        public System.Drawing.Rectangle GetCollisionRec(System.Drawing.Rectangle rec1, System.Drawing.Rectangle rec2)
        {
            var newrec1 = new Rectangle(rec1.X, rec1.Y, rec1.Width, rec1.Height);
            var newrec2 = new Rectangle(rec2.X, rec2.Y, rec2.Width, rec2.Height);
            var result = Raylib.GetCollisionRec(newrec1, newrec2);
            return new((int)result.x, (int)result.y, (int)result.width, (int)result.height);
        }
    }
}
