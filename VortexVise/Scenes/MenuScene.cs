using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

enum MainMenuItens { None, Versus, Online, Survival, Settings, Exit, Return, Connect, IP };
enum MainMenuTypes { Button, TextInput };
enum MainMenuState { StateMain = 0, StateSettings, StateOnline };
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
        GameUserInterface.DisableCursor = false;
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
        var state = MainMenuState.StateMain;
        menuItems.Add(new MenuItem("SURVIVAL MODE", MainMenuItens.Survival, state, false));
        menuItems.Add(new MenuItem("VERSUS MODE", MainMenuItens.Versus, state, true));
        menuItems.Add(new MenuItem("ONLINE", MainMenuItens.Online, state, true));
        menuItems.Add(new MenuItem("SETTINGS", MainMenuItens.Settings, state, false));
        menuItems.Add(new MenuItem("EXIT", MainMenuItens.Exit, state, true));
        state = MainMenuState.StateOnline;
        menuItems.Add(new MenuItem("127.0.0.1", MainMenuItens.IP, state, true, MainMenuTypes.TextInput));
        menuItems.Add(new MenuItem("CONNECT", MainMenuItens.Connect, state, true));
        menuItems.Add(new MenuItem("GO BACK", MainMenuItens.Return, state, true));

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
                case MainMenuItens.Exit:
                    {
                        finishScreen = -1;   // EXIT
                        GameCore.GameShouldClose = true;
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.Settings:
                    {
                        finishScreen = 1;   // OPTIONS
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.Online:
                    {
                        foreach (var item in menuItems) item.IsSelected = false;
                        currentState = MainMenuState.StateOnline;
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.Versus:
                    {
                        finishScreen = 2;   // GAMEPLAY
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.Return:
                    {
                        foreach (var item in menuItems) item.IsSelected = false;
                        currentState = MainMenuState.StateOnline;
                        currentState = MainMenuState.StateMain;
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                default: break;
            }
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
        {
            GameUserInterface.IsCursorVisible = false;
            var shouldSelectNext = false;
            foreach (var item in menuItems)
            {
                if (item.State != currentState) continue;
                if (item.IsSelected)
                {
                    shouldSelectNext = true;
                    item.IsSelected = false;
                    selected = MainMenuItens.None;
                }
                else if (shouldSelectNext && item.IsEnabled)
                {
                    shouldSelectNext = false;
                    item.IsSelected = true;
                    selected = item.Item;

                }
            }
            if (shouldSelectNext || selected == MainMenuItens.None)
            {
                var item = menuItems.Where(x => x.IsEnabled && x.State == currentState).Last();
                item.IsSelected = true; // Means the item is the last
                selected = item.Item;
            }
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
        {
            GameUserInterface.IsCursorVisible = false;
            var shouldSelectNext = false;
            for (int i = menuItems.Count - 1; i >= 0; i--)
            {
                var item = menuItems[i];
                if (item.State != currentState) continue;
                if (item.IsSelected)
                {
                    shouldSelectNext = true;
                    item.IsSelected = false;
                    selected = MainMenuItens.None;
                }
                else if (shouldSelectNext && item.IsEnabled)
                {
                    shouldSelectNext = false;
                    item.IsSelected = true;
                    selected = item.Item;
                }
            }
            if (shouldSelectNext || selected == MainMenuItens.None)
            {
                var item = menuItems.Where(x => x.IsEnabled && x.State == currentState).First();
                item.IsSelected = true; // Means the item is the first
                selected = item.Item;
            }
        }

        // Play selection sound
        //----------------------------------------------------------------------------------
        PlaySelectionSound();

        // Update menu
        //----------------------------------------------------------------------------------
        Vector2 textPosition = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.7f);
        foreach (var item in menuItems) if (item.State == currentState) item.Update(ref textPosition.X, ref textPosition.Y);
        var s = menuItems.FirstOrDefault(x => x.IsSelected);
        if (s == null) selected = MainMenuItens.None;
        else selected = s.Item;
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
        if (lastSelected != selected && selected != MainMenuItens.None) GameSounds.PlaySound(GameSounds.Selection);
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
            Type = MainMenuTypes.Button;
        }
        public MenuItem(string text, MainMenuItens item, MainMenuState state, bool isEnabled, MainMenuTypes type)
        {
            Text = text;
            Item = item;
            State = state;
            IsEnabled = isEnabled;
            Size = 32;
            IsSelected = false;
            Type = type;
        }
        public string Text;
        public MainMenuItens Item { get; set; }
        public MainMenuState State { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSelected { get; set; }
        public Vector2 Position { get; set; }
        public int Size { get; set; }
        Color Color { get; set; }
        public MainMenuTypes Type { get; set; }
        Vector2 TextSize;
        public void Update(ref float x, ref float y)
        {
            if(Type == MainMenuTypes.TextInput && IsSelected)
            {
                Utils.UpdateTextUsingKeyboard(ref Text);
            }


            // Center the text
            TextSize = Raylib.MeasureTextEx(GameCore.Font, Text, Size, 0);
            var pos = new Vector2(x - TextSize.X * 0.5f, y - TextSize.Y * 0.5f); // Centers text

            // Check if mouse is selecting the menu
            if (IsEnabled && GameUserInterface.IsCursorVisible && Raylib.CheckCollisionRecs(new Rectangle(pos.X, pos.Y, TextSize.X, TextSize.Y), new Rectangle(GameUserInterface.CursorPosition.X, GameUserInterface.CursorPosition.Y, 1, 1)))
            {
                IsSelected = true;
            }
            else if (GameUserInterface.IsCursorVisible || selected != Item)
            {
                IsSelected = false;
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
            // Draw input box
            if (Type == MainMenuTypes.TextInput)
                Raylib.DrawRectangle((int)Position.X -4,(int)Position.Y-2,(int)TextSize.X+8,(int)TextSize.Y+4,new(0,0,0,100));
            // Draw the text
            Raylib.DrawTextEx(GameCore.Font, Text, Position, Size, 0, Color);
        }
    }

}


