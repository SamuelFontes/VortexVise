using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Models;

public class Animation
{
    public Animation(string textureLocation, int size, int stateAmount, int scale, Color color, float frameTime)
    {
        Texture = Raylib.LoadTexture(textureLocation);
        Size = size;
        StateAmount = stateAmount;
        Scale = scale;
        Color = color;
        FrameTime = frameTime;
    }
    public Texture Texture;
    public int Size;
    public int StateAmount;
    public int Scale;
    public Color Color;
    public float FrameTime;
}
