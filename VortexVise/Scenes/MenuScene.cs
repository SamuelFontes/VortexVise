using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

enum MenuItem { None, Online, Offline, Settings, Exit, Return, PressStart };
enum MenuItemType { Button, TextInput };
enum MenuState { MainMenu, Settings, PressStart, ChooseProfile, NewProfile, Loading, Input, OnlineMain, Lobby };
public static class MenuScene
{
    static List<MenuItem> menuItems = new List<MenuItem>();
    static int finishScreen = 0;
    static Texture logo;
    static Texture background;
    static Scenes.MenuItem selected;
    static Scenes.MenuItem lastSelected;
    static MenuState currentState;
    static bool IsOnline = false;


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
        var state = MenuState.PressStart;
        menuItems.Add(new MenuItem("PRESS START", Scenes.MenuItem.PressStart, state, true)); // TODO: Change how this one workd
        menuItems[0].IsEnabled = true;
        selected = menuItems[0].Item;
        currentState = state;
        state = MenuState.MainMenu;
        menuItems.Add(new MenuItem("LOCAL PLAY", Scenes.MenuItem.Offline, state, true));
        menuItems.Add(new MenuItem("ONLINE", Scenes.MenuItem.Online, state, true));
        menuItems.Add(new MenuItem("SETTINGS", Scenes.MenuItem.Settings, state, false));
        menuItems.Add(new MenuItem("EXIT", Scenes.MenuItem.Exit, state, true));
        state = MenuState.Lobby;
        //menuItems.Add(new MenuItem("127.0.0.1", MainMenuItens.IP, state, true, MainMenuTypes.TextInput));
        menuItems.Add(new MenuItem("GO BACK", Scenes.MenuItem.Return, state, true));

    }

    static public void UpdateMenuScene()
    {
        if (currentState == MenuState.PressStart)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) || Raylib.IsGestureDetected(Gesture.GESTURE_TAP))
                GameCore.PlayerOneGamepad = -1; // Mouse and keyboard
            else if (Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneGamepad = 0;
            else if (Raylib.IsGamepadButtonPressed(1, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(1, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneGamepad = 1;
            else if (Raylib.IsGamepadButtonPressed(2, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(2, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneGamepad = 2;
            else if (Raylib.IsGamepadButtonPressed(3, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(3, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneGamepad = 3;
            if (GameCore.PlayerOneGamepad != -9)
            {
                GameSounds.PlaySound(GameSounds.Click, pitch: 0.8f);
                currentState = MenuState.MainMenu;
                selected = Scenes.MenuItem.None;
            }
        }
        else
        {
            var input = PlayerLogic.GetInput(GameCore.PlayerOneGamepad);
            if (input.Confirm || Raylib.IsGestureDetected(Gesture.GESTURE_TAP))
            {
                GameUserInterface.IsCursorVisible = false;
                switch (selected)
                {
                    case Scenes.MenuItem.PressStart:
                        {

                            break;
                        }
                    case Scenes.MenuItem.Exit:
                        {
                            finishScreen = -1;   // EXIT
                            GameCore.GameShouldClose = true;
                            GameSounds.PlaySound(GameSounds.Click);
                            break;
                        }
                    case Scenes.MenuItem.Settings:
                        {
                            finishScreen = 1;   // OPTIONS
                            GameSounds.PlaySound(GameSounds.Click);
                            break;
                        }
                    /*                case Scenes.MenuItem.Online:
                                        {
                                            foreach (var item in menuItems) item.IsSelected = false;
                                            currentState = MenuState.StateOnline;
                                            GameSounds.PlaySound(GameSounds.Click);
                                            break;
                    }
                                    /*                case Scenes.MenuItem.Versus:
                                                        {
                                                            finishScreen = 2;   // GAMEPLAY
                                                            GameCore.IsNetworkGame = false;
                                                            GameSounds.PlaySound(GameSounds.Click);
                                                            break;
                                                        }
                                    */
                    case Scenes.MenuItem.Return:
                        {
                            foreach (var item in menuItems) item.IsSelected = false;
                            currentState = MenuState.MainMenu;
                            GameSounds.PlaySound(GameSounds.Click);
                            break;
                        }
                    default: break;
                }
            }
            else if (input.Down)
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
                        selected = Scenes.MenuItem.None;
                    }
                    else if (shouldSelectNext && item.IsEnabled)
                    {
                        shouldSelectNext = false;
                        item.IsSelected = true;
                        selected = item.Item;

                    }
                }
                if (shouldSelectNext || selected == Scenes.MenuItem.None)
                {
                    var item = menuItems.Where(x => x.IsEnabled && x.State == currentState).Last();
                    item.IsSelected = true; // Means the item is the last
                    selected = item.Item;
                }
            }
            else if (input.Up)
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
                        selected = Scenes.MenuItem.None;
                    }
                    else if (shouldSelectNext && item.IsEnabled)
                    {
                        shouldSelectNext = false;
                        item.IsSelected = true;
                        selected = item.Item;
                    }
                }
                if (shouldSelectNext || selected == Scenes.MenuItem.None)
                {
                    var item = menuItems.Where(x => x.IsEnabled && x.State == currentState).First();
                    item.IsSelected = true; // Means the item is the first
                    selected = item.Item;
                }
            }


        }

        // Handle inputs
        //----------------------------------------------------------------------------------
        // Play selection sound
        //----------------------------------------------------------------------------------
        PlaySelectionSound();

        // Update menu
        //----------------------------------------------------------------------------------
        Vector2 textPosition = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.7f);
        foreach (var item in menuItems) if (item.State == currentState) item.Update(ref textPosition.X, ref textPosition.Y);
        var s = menuItems.FirstOrDefault(x => x.IsSelected);
        if (s == null) selected = Scenes.MenuItem.None;
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
        if (lastSelected != selected && selected != Scenes.MenuItem.None) GameSounds.PlaySound(GameSounds.Selection);
        lastSelected = selected;
    }
    class MenuItem
    {
        public MenuItem(string text, Scenes.MenuItem item, MenuState state, bool isEnabled)
        {
            Text = text;
            Item = item;
            State = state;
            IsEnabled = isEnabled;
            Size = 32;
            IsSelected = false;
            Type = MenuItemType.Button;
        }
        public MenuItem(string text, Scenes.MenuItem item, MenuState state, bool isEnabled, MenuItemType type)
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
        public Scenes.MenuItem Item { get; set; }
        public MenuState State { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSelected { get; set; }
        public Vector2 Position { get; set; }
        public int Size { get; set; }
        Color Color { get; set; }
        public MenuItemType Type { get; set; }
        Vector2 TextSize;
        public void Update(ref float x, ref float y)
        {
            if (Type == MenuItemType.TextInput && IsSelected)
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

            // Make press start always selected
            if (currentState == MenuState.PressStart && Item == Scenes.MenuItem.PressStart)
            {
                IsSelected = true;
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
            if (Type == MenuItemType.TextInput)
                Raylib.DrawRectangle((int)Position.X - 4, (int)Position.Y - 2, (int)TextSize.X + 8, (int)TextSize.Y + 4, new(0, 0, 0, 100));
            // Draw the text
            Raylib.DrawTextEx(GameCore.Font, Text, Position, Size, 0, Color);
            if (Utils.Debug())
                Raylib.DrawRectangle(80, 0, 800, 600, new(0, 0, 0, 20));
        }
    }

}


