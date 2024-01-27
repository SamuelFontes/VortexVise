// See https://aka.ms/new-console-template for more information
using Raylib_cs;
using System.Numerics;

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
Rectangle boxB = new(Raylib.GetScreenWidth() / 2.0f - 30, Raylib.GetScreenHeight() / 2.0f - 30, 60, 60);

// Define camera
Camera2D camera = new();
camera.Target = new(0, 0);
camera.Offset = new(0, 0);
camera.Rotation = 0.0f;
camera.Zoom = 1.0f;


// Main game loop
while (!Raylib.WindowShouldClose())    // Detect window close button or ESC key
{
    Vector2 mouse = Raylib.GetMousePosition();


    camera.Zoom += ((float)Raylib.GetMouseWheelMove() * 0.2f);
    if (camera.Zoom > 2)
        camera.Zoom = 2;
    if (camera.Zoom <= 0.4)
        camera.Zoom = 0.4f;

    mouse.X = (mouse.X / camera.Zoom);
    mouse.Y = (mouse.Y / camera.Zoom);
    boxB.X = mouse.X - boxB.Width / 2;
    boxB.Y = mouse.Y - boxB.Height / 2;
    // Draw
    //-----------------------------------------------------
    Raylib.BeginDrawing();

    Raylib.ClearBackground(Color.RayWhite);

    Raylib.BeginMode2D(camera);

    Raylib.DrawRectangleRec(boxB, Color.Blue);

    Raylib.DrawText($"debug: {camera.Zoom} {mouse.X} {mouse.Y} {boxB.X} {boxB.Y}", 0, 0, (int)roundf(20 / camera.Zoom), Color.Black);

    Raylib.EndDrawing();

    //-----------------------------------------------------
}

// De-Initialization
//---------------------------------------------------------
Raylib.CloseWindow();        // Close window and OpenGL context
                      //----------------------------------------------------------

