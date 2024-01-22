#include <iostream>
#include <raylib.h>
#include <raymath.h>
float roundf(float var)
{
	// 37.66666 * 100 =3766.66
	// 3766.66 + .5 =3767.16    for rounding off value
	// then type cast to int so value is 3767
	// then divided by 100 so the value converted into 37.67
	float value = (int)(var * 100 + .5);
	return (float)value / 100;
}

int main()
{
    std::cout << "Hello World!\n";
	int screenWidth = 1280;
	int screenHeight = 720;
	InitWindow(screenWidth, screenHeight, "Vortex Vise Editor");


    // Box B: Mouse moved box
    Rectangle boxB = { GetScreenWidth()/2.0f - 30, GetScreenHeight()/2.0f - 30, 60, 60 };

     // Define camera
    Camera2D camera = { 0 };
    camera.target = {0,0};
    camera.offset = { 0, 0 };
    camera.rotation = 0.0f;
    camera.zoom = 1.0f;


    // Main game loop
    while (!WindowShouldClose())    // Detect window close button or ESC key
    {
		Vector2 mouse = GetMousePosition();


        camera.zoom += ((float)GetMouseWheelMove()*0.2f);
        if (camera.zoom > 2)
            camera.zoom = 2;
        if (camera.zoom <= 0.4)
            camera.zoom = 0.4;

        mouse.x = (mouse.x / camera.zoom);
        mouse.y = (mouse.y / camera.zoom);
        boxB.x = mouse.x - boxB.width / 2;
        boxB.y = mouse.y - boxB.height / 2;
        // Draw
        //-----------------------------------------------------
        BeginDrawing();

            ClearBackground(RAYWHITE);

			BeginMode2D(camera);

            DrawRectangleRec(boxB, BLUE);

			DrawText(TextFormat("debug: %02f %02f %02f %02f %02f",camera.zoom, mouse.x, mouse.y, boxB.x, boxB.y), 0 , 0 , roundf(20 /camera.zoom) , BLACK);

        EndDrawing();

        //-----------------------------------------------------
    }

    // De-Initialization
    //---------------------------------------------------------
    CloseWindow();        // Close window and OpenGL context
    //----------------------------------------------------------

    return 0;
}
