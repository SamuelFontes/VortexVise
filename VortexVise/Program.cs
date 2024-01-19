using Raylib_cs;
using System.Numerics;
using System.Reflection;

int screenWidth = 800;
int screenHeight = 480;
Raylib.InitWindow(screenWidth, screenHeight, "Vortex Vise");


Texture2D player = Raylib.LoadTexture("Resources/Sprites/Skins/fatso.png");

Vector2 playerPosition = new Vector2( screenWidth/2, screenHeight/2 );
//Raylib.SetTargetFPS(60);               // TODO: Implement deltatime 


float playerVelocity = 0f;
while (!Raylib.WindowShouldClose())
{
    float deltaTime = Raylib.GetFrameTime();

    Raylib.DrawText("FPS: " + (int)(1f/deltaTime), 12, 12, 20, Color.BLACK);

    playerVelocity += 20f * deltaTime;
    playerPosition.Y += playerVelocity;
    var playerFeet = player.Height + playerPosition.Y;
    if(playerFeet > screenHeight)
    {
        playerVelocity = 0f;
        playerPosition.Y = screenHeight - player.Height;
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) playerPosition.X += 500.0f * deltaTime;
    if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
    {
        playerPosition.X -= 500.0f * deltaTime;
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.WHITE);

    Raylib.DrawTextureV(player, playerPosition, Color.WHITE);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();