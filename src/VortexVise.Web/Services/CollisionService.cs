using VortexVise.Core.Interfaces;

namespace VortexVise.Web.Services
{
    internal class CollisionService : ICollisionService
    {
        public bool CheckCollisionRecs(System.Drawing.Rectangle rec1, System.Drawing.Rectangle rec2)
        {
            var newrec1 = new Raylib_cs.Rectangle(rec1.X, rec1.Y, rec1.Width, rec1.Height);
            var newrec2 = new Raylib_cs.Rectangle(rec2.X, rec2.Y, rec2.Width, rec2.Height);
            return Raylib_cs.Raylib.CheckCollisionRecs(newrec1, newrec2);
        }

        public System.Drawing.Rectangle GetCollisionRec(System.Drawing.Rectangle rec1, System.Drawing.Rectangle rec2)
        {
            var newrec1 = new Raylib_cs.Rectangle(rec1.X, rec1.Y, rec1.Width, rec1.Height);
            var newrec2 = new Raylib_cs.Rectangle(rec2.X, rec2.Y, rec2.Width, rec2.Height);
            var result = Raylib_cs.Raylib.GetCollisionRec(newrec1, newrec2);
            return new((int)result.X, (int)result.Y, (int)result.Width, (int)result.Height);
        }
    }
}
