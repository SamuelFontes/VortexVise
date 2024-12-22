using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Interfaces
{
    public interface ICollisionService
    {
        public bool CheckCollisionRecs(Rectangle rec1, Rectangle rec2);
        public Rectangle GetCollisionRec(Rectangle rec1, Rectangle rec2);
    }
}
