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
        public Guid OwnerId { get; set; }
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Velocity { get; set; } = new Vector2(0, 0);
        public Rectangle Collision { get; set; } = new Rectangle(0, 0, 0, 0);
        public bool IsHookAttached { get; set; } = false;
        public bool IsHookReleased { get; set; } = false;
        public bool IsPressingHookKey { get; set; } = false;
    }
}
