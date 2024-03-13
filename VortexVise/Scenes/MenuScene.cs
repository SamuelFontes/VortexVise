using System.Numerics;
using VortexVise.GameGlobals;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

enum MainMenuItens { MENU_NONE = 0, MENU_VERSUS, MENU_ONLINE, MENU_SURVIVAL, MENU_SETTINGS, MENU_EXIT, MENU_RETURN };
enum MainMenuState { MENU_STATE_MAIN = 0, MENU_STATE_SETTINGS, MENU_STATE_ONLINE };
public static class MenuScene
{
    static List<MenuItem> menuItems = new List<MenuItem>();
    static int finishScreen = 0;
    static Texture2D logo;
    static Texture2D background;
    static MainMenuItens selected;
    static MainMenuItens lastSelected;
    static MainMenuState currentState;


    static public void InitMenuScene()
    {
        // Initialize menu
        //----------------------------------------------------------------------------------
        finishScreen = 0;
        //ResetMenu();

        // Load textures
        //----------------------------------------------------------------------------------
        logo = Raylib.LoadTexture("Resources/Common/vortex-vise-logo.png");
        background = Raylib.LoadTexture("resources/Common/MenuBackground.png");

        // Initialize items
        //----------------------------------------------------------------------------------
        menuItems.Add(new MenuItem("SURVIVAL MODE", MainMenuItens.MENU_SURVIVAL, MainMenuState.MENU_STATE_MAIN, false));
        menuItems.Add(new MenuItem("VERSUS MODE", MainMenuItens.MENU_VERSUS, MainMenuState.MENU_STATE_MAIN, true));
        menuItems.Add(new MenuItem("ONLINE", MainMenuItens.MENU_ONLINE, MainMenuState.MENU_STATE_MAIN, false));
        menuItems.Add(new MenuItem("SETTINGS", MainMenuItens.MENU_SETTINGS, MainMenuState.MENU_STATE_MAIN, false));
        menuItems.Add(new MenuItem("EXIT", MainMenuItens.MENU_EXIT, MainMenuState.MENU_STATE_MAIN, true));

    }

    static public void UpdateMenuScene()
    {
        // Handle inputs
        //----------------------------------------------------------------------------------
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) || Raylib.IsGestureDetected(Gesture.GESTURE_TAP))
        {
            GameUserInterface.IsCursorVisible = false;
            switch (selected)
            {
                case MainMenuItens.MENU_EXIT:
                    {
                        finishScreen = -1;   // EXIT
                        GameCore.GameShouldClose = true;
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.MENU_SETTINGS:
                    {
                        finishScreen = 1;   // OPTIONS
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.MENU_VERSUS:
                    {
                        finishScreen = 2;   // GAMEPLAY
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.MENU_RETURN:
                    {
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                default: break;
            }
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_S) || Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
        {
            GameUserInterface.IsCursorVisible = false;
            switch (selected)
            {
                case MainMenuItens.MENU_NONE: selected = MainMenuItens.MENU_VERSUS; break;
                case MainMenuItens.MENU_VERSUS: selected = MainMenuItens.MENU_EXIT; break;
                case MainMenuItens.MENU_EXIT: selected = MainMenuItens.MENU_VERSUS; break;
                default: break;
            }
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_W) || Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
        {
            GameUserInterface.IsCursorVisible = false;
            switch (selected)
            {
                case MainMenuItens.MENU_NONE: selected = MainMenuItens.MENU_VERSUS; break;
                case MainMenuItens.MENU_VERSUS: selected = MainMenuItens.MENU_EXIT; break;
                case MainMenuItens.MENU_EXIT: selected = MainMenuItens.MENU_VERSUS; break;
                default: break;
            }
        }

        // Play selection sound
        //----------------------------------------------------------------------------------
        PlaySelectionSound();
        Vector2 textPosition = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.7f);
        foreach (var item in menuItems) if (item.State == currentState) item.Update(ref textPosition.X, ref textPosition.Y);
    }

    static public void DrawMenuScene()
    {
        // Draw Background and Logo
        //----------------------------------------------------------------------------------
        Vector2 backgroundPos = new(0, 0); // Can use this to move the background around
        Raylib.DrawTextureEx(background, backgroundPos, 0, 1, Raylib.DARKGRAY); // FIXME: this should scale like the camera zoom
        Raylib.DrawTextureEx(logo, new Vector2(GameCore.GameScreenWidth * 0.5f - logo.width * 0.5f, GameCore.GameScreenHeight * 0.3f - logo.width * 0.5f), 0, 1, Raylib.WHITE); // TODO: Create a logo 

        // Draw menu
        //----------------------------------------------------------------------------------
        foreach (var item in menuItems) if (item.State == currentState) item.Draw();


        // Play selection sound
        //----------------------------------------------------------------------------------
        PlaySelectionSound();
    }

    static public void UnloadMenuScene()
    {
    }
    static public int FinishMenuScene()
    {
        return finishScreen;
    }

    static void PlaySelectionSound()
    {
        // Play selection sound when change selection
        //----------------------------------------------------------------------------------
        if (lastSelected != selected && selected != MainMenuItens.MENU_NONE) GameSounds.PlaySound(GameSounds.Selection);
        lastSelected = selected;
    }
    class MenuItem
    {
        public MenuItem(string text, MainMenuItens item, MainMenuState state, bool isEnabled)
        {
            Text = text;
            Item = item;
            State = state;
            IsEnabled = isEnabled;
            Size = 32;
            IsSelected = false;
        }
        public string Text { get; set; }
        public MainMenuItens Item { get; set; }
        public MainMenuState State { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSelected { get; set; }
        public Vector2 Position { get; set; }
        public int Size { get; set; }
        Color Color { get; set; }
        public void Update(ref float x, ref float y)
        {
            // Center the text
            Vector2 textSize = Raylib.MeasureTextEx(GameCore.Font, Text, Size, 0);
            var pos = new Vector2(x - textSize.X * 0.5f, y - textSize.Y * 0.5f); // Centers text

            // Check if mouse is selecting the menu
            if (IsEnabled && GameUserInterface.IsCursorVisible && Raylib.CheckCollisionRecs(new Rectangle(pos.X, pos.Y, textSize.X, textSize.Y), new Rectangle(GameUserInterface.CursorPosition.X, GameUserInterface.CursorPosition.Y, 1, 1)))
            {
                IsSelected = true;
                selected = Item;
            }
            else
            {
                IsSelected = false;
                selected = MainMenuItens.MENU_NONE;
            }

            // Paint the text
            if (!IsEnabled) Color = Raylib.GRAY;
            else if (IsSelected) Color = Raylib.ORANGE;
            else Color = Raylib.RAYWHITE;

            Position = pos;

            y += Size;
        }

        public void Draw()
        {
            // Draw the text
            Raylib.DrawTextEx(GameCore.Font, Text, Position, Size, 0, Color);
        }
    }

}


