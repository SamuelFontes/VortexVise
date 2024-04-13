using ZeroElectric.Vinculum;

namespace VortexVise.Models;

public class Animation
{
    public Animation(string textureLocation, int size, int stateAmount)
    {
        Texture = Raylib.LoadTexture(textureLocation);
        Size = size;
        StateAmount = stateAmount;
    }
    public Texture Texture;
    public int Size;
    public int StateAmount;
}
