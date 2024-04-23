﻿using System.ComponentModel.Design;
using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

enum MenuItem { None, Online, Arcade, Settings, Exit, Return, PressStart };
enum MenuItemType { Button, TextInput };
enum MenuState { MainMenu, Settings, PressStart, ChooseProfile, NewProfile, Loading, InputSelection, OnlineMain, Lobby };
/// <summary>
/// Main Menu Scene
/// </summary>
public static class MenuScene
{
    static readonly List<MenuItem> menuItems = [];
    static int finishScreen = 0;
    static Texture logo;
    static Texture background;
    static Texture box;
    static Texture keyboard;
    static Texture gamepad;
    static Texture gamepadSlotOn;
    static Texture gamepadSlotOff;
    static Texture disconnected;
    static Texture arrow;
    static Scenes.MenuItem selected;
    static Scenes.MenuItem lastSelected;
    static MenuState currentState;
    static float arrowAnimationTimer = 0;
    static bool arrowExpanding = true;
    //static bool IsOnline = false;


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
        gamepadSlotOn = Raylib.LoadTexture("resources/Common/gamepad_slot_on.png");
        gamepadSlotOff = Raylib.LoadTexture("resources/Common/gamepad_slot_off.png");
        arrow = Raylib.LoadTexture("resources/Common/arrow.png");

        // Load player skins
        //----------------------------------------------------------------------------------
        GameCore.PlayerOneProfile.Skin = GameAssets.Gameplay.Skins.First();
        GameCore.PlayerTwoProfile.Skin = GameAssets.Gameplay.Skins.First();
        GameCore.PlayerThreeProfile.Skin = GameAssets.Gameplay.Skins.First();
        GameCore.PlayerFourProfile.Skin = GameAssets.Gameplay.Skins.First();

        // Initialize items
        //----------------------------------------------------------------------------------
        var state = MenuState.PressStart;
        menuItems.Add(new MenuItem("PRESS START", Scenes.MenuItem.PressStart, state, true)); // TODO: Change how this one workd
        menuItems[0].IsEnabled = true;
        selected = menuItems[0].Item;
        currentState = state;
        state = MenuState.MainMenu;
        menuItems.Add(new MenuItem("CAMPAIGN", Scenes.MenuItem.None, state, false));
        menuItems.Add(new MenuItem("ARCADE", Scenes.MenuItem.Arcade, state, true));
        menuItems.Add(new MenuItem("ONLINE", Scenes.MenuItem.Online, state, false));
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
                GameCore.PlayerOneProfile.Gamepad = -1; // Mouse and keyboard
            else if (Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneProfile.Gamepad = 0;
            else if (Raylib.IsGamepadButtonPressed(1, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(1, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneProfile.Gamepad = 1;
            else if (Raylib.IsGamepadButtonPressed(2, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(2, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneProfile.Gamepad = 2;
            else if (Raylib.IsGamepadButtonPressed(3, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(3, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                GameCore.PlayerOneProfile.Gamepad = 3;
            if (GameCore.PlayerOneProfile.Gamepad != -9)
            {
                GameUserInterface.IsCursorVisible = false;
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click, pitch: 0.8f);
                currentState = MenuState.MainMenu;
                selected = Scenes.MenuItem.Arcade;
                menuItems.First(x => x.Item == selected).IsSelected = true;
                menuItems[0].IsSelected = false;
            }
        }
        else
        {
            var input = GameInput.GetInput(GameCore.PlayerOneProfile.Gamepad);
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
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                        break;
                    }
                    case Scenes.MenuItem.Settings:
                    {
                        finishScreen = 1;   // OPTIONS
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                        break;
                    }
                    case Scenes.MenuItem.Arcade:
                    {

                        //finishScreen = 2;   // GAMEPLAY
                        GameCore.IsNetworkGame = false;
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                        currentState = MenuState.InputSelection;
                        break;
                    }
                    case Scenes.MenuItem.Return:
                    {
                        foreach (var item in menuItems) item.IsSelected = false;
                        currentState = MenuState.MainMenu;
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
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
            else if (input.Back)
            {
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click, pitch: 0.5f);
                currentState = MenuState.PressStart;
                var m = menuItems.FirstOrDefault(x => x.IsSelected);
                if (m != null) m.IsSelected = false;
                menuItems[0].IsEnabled = true;
                selected = menuItems[0].Item;
            }


        }

        // Handle local player joining
        if (currentState == MenuState.InputSelection)
        {
            for (int i = -1; i < 4; i++)
            {
                var input = GameInput.GetInput(i);
                if (input.Back)
                {
                    // Disconnect or go back one screen 
                    if (i == GameCore.PlayerOneProfile.Gamepad)
                    {
                        currentState = MenuState.MainMenu;
                        break;
                    }
                    else if (i == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameCore.PlayerTwoProfile.Gamepad = -9;
                    }
                    else if (i == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameCore.PlayerThreeProfile.Gamepad = -9;
                    }
                    else if (i == GameCore.PlayerFourProfile.Gamepad)
                    {
                        GameCore.PlayerFourProfile.Gamepad = -9;
                    }

                }
                else if (input.UIRight)
                {
                    if (i == GameCore.PlayerOneProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerOneProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerTwoProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerThreeProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerFourProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerFourProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerFourProfile.Skin = skin;
                    }
                }
                else if (input.UILeft)
                {
                    if (i == GameCore.PlayerOneProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerOneProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerTwoProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerThreeProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerFourProfile.Gamepad)
                    {
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerFourProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerFourProfile.Skin = skin;
                    }

                }
                else if (input.Confirm)
                {
                    if (i != GameCore.PlayerOneProfile.Gamepad && i != GameCore.PlayerTwoProfile.Gamepad && i != GameCore.PlayerThreeProfile.Gamepad && i != GameCore.PlayerFourProfile.Gamepad)
                    {
                        if (GameCore.PlayerTwoProfile.Gamepad == -9)
                            GameCore.PlayerTwoProfile.Gamepad = i;
                        else if (GameCore.PlayerThreeProfile.Gamepad == -9)
                            GameCore.PlayerThreeProfile.Gamepad = i;
                        else if (GameCore.PlayerFourProfile.Gamepad == -9)
                            GameCore.PlayerFourProfile.Gamepad = i;
                    }
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
        Raylib.UnloadTexture(logo);
        Raylib.UnloadTexture(background);
        Raylib.UnloadTexture(box);
        Raylib.UnloadTexture(keyboard);
        Raylib.UnloadTexture(gamepad);
        Raylib.UnloadTexture(disconnected);
        Raylib.UnloadTexture(gamepadSlotOn);
        Raylib.UnloadTexture(gamepadSlotOff);
        Raylib.UnloadTexture(arrow);
    }
    static public int FinishMenuScene()
    {
        return finishScreen;
    }

    static void PlaySelectionSound()
    {
        // Play selection sound when change selection
        //----------------------------------------------------------------------------------
        if (lastSelected != selected && selected != Scenes.MenuItem.None) GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection);
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
            TextSize = Raylib.MeasureTextEx(GameAssets.Misc.Font, Text, Size, 0);
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
            Raylib.DrawTextEx(GameAssets.Misc.Font, Text, Position, Size, 0, Color);
        }
    }


    private static void DrawInputSelection()
    {
        Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, new(0, 0, 0, 100)); // Overlay

        Vector2 screenCenter = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f);

        // Render BoxPlayerOne
        Vector2 boxPlayerOne = new(screenCenter.X - 316, screenCenter.Y - 216);
        Raylib.DrawTextureEx(box, boxPlayerOne, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerOne, box.width, box.height, GameCore.PlayerOneProfile.Gamepad, GameCore.PlayerOneProfile.Name, GameCore.PlayerOneProfile.Skin.Texture);

        Vector2 boxPlayerTwo = new(screenCenter.X + 16, screenCenter.Y - 216);
        Raylib.DrawTextureEx(box, boxPlayerTwo, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerTwo, box.width, box.height, GameCore.PlayerTwoProfile.Gamepad, GameCore.PlayerTwoProfile.Name, GameCore.PlayerTwoProfile.Skin.Texture);

        Vector2 boxPlayerThree = new(screenCenter.X - 316, screenCenter.Y + 16);
        Raylib.DrawTextureEx(box, boxPlayerThree, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerThree, box.width, box.height, GameCore.PlayerThreeProfile.Gamepad, GameCore.PlayerThreeProfile.Name, GameCore.PlayerThreeProfile.Skin.Texture);

        Vector2 boxPlayerFour = new(screenCenter.X + 16, screenCenter.Y + 16);
        Raylib.DrawTextureEx(box, boxPlayerFour, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerFour, box.width, box.height, GameCore.PlayerFourProfile.Gamepad, GameCore.PlayerFourProfile.Name, GameCore.PlayerFourProfile.Skin.Texture);

        static void DrawPlayerCard(Vector2 cardPosition, int cardWidth, int cardHeight, int playerGamepadNumber, string profileName, Texture player)
        {
            Vector2 skinPosition = new(cardPosition.X + cardWidth * 0.3f, cardPosition.Y + cardHeight * 0.6f);
            Vector2 inputDevicePosition = new(cardPosition.X + cardWidth * 0.7f, cardPosition.Y + cardHeight * 0.7f);
            Vector2 profileNamePosition = new(cardPosition.X + cardWidth * 0.5f, cardPosition.Y + cardHeight * 0.2f);
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

            // Draw player skin
            if (playerGamepadNumber != -9)
            {
                Raylib.DrawTextureEx(player, new(skinPosition.X - player.width * 2f, skinPosition.Y - player.height * 2f), 0, 4, Raylib.WHITE);

                if (arrowAnimationTimer > 10) arrowExpanding = false;
                else if (arrowAnimationTimer < 0) arrowExpanding = true;

                if (arrowExpanding)
                    arrowAnimationTimer += Raylib.GetFrameTime() * 8;
                else
                    arrowAnimationTimer -= Raylib.GetFrameTime() * 8;

                Raylib.DrawTexturePro(arrow, new(0, 0, arrow.width, arrow.height), new(skinPosition.X + 54 + (int)arrowAnimationTimer, skinPosition.Y, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Raylib.DrawTexturePro(arrow, new(0, 0, -arrow.width, arrow.height), new(skinPosition.X - 54 - (int)arrowAnimationTimer - arrow.width, skinPosition.Y, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Utils.DrawTextCentered(profileName, profileNamePosition, 32, Raylib.WHITE);
            }
        }

    }
}


