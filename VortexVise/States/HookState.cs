using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.States
{
    public class HookState
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Rectangle Collision;
        public bool IsHookAttached;
        public bool IsHookReleased;
        public bool IsPressingHookKey;
    }
}
