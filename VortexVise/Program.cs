using Raylib_cs;
using System.Numerics;
using System.Reflection;

int screenWidth = 1920;
int screenHeight = 1080;
Raylib.InitWindow(screenWidth, screenHeight, "Vortex Vise");
Raylib.ToggleFullscreen();


Texture2D player = Raylib.LoadTexture("Resources/Sprites/Skins/fatso.png");

Vector2 playerPosition = new Vector2( screenWidth/2, screenHeight/2 );
//Raylib.SetTargetFPS(60);               // TODO: Implement deltatime 

// Source rectangle (part of the texture to use for drawing)

// Destination rectangle (screen rectangle where drawing part of texture)

float playerGravitacionalForce = 0f;
int playerDirection = 1;
float playerMoveSpeed = 450;
while (!Raylib.WindowShouldClose())
{
    float deltaTime = Raylib.GetFrameTime();

    Raylib.DrawText("FPS: " + (int)(1f/deltaTime), 12, 12, 20, Color.BLACK);

    playerGravitacionalForce += 20f * deltaTime;
    playerPosition.Y -= playerGravitacionalForce;
    var playerFeet = player.Height + playerPosition.Y;

    if(playerFeet > (screenHeight / 2 * -1)  )
    {
        playerGravitacionalForce = 0f;
        playerPosition.Y = (screenHeight / 2 * -1)+ player.Height;
    }

    if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
    {
        playerPosition.X -= playerMoveSpeed * deltaTime;
        playerDirection = -1;
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
    {
        playerPosition.X += playerMoveSpeed * deltaTime;
        playerDirection = 1;
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.WHITE);

    Rectangle sourceRec = new Rectangle( 0.0f, 0.0f, player.Width * playerDirection, player.Height);

    Rectangle destRec = new Rectangle ( screenWidth/2.0f, screenHeight/2.0f, player.Width, player.Height);

    Raylib.DrawTexturePro(player,sourceRec,destRec, playerPosition,0, Color.WHITE);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();