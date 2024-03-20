using System.Numerics;
using VortexVise.Enums;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

enum MenuItem { None, Online, Offline, Settings, Exit, Return, PressStart };
enum MenuItemType { Button, TextInput };
enum MenuState { MainMenu, Settings, PressStart, ChooseProfile, NewProfile, Loading, InputSelection, OnlineMain, Lobby };
public static class MenuScene
{
    static List<MenuItem> menuItems = new List<MenuItem>();
    static int finishScreen = 0;
    static Texture logo;
    static Texture background;
    static Texture box;
    static Texture player;
    static Texture keyboard;
    static Texture gamepad;
    static Texture gamepadSlotOn;
    static Texture gamepadSlotOff;
    static Texture disconnected;
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
        box = Raylib.LoadTexture("resources/Common/rounded_box.png");
        keyboard = Raylib.LoadTexture("resources/Common/keyboard.png");
        gamepad = Raylib.LoadTexture("resources/Common/xbox_gamepad.png");
        disconnected = Raylib.LoadTexture("resources/Common/xbox_gamepad_disconnected.png");
        player = Raylib.LoadTexture("Resources/Sprites/Skins/fatso.png"); // TODO: make load skin, not this hardcoded crap
        gamepadSlotOn = Raylib.LoadTexture("resources/Common/gamepad_slot_on.png");
        gamepadSlotOff = Raylib.LoadTexture("resources/Common/gamepad_slot_off.png");

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
        //TODO: Create input boxes 300x200 4x4
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
                menuItems[0].IsSelected = false;
            }
        }
        else
        {
            var input = PlayerLogic.GetInput(GameCore.PlayerOneGamepad);
            if (input.Confirm || Raylib.IsGestureDetected(Gesture.GESTURE_TAP))
            {
                if (currentState == MenuState.InputSelection)
                {
                    finishScreen = 2;
                    return;
                }
                GameUserInterface.IsCursorVisible = false;
                switch (selected)
                {
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
                    case Scenes.MenuItem.Offline:
                        {

                            //finishScreen = 2;   // GAMEPLAY
                            GameCore.IsNetworkGame = false;
                            GameSounds.PlaySound(GameSounds.Click);
                            currentState = MenuState.InputSelection;
                            break;
                        }
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
            else if (input.UIDown)
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
            else if (input.UIUp)
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
        Raylib.DrawTextureEx(background, backgroundPos, 0, 1, Raylib.DARKGRAY);
        if (currentState == MenuState.PressStart || currentState == MenuState.MainMenu)
            Raylib.DrawTextureEx(logo, new Vector2(GameCore.GameScreenWidth * 0.5f - logo.width * 0.5f, GameCore.GameScreenHeight * 0.3f - logo.width * 0.5f), 0, 1, Raylib.WHITE);

        // Draw menu
        //----------------------------------------------------------------------------------
        foreach (var item in menuItems) if (item.State == currentState) item.Draw();

        // Input Selection
        if (currentState == MenuState.InputSelection)
        {
            DrawInputSelection();
        }
        if (Utils.Debug())
            Raylib.DrawRectangle(80, 0, 800, 600, new(255, 0, 0, 20));


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
        }
    }


    private static void DrawInputSelection()
    {
        Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, new(0, 0, 0, 100)); // Overlay

        Vector2 screenCenter = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f);

        // Render BoxPlayerOne
        Vector2 boxPlayerOne = new(screenCenter.X - 316, screenCenter.Y - 216);
        Raylib.DrawTextureEx(box, boxPlayerOne, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerOne, box.width, box.height, GameCore.PlayerOneGamepad, GameCore.PlayerOneProfile.Name);

        Vector2 boxPlayerTwo = new(screenCenter.X + 16, screenCenter.Y - 216);
        Raylib.DrawTextureEx(box, boxPlayerTwo, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerTwo, box.width, box.height, GameCore.PlayerTwoGamepad, GameCore.PlayerTwoProfile.Name);

        Vector2 boxPlayerThree = new(screenCenter.X - 316, screenCenter.Y + 16);
        Raylib.DrawTextureEx(box, boxPlayerThree, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerThree, box.width, box.height, GameCore.PlayerThreeGamepad, GameCore.PlayerThreeProfile.Name);

        Vector2 boxPlayerFour = new(screenCenter.X + 16, screenCenter.Y + 16);
        Raylib.DrawTextureEx(box, boxPlayerFour, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerFour, box.width, box.height, GameCore.PlayerFourGamepad, GameCore.PlayerFourProfile.Name);

        void DrawPlayerCard(Vector2 cardPosition, int cardWidth, int cardHeight, int playerGamepadNumber, string profileName)
        {
            Vector2 skinPosition = new(cardPosition.X + cardWidth * 0.3f, cardPosition.Y + cardHeight * 0.6f);
            Vector2 inputDevicePosition = new(cardPosition.X + cardWidth * 0.7f, cardPosition.Y + cardHeight * 0.7f);
            Vector2 profileNamePosition = new(cardPosition.X + cardWidth * 0.7f, cardPosition.Y + cardHeight * 0.4f);
            Vector2 gamepadSlotPostion = new(cardPosition.X + cardWidth * 0.59f, cardPosition.Y + cardHeight * 0.5f);
            if (playerGamepadNumber == -1)
            {
                // mouse and keyboard
                Raylib.DrawTextureEx(keyboard, new(inputDevicePosition.X - keyboard.width * 1f, inputDevicePosition.Y - keyboard.height * 1f), 0, 2, Raylib.WHITE);
            }
            else if (playerGamepadNumber == 0)
            {
                Raylib.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.width * 1f, inputDevicePosition.Y - gamepad.height * 1f), 0, 2, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 1f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 2f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 3f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
            }
            else if (playerGamepadNumber == 1)
            {
                Raylib.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.width * 1f, inputDevicePosition.Y - gamepad.height * 1f), 0, 2, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X + gamepadSlotOn.width * 1f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 2f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 3f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);

            }
            else if (playerGamepadNumber == 2)
            {
                Raylib.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.width * 1f, inputDevicePosition.Y - gamepad.height * 1f), 0, 2, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 1f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X + gamepadSlotOn.width * 2f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 3f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
            }
            else if (playerGamepadNumber == 3)
            {
                Raylib.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.width * 1f, inputDevicePosition.Y - gamepad.height * 1f), 0, 2, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 1f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.width * 2f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
                Raylib.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X + gamepadSlotOn.width * 3f, gamepadSlotPostion.Y), 0, 1, Raylib.WHITE);
            }
            else if (playerGamepadNumber == -9)
            {
                // Disconnected
                Vector2 disconnectedPosition = new(cardPosition.X + cardWidth * 0.5f, cardPosition.Y + cardHeight * 0.5f);
                Raylib.DrawTextureEx(disconnected, new(disconnectedPosition.X - disconnected.width * 2f, disconnectedPosition.Y - disconnected.height * 2f), 0, 4, Raylib.WHITE);
            }
            if (playerGamepadNumber != -9)
            {
                Raylib.DrawTextureEx(player, new(skinPosition.X - player.width * 2f, skinPosition.Y - player.height * 2f), 0, 4, Raylib.WHITE);
                Utils.DrawTextCentered(profileName, profileNamePosition, 12, Raylib.WHITE);
            }
        }

    }
}


