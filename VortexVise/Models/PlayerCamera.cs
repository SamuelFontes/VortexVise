using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ZeroElectric.Vinculum;

namespace VortexVise.Models;

public class PlayerCamera
{
    public Camera2D Camera;
    public RenderTexture RenderTexture;
    public Rectangle RenderRectangle;
    public Vector2 CameraPosition;
}
