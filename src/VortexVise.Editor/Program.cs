// See https://aka.ms/new-console-template for more information
using System.Numerics;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Models;
using ZeroElectric.Vinculum;
using VortexVise.Core.Services;
using VortexVise.Core.GameGlobals;
using System.Text.Json.Nodes;
using System.Text.Json;
using VortexVise.Editor;

// Initialize Services
var assetService = new AssetService();
int screenWidth = 1280;
int screenHeight = 720;
Raylib.InitWindow(screenWidth, screenHeight, "Vortex Vise Editor");

GameAssets.Gameplay.LoadMaps(assetService);
IOrderedEnumerable<Map> maps = GameAssets.Gameplay.Maps.OrderBy(x => x.Name);
start:
var mapId = 0;
foreach (var m in GameAssets.Gameplay.Maps)
{
    Console.WriteLine($"{mapId} - {m.TextureLocation}");
    mapId++;
}
Console.WriteLine($"-1 - EXIT");
int selected = Convert.ToInt32(Console.ReadLine());
if (selected == -1) return;
Map map = GameAssets.Gameplay.Maps[selected];


// THIS CODE IS SHIT, IT WAS DONE REALLY FAST
static float roundf(float var)
{
    // 37.66666 * 100 =3766.66
    // 3766.66 + .5 =3767.16    for rounding off value
    // then type cast to int so value is 3767
    // then divided by 100 so the value converted into 37.67
    float value = (int)(var * 100 + .5);
    return (float)value / 100;
}


// Box B: Mouse moved box
Rectangle cursorRec = new(Raylib.GetScreenWidth() / 2.0f - 30, Raylib.GetScreenHeight() / 2.0f - 30, 16, 16);

// Define camera
Camera2D camera = new()
{
    target = new(0, 0),
    offset = new(0, 0),
    rotation = 0.0f,
    zoom = 1.0f
};

var mapTexture = Raylib.LoadTexture(map.TextureLocation);
var mouseTexture = Raylib.LoadTexture("Resources/Common/cursor.png");

Vector2 oldMousePosition = new(0, 0);
Vector2 mapPos = new(0, 0);
Raylib.HideCursor();

int state = 0;
bool isDrawing = false;
System.Drawing.Rectangle selection = new();
// Main game loop
while (!Raylib.WindowShouldClose())    // Detect window close button or ESC key
{
    Vector2 mouse = Raylib.GetMousePosition();


    camera.zoom += ((float)Raylib.GetMouseWheelMove() * 0.2f);
    if (camera.zoom > 2)
        camera.zoom = 2;
    if (camera.zoom <= 0.4)
        camera.zoom = 0.4f;

    mouse.X /= camera.zoom;
    mouse.Y /= camera.zoom;
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
    if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) state = 4; // Delete

    Color color = Raylib.WHITE;
    if (state == 0) color = Raylib.BLUE;
    if (state == 1) color = Raylib.GREEN;
    if (state == 2) color = Raylib.RED;
    if (state == 3) color = Raylib.PURPLE;
    if (state == 4) color = Raylib.DARKPURPLE;

    var mapX = (int)mapPos.X - (int)(mapTexture.width * 0.5f);
    var mapY = (int)mapPos.Y - (int)(mapTexture.height * 0.5f);
    var mapCursorX = mouse.X - mapX;
    var mapCursorY = mouse.Y - mapY;

    if (isDrawing && state != 0) isDrawing = false;
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
    {
        if (state == 0)
        {
            if (!isDrawing)
            {
                selection.X = (int)mapCursorX;
                selection.Y = (int)mapCursorY;
                isDrawing = true;
            }
            else
            {
                if (!(selection.X < 0 || selection.Y < 0 || selection.Width < 0 || selection.Height < 0))
                    map.Collisions.Add(selection);
                isDrawing = false;
            }

        }
        else if (state == 1)
            map.PlayerSpawnPoints.Add(new(mapCursorX - 16, mapCursorY - 16));
        else if (state == 2)
            map.EnemySpawnPoints.Add(new(mapCursorX - 16, mapCursorY - 16));
        else if (state == 3)
            map.ItemSpawnPoints.Add(new(mapCursorX - 16, mapCursorY - 16));
        else if (state == 4)
        {
            Rectangle rec = new(0, 0, 1, 1)
            {
                x = mapCursorX,
                y = mapCursorY
            };
            var index = 0;
            foreach (var c in map.Collisions)
            {
                //if (Raylib.CheckCollisionRecs(c, rec))
                //{
                    //break;
                //}
                index++;

            }
            if (index < map.Collisions.Count) map.Collisions.RemoveAt(index);

            index = 0;
            foreach (var c in map.PlayerSpawnPoints)
            {
                if (Raylib.CheckCollisionRecs(new(c.X, c.Y, 32, 32), rec))
                    break;
                index++;

            }
            if (index < map.PlayerSpawnPoints.Count) map.PlayerSpawnPoints.RemoveAt(index);

            index = 0;
            foreach (var c in map.EnemySpawnPoints)
            {
                if (Raylib.CheckCollisionRecs(new(c.X, c.Y, 32, 32), rec))
                    break;
                index++;

            }
            if (index < map.EnemySpawnPoints.Count) map.EnemySpawnPoints.RemoveAt(index);

            index = 0;
            foreach (var c in map.ItemSpawnPoints)
            {
                if (Raylib.CheckCollisionRecs(new(c.X, c.Y, 32, 32), rec))
                    break;
                index++;

            }
            if (index < map.ItemSpawnPoints.Count) map.ItemSpawnPoints.RemoveAt(index);
        }

    }
    if (isDrawing) selection = new((int)selection.X, (int)selection.Y, (int)mapCursorX - (int)selection.X, (int)mapCursorY - (int)selection.Y);


    // SAVE
    if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && Raylib.IsKeyPressed(KeyboardKey.KEY_S))
    {
        //Console.WriteLine($"Choose Map Name(Leave empty to use \"{map.Name}\"):");
        //var mapName = Console.ReadLine();
        //if (mapName == null || mapName.Length == 0) mapName = map.Name;
        var mapName = map.Name;
        string collisions = "";
        foreach (var c in map.Collisions) collisions += $"{(int)c.X},{(int)c.Y},{(int)c.Width},{(int)c.Height};";
        string playerSpawn = "";
        foreach (var c in map.PlayerSpawnPoints) playerSpawn += $"{(int)c.X},{(int)c.Y};";
        string enemySpawn = "";
        foreach (var c in map.EnemySpawnPoints) enemySpawn += $"{(int)c.X},{(int)c.Y};";
        string itemSpawn = "";
        foreach (var c in map.ItemSpawnPoints) itemSpawn += $"{(int)c.X},{(int)c.Y};";



        string save = $@"[VortexViseMap]
NAME={mapName}
COLLISIONS={collisions}
PLAYER_SPAWN={playerSpawn}
ENEMY_SPAWN={enemySpawn}
ITEM_SPAWN={itemSpawn}
GAME_MODES = DM,TDM,SURVIVAL";

        save = JsonSerializer.Serialize(map);
        Console.WriteLine(save);
    }

    // Draw
    //-----------------------------------------------------
    Raylib.BeginDrawing();

    Raylib.ClearBackground(Raylib.RAYWHITE);


    Raylib.BeginMode2D(camera);
    Raylib.DrawTexture(mapTexture, mapX, mapY, Raylib.WHITE);
    if (isDrawing)
    {
        Raylib.DrawRectangle((int)selection.X + (int)mapX, (int)selection.Y + (int)mapY, (int)selection.Width, (int)selection.Height, Raylib.BLUE);
    }
    foreach (var rec in map.Collisions) Raylib.DrawRectangle((int)rec.X + mapX, (int)rec.Y + mapY, (int)rec.Width, (int)rec.Height, Raylib.BLUE);
    foreach (var rec in map.PlayerSpawnPoints) Raylib.DrawRectangle((int)rec.X + mapX, (int)rec.Y + mapY, 32, 32, Raylib.GREEN);
    foreach (var rec in map.EnemySpawnPoints) Raylib.DrawRectangle((int)rec.X + mapX, (int)rec.Y + mapY, 32, 32, Raylib.RED);
    foreach (var rec in map.ItemSpawnPoints) Raylib.DrawRectangle((int)rec.X + mapX, (int)rec.Y + mapY, 32, 32, Raylib.PURPLE);


    Raylib.DrawTexturePro(mouseTexture, new(0, 0, mouseTexture.width, mouseTexture.height), cursorRec, new(0, 0), 0, color);

    Raylib.DrawText($"debug: {state} {mouse.X - mapX} {mouse.Y - mapY} {(int)mapCursorX - (int)selection.X}", 0, 0, (int)roundf(20 / camera.zoom), Raylib.BLACK);

    Raylib.EndDrawing();

    //-----------------------------------------------------
}

// De-Initialization
//---------------------------------------------------------
Raylib.CloseWindow();        // Close window and OpenGL context
                             //----------------------------------------------------------
goto start;

