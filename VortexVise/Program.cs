using Raylib_cs;
using System.Numerics;
using System.Reflection;
using System.Text.Encodings.Web;

int screenWidth = 1920;
int screenHeight = 1080;
Raylib.InitWindow(screenWidth, screenHeight, "Vortex Vise");
Raylib.ToggleFullscreen();


Texture2D player = Raylib.LoadTexture("Resources/Sprites/Skins/fatso.png");

Vector2 playerPosition = new Vector2( 0, 0 );
Raylib.SetTargetFPS(60);               // TODO: Implement deltatime 


float playerGravitacionalForce = 0f;
float gravity = 20f;
int playerDirection = 1;
float playerMoveSpeed = 0;
float playerMaxMoveSpeed = 600;
float playerAcceleration = 1500;

while (!Raylib.WindowShouldClose())
{
    float deltaTime = Raylib.GetFrameTime();

    Raylib.DrawText("FPS: " + (int)(1f/deltaTime), 12, 12, 20, Color.BLACK);

    playerGravitacionalForce += gravity * deltaTime;
    var playerFeet = playerPosition.Y - player.Height; 

    var screenBottom = (screenHeight / 2 * -1);
    if(playerFeet <= screenBottom )
    {
        playerGravitacionalForce = 0f;
        playerPosition.Y = screenBottom + player.Height;
    }
    playerPosition.Y -= playerGravitacionalForce;

    if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
    {
        if(playerDirection != -1)
        {
            // Player changed direction
            playerMoveSpeed = 0;
        }
        playerMoveSpeed += playerAcceleration * deltaTime;
        if (playerMoveSpeed > playerMaxMoveSpeed)
            playerMoveSpeed = playerMaxMoveSpeed;
        playerDirection = -1;
    } else if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
    {
        if(playerDirection != 1)
        {
            // Player changed direction
            playerMoveSpeed = 0;
        }
        playerMoveSpeed += playerAcceleration * deltaTime;
        if (playerMoveSpeed > playerMaxMoveSpeed)
            playerMoveSpeed = playerMaxMoveSpeed;
        playerDirection = 1;
    }
    else
    {
        playerMoveSpeed -= playerAcceleration * deltaTime * 3;  
        if(playerMoveSpeed < 0)
        {
            playerMoveSpeed = 0;
        }
    }

    if(playerDirection == 1)
    {
        playerPosition.X += playerMoveSpeed * deltaTime;
    }
    else
    {
        playerPosition.X -= playerMoveSpeed * deltaTime;
    }

    if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
    {
        playerPosition.Y += 1500 * deltaTime;
        playerGravitacionalForce = 0f;
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.WHITE);

    Rectangle sourceRec = new Rectangle( 0.0f, 0.0f, player.Width * playerDirection, player.Height);

    Rectangle destRec = new Rectangle ( screenWidth/2.0f, screenHeight/2.0f, player.Width, player.Height);

    Raylib.DrawTexturePro(player,sourceRec,destRec, playerPosition,0, Color.WHITE);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();