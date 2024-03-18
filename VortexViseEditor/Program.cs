// See https://aka.ms/new-console-template for more information
using System.Numerics;
using VortexVise.Models;
using ZeroElectric.Vinculum;

// THIS CODE IS SHIT, IT WAS DONE REALLY FAST
float roundf(float var)
{
    // 37.66666 * 100 =3766.66
    // 3766.66 + .5 =3767.16    for rounding off value
    // then type cast to int so value is 3767
    // then divided by 100 so the value converted into 37.67
    float value = (int)(var * 100 + .5);
    return (float)value / 100;
}
int screenWidth = 1280;
int screenHeight = 720;
Raylib.InitWindow(screenWidth, screenHeight, "Vortex Vise Editor");


// Box B: Mouse moved box
Rectangle cursorRec = new(Raylib.GetScreenWidth() / 2.0f - 30, Raylib.GetScreenHeight() / 2.0f - 30, 16, 16);

// Define camera
Camera2D camera = new();
camera.target = new(0, 0);
camera.offset = new(0, 0);
camera.rotation = 0.0f;
camera.zoom = 1.0f;

var mapname = "CookhouseShootout";
var mapTexture = Raylib.LoadTexture("C:/code/personal/VortexVise/VortexVise/Resources/Maps/" + mapname + ".png");
var mouseTexture = Raylib.LoadTexture("C:\\code\\personal\\VortexVise\\VortexVise\\Resources\\Common\\cursor.png");

Vector2 oldMousePosition = new(0, 0);
Vector2 mapPos = new(0, 0);
Map map = new Map();
Raylib.HideCursor();

int state = 0;
bool isDrawing = false;
Rectangle selection = new();
// Main game loop
while (!Raylib.WindowShouldClose())    // Detect window close button or ESC key
{
    Vector2 mouse = Raylib.GetMousePosition();


    camera.zoom += ((float)Raylib.GetMouseWheelMove() * 0.2f);
    if (camera.zoom > 2)
        camera.zoom = 2;
    if (camera.zoom <= 0.4)
        camera.zoom = 0.4f;

    mouse.X = (mouse.X / camera.zoom);
    mouse.Y = (mouse.Y / camera.zoom);
    cursorRec.X = mouse.X;
    cursorRec.Y = mouse.Y;


    if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE))
    {
        mapPos.X -= oldMousePosition.X - mouse.X;
        mapPos.Y -= oldMousePosition.Y - mouse.Y;

    }
    oldMousePosition = mouse;

    // Mouse modes
    if (Raylib.IsKeyDown(KeyboardKey.KEY_Q)) state = 0; // Collision
    if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) state = 1; // PlayerSpawn
    if (Raylib.IsKeyDown(KeyboardKey.KEY_E)) state = 2; // EnemySpawn
    if (Raylib.IsKeyDown(KeyboardKey.KEY_R)) state = 3; // ItemSpawn

    Color color = Raylib.WHITE;
    if (state == 0) color = Raylib.BLUE;
    if (state == 1) color = Raylib.GREEN;
    if (state == 2) color = Raylib.RED;
    if (state == 3) color = Raylib.PURPLE;

    var mapX = (int)mapPos.X - (int)(mapTexture.width * 0.5f);
    var mapY = (int)mapPos.Y - (int)(mapTexture.height * 0.5f);
    var mapCursorX = mouse.X - mapX;
    var mapCursorY = mouse.Y - mapY;

    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
    {
        if (state == 0)
        {
            if (!isDrawing)
            {
                selection.x = mapCursorX;
                selection.y = mapCursorY;
                isDrawing = true;
            }
            else
            {
                map.Collisions.Add(selection);
                isDrawing = false;
            }

        }

    }
    if (isDrawing) selection = new((int)selection.x, (int)selection.y, (int)mapCursorX - (int)selection.x, (int)mapCursorY - (int)selection.y);

    // Draw
    //-----------------------------------------------------
    Raylib.BeginDrawing();

    Raylib.ClearBackground(Raylib.RAYWHITE);


    Raylib.BeginMode2D(camera);
    Raylib.DrawTexture(mapTexture, mapX, mapY, Raylib.WHITE);
    if (isDrawing)
    {
        Raylib.DrawRectangle((int)selection.x + (int)mapX, (int)selection.y + (int)mapY, (int)selection.width, (int)selection.height, Raylib.BLUE);
    }
    foreach(var rec in map.Collisions) Raylib.DrawRectangle((int)rec.X + mapX, (int)rec.Y + mapY, (int)rec.Width, (int)rec.Height, Raylib.BLUE);


    Raylib.DrawTexturePro(mouseTexture, new(0, 0, mouseTexture.width, mouseTexture.height), cursorRec, new(0, 0), 0, color);

    Raylib.DrawText($"debug: {state} {mouse.X - mapX} {mouse.Y - mapY} {(int)mapCursorX - (int)selection.x}", 0, 0, (int)roundf(20 / camera.zoom), Raylib.BLACK);

    Raylib.EndDrawing();

    //-----------------------------------------------------
}

// De-Initialization
//---------------------------------------------------------
Raylib.CloseWindow();        // Close window and OpenGL context
                             //----------------------------------------------------------

