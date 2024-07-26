using System.Numerics;
using VortexVise.GameGlobals;
using VortexVise.Logic;
using VortexVise.Networking;
using VortexVise.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

enum MenuItem { Online, Local, Exit, Return, PressStart, StartGame, ChangeMap, ChangeGameMode, ChangeNumberOfBots, IP, Connect };
enum MenuItemType { Button, TextInput, Selection };
enum MenuState { MainMenu, PressStart, InputSelection, Online, Connecting, Lobby };
/// <summary>
/// Main Menu Scene
/// </summary>
public static class MenuScene
{
    static readonly List<UIMenuItem> menuItems = [];
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
    static Guid selected = Guid.Empty;
    static Guid lastSelected;
    static MenuState currentState = MenuState.PressStart;
    static float arrowAnimationTimer = 0;
    static bool arrowExpanding = true;


    static public void InitMenuScene()
    {
        GameUserInterface.DisableCursor = false;
        // Initialize menu
        //----------------------------------------------------------------------------------
        finishScreen = 0;

        // Load textures
        //----------------------------------------------------------------------------------
        logo = Raylib.LoadTexture("Resources/Common/vortex-vise-logo.png");
        background = Raylib.LoadTexture("Resources/Common/MenuBackground.png");
        box = Raylib.LoadTexture("Resources/Common/rounded_box.png");
        keyboard = Raylib.LoadTexture("Resources/Common/keyboard.png");
        gamepad = Raylib.LoadTexture("Resources/Common/xbox_gamepad.png");
        disconnected = Raylib.LoadTexture("Resources/Common/xbox_gamepad_disconnected.png");
        gamepadSlotOn = Raylib.LoadTexture("Resources/Common/gamepad_slot_on.png");
        gamepadSlotOff = Raylib.LoadTexture("Resources/Common/gamepad_slot_off.png");
        arrow = Raylib.LoadTexture("Resources/Common/arrow.png");

        // Load player skins
        //----------------------------------------------------------------------------------
        if (GameCore.PlayerOneProfile.Skin.Id == "") GameCore.PlayerOneProfile.Skin = GameAssets.Gameplay.Skins.First();
        if (GameCore.PlayerTwoProfile.Skin.Id == "") GameCore.PlayerTwoProfile.Skin = GameAssets.Gameplay.Skins.First();
        if (GameCore.PlayerThreeProfile.Skin.Id == "") GameCore.PlayerThreeProfile.Skin = GameAssets.Gameplay.Skins.First();
        if (GameCore.PlayerFourProfile.Skin.Id == "") GameCore.PlayerFourProfile.Skin = GameAssets.Gameplay.Skins.First();

        // Initialize items
        //----------------------------------------------------------------------------------
        Vector2 mainMenuTextPosition = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.7f);

        // PRESS START
        var state = MenuState.PressStart;
        menuItems.Add(new UIMenuItem("PRESS START", MenuItem.PressStart, state, true, MenuItemType.Button, mainMenuTextPosition));
        menuItems[0].IsEnabled = true;
        if (selected == Guid.Empty) selected = menuItems[0].Id;
        state = MenuState.MainMenu;

        // MAIN MENU
        var yOffset = GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem("LOCAL", MenuItem.Local, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem("ONLINE", MenuItem.Online, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem("EXIT", MenuItem.Exit, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));

        // LOBBY
        state = MenuState.Lobby;
        Vector2 lobbyButtonPosition = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.6f);
        yOffset = GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem($"MAP: {GameMatch.CurrentMap.Name}", MenuItem.ChangeMap, state, true, MenuItemType.Selection, lobbyButtonPosition));
        //menuItems.Add(new UIMenuItem("MODE: DEATHMATCH", MenuItem.ChangeGameMode, state, true, MenuItemType.Selection, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        //yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem($"BOTS: {GameMatch.NumberOfBots}", MenuItem.ChangeNumberOfBots, state, true, MenuItemType.Selection, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize * 2;
        menuItems.Add(new UIMenuItem("START GAME", MenuItem.StartGame, state, true, MenuItemType.Button, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem("GO BACK", MenuItem.Return, state, true, MenuItemType.Button, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;

        // ONLINE
        state = MenuState.Online;
        yOffset = GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem("192.168.0.1:3030", MenuItem.IP, state, true, MenuItemType.TextInput, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem("Connect", MenuItem.Connect, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem("GO BACK", MenuItem.Return, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;

        UpdateMenuScene();
    }

    static public void UpdateMenuScene()
    {
        // Update
        //----------------------------------------------------------------------------------
        if (currentState == MenuState.PressStart) // MAIN MENU PRESS START 
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
                selected = menuItems.First(x => x.Item == MenuItem.Local && x.State == currentState).Id;
            }
        }
        else // MAIN MENU
        {
            var input = GameInput.GetInput(GameCore.PlayerOneProfile.Gamepad);
            if (input.Confirm || Raylib.IsGestureDetected(Gesture.GESTURE_TAP))
            {
                if (currentState == MenuState.InputSelection)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                    if (GameCore.IsNetworkGame)
                    {
                        currentState = MenuState.Online;
                        selected = menuItems.First(x => x.State == currentState).Id;
                    }
                    else
                    {
                        currentState = MenuState.Lobby;
                        selected = menuItems.First(x => x.Item == MenuItem.StartGame && x.State == currentState).Id;
                    }
                    return;
                }
                else if (currentState == MenuState.Lobby && selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.StartGame && x.State == currentState)?.Id)
                {
                    finishScreen = 2;
                    GameAssets.MusicAndAmbience.StopMusic();
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click, pitch: 0.5f);
                    return;
                }
                else if (currentState == MenuState.Connecting)
                {

                }
                GameUserInterface.IsCursorVisible = false;
                var itemSelected = menuItems.FirstOrDefault(x => x.Id == selected);
                if (itemSelected != null)
                {
                    switch (itemSelected.Item)
                    {
                        case MenuItem.Exit:
                            {
                                finishScreen = -1;   // EXIT
                                GameCore.GameShouldClose = true;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                                break;
                            }
                        case MenuItem.Local:
                            {

                                //finishScreen = 2;   // GAMEPLAY
                                GameCore.IsNetworkGame = false;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                                currentState = MenuState.InputSelection;
                                break;
                            }
                        case MenuItem.Online:
                            {
                                //finishScreen = 2;   // GAMEPLAY
                                GameCore.IsNetworkGame = true;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                                currentState = MenuState.InputSelection;
                                break;
                            }
                        case MenuItem.Return:
                            {
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                                currentState = MenuState.MainMenu;
                                selected = menuItems.Where(x => x.State == currentState && x.IsEnabled).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                                break;
                            }
                        case MenuItem.Connect:
                            {
                                currentState = MenuState.Connecting;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click);
                                break;
                            }
                        default: break;
                    }

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
                        selected = Guid.Empty;
                    }
                    else if (shouldSelectNext && item.IsEnabled)
                    {
                        shouldSelectNext = false;
                        selected = item.Id;

                    }
                }
                if (shouldSelectNext || selected == Guid.Empty)
                {
                    var item = menuItems.Where(x => x.IsEnabled && x.State == currentState).Last();
                    selected = item.Id;
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
                        selected = Guid.Empty;
                    }
                    else if (shouldSelectNext && item.IsEnabled)
                    {
                        shouldSelectNext = false;
                        selected = item.Id;
                    }
                }
                if (shouldSelectNext || selected == Guid.Empty)
                {
                    selected = menuItems.Where(x => x.IsEnabled && x.State == currentState).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                }
            }
            else if (input.UILeft)
            {
                if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeMap && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                    MapLogic.LoadNextMap();
                    menuItems.First(x => selected == x.Id).Text = $"MAP: {GameMatch.CurrentMap.Name}";
                }
                else if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeNumberOfBots && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                    GameMatch.NumberOfBots--;
                    if (GameMatch.NumberOfBots < 0) GameMatch.NumberOfBots = 0;
                    menuItems.First(x => selected == x.Id).Text = $"BOTS: {GameMatch.NumberOfBots}";
                }
            }
            else if (input.UIRight)
            {
                if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeMap && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                    MapLogic.LoadPreviousMap();
                    menuItems.First(x => selected == x.Id).Text = $"MAP: {GameMatch.CurrentMap.Name}";
                }
                else if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeNumberOfBots && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                    GameMatch.NumberOfBots++;
                    if (GameMatch.NumberOfBots > 10) GameMatch.NumberOfBots = 10;
                    menuItems.First(x => selected == x.Id).Text = $"BOTS: {GameMatch.NumberOfBots}";
                }

            }
            else if (input.Back)
            {
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click, pitch: 0.5f);
                if (currentState == MenuState.MainMenu)
                {
                    currentState = MenuState.PressStart;
                    menuItems[0].IsEnabled = true;
                    selected = menuItems.Where(x => x.IsEnabled && x.State == currentState).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                    GameCore.PlayerOneProfile.Gamepad = -9;
                }
                if (currentState == MenuState.InputSelection || currentState == MenuState.Lobby)
                {
                    currentState = MenuState.MainMenu;
                    selected = menuItems.Where(x => x.IsEnabled && x.State == currentState).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                    GameCore.PlayerTwoProfile.Gamepad = -9;
                    GameCore.PlayerThreeProfile.Gamepad = -9;
                    GameCore.PlayerFourProfile.Gamepad = -9;
                }
                else if (currentState == MenuState.Connecting)
                {
                    // TODO: Connect to server here
                }
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
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerOneProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerTwoProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerThreeProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerFourProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerFourProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerFourProfile.Skin = skin;
                    }
                }
                else if (input.UILeft)
                {
                    if (i == GameCore.PlayerOneProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerOneProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerTwoProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerThreeProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (i == GameCore.PlayerFourProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection, pitch: 2);
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


        if (currentState == MenuState.Connecting && !GameClient.IsConnecting && !GameClient.IsConnected)
        {
            currentState = MenuState.Online;
            GameAssets.Sounds.PlaySound(GameAssets.Sounds.VinylScratch);
            selected = menuItems.First(x => x.Item == MenuItem.Connect && x.State == currentState).Id;
        }

        // Play selection sound
        PlaySelectionSound();

        // Update menu
        foreach (var item in menuItems) if (item.State == currentState) item.Update();
        var s = menuItems.FirstOrDefault(x => x.IsSelected);
        if (s == null) selected = Guid.Empty;
        else selected = s.Id;

        // Update visual things
        if (arrowAnimationTimer > 10) arrowExpanding = false;
        else if (arrowAnimationTimer < 0) arrowExpanding = true;

        if (arrowExpanding)
            arrowAnimationTimer += Raylib.GetFrameTime() * 8;
        else
            arrowAnimationTimer -= Raylib.GetFrameTime() * 8;
    }

    static public void DrawMenuScene()
    {
        // Draw Background, Logo and Misc
        Vector2 backgroundPos = new(0, 0); // Can use this to move the background around
        Raylib.DrawTextureEx(background, backgroundPos, 0, 2, Raylib.DARKGRAY);
        if (currentState == MenuState.PressStart || currentState == MenuState.MainMenu)
            Raylib.DrawTextureEx(logo, new Vector2(GameCore.GameScreenWidth * 0.5f - logo.width * 0.5f, GameCore.GameScreenHeight * 0.3f - logo.width * 0.5f), 0, 1, Raylib.WHITE);
        else if (currentState == MenuState.Online)
        {
            // TODO: add here input with text
            Utils.DrawTextCentered("CONNECT TO: ", new(GameCore.GameScreenWidth * 0.5f, 64), 64, Raylib.WHITE);
        }
        else if (currentState == MenuState.Connecting)
        {
            Utils.DrawTextCentered("CONNECTING", new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f), 64, Raylib.WHITE);
        }
        else if (currentState == MenuState.Lobby && menuItems.Count > 0)
        {
            // Draw map
            var mapCenterPostion = new Vector2(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.37f);
            var size = 176;
            var rec = new Rectangle(mapCenterPostion.X - size / 2, mapCenterPostion.Y - size / 2, size, size);
            Raylib.DrawRectangle((int)rec.x - 8, (int)rec.y - 8, (int)rec.width + 16, (int)rec.height + 16, Raylib.BLACK);
            Raylib.DrawTexturePro(GameMatch.CurrentMap.Texture, new(0, 0, GameMatch.CurrentMap.Texture.width, GameMatch.CurrentMap.Texture.height), rec, new(0, 0), 0, Raylib.WHITE);

            if (!GameCore.IsNetworkGame)// TODO: draw room id if online
            {
                Utils.DrawTextCentered("ARCADE", new(GameCore.GameScreenWidth * 0.5f, 64), 64, Raylib.WHITE);
            }
        }

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
        menuItems.Clear();
        GC.Collect();

    }
    static public int FinishMenuScene()
    {
        return finishScreen;
    }

    static void PlaySelectionSound()
    {
        // Play selection sound when change selection
        //----------------------------------------------------------------------------------
        if (lastSelected != selected && selected != Guid.Empty) GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection);
        lastSelected = selected;
    }
    class UIMenuItem
    {
        public UIMenuItem(string text, MenuItem item, MenuState state, bool isEnabled, MenuItemType type, Vector2 centerPosition, string value = "")
        {
            Id = Guid.NewGuid();
            Text = text;
            Item = item;
            State = state;
            IsEnabled = isEnabled;
            Size = GameCore.MenuFontSize;
            Type = type;
            CenterPosition = centerPosition;
            Value = value;
        }
        public Guid Id { get; }
        public string Value { get; set; }
        public string Text;
        public MenuItem Item { get; set; }
        public MenuState State { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSelected { get { return selected == Id; } }
        public Vector2 Position { get; set; }
        public Vector2 CenterPosition { get; set; }
        public int Size { get; set; }
        Color Color { get; set; }
        public MenuItemType Type { get; set; }
        Vector2 TextSize;
        public void Update()
        {
            if (Type == MenuItemType.TextInput && IsSelected)
            {
                Utils.UpdateTextUsingKeyboard(ref Text);
            }
            else if (Type == MenuItemType.TextInput && IsSelected)
            {
            }

            // Center the text
            TextSize = Raylib.MeasureTextEx(GameAssets.Misc.Font, Text, Size, 0);
            var pos = new Vector2(CenterPosition.X - TextSize.X * 0.5f, CenterPosition.Y - TextSize.Y * 0.5f); // Centers text

            // Check if mouse is selecting the menu
            if (IsEnabled && GameUserInterface.IsCursorVisible && Raylib.CheckCollisionRecs(new Rectangle(pos.X, pos.Y, TextSize.X, TextSize.Y), new Rectangle(GameUserInterface.CursorPosition.X, GameUserInterface.CursorPosition.Y, 1, 1)))
            {
                selected = Id;
            }

            // Make press start always selected
            if (currentState == MenuState.PressStart && Item == MenuItem.PressStart)
            {
                selected = Id;
            }

            // Paint the text
            if (!IsEnabled) Color = Raylib.GRAY;
            else if (IsSelected) Color = Raylib.ORANGE;
            else Color = Raylib.RAYWHITE;

            Position = pos;
        }

        public void Draw()
        {
            // Draw input box
            if (Type == MenuItemType.TextInput)
                Raylib.DrawRectangle((int)Position.X - 4, (int)Position.Y - 2, (int)TextSize.X + 8, (int)TextSize.Y + 4, new(0, 0, 0, 100));
            // Draw the text
            Raylib.DrawTextEx(GameAssets.Misc.Font, Text, Position, Size, 0, Color);

            if (Type == MenuItemType.Selection && IsSelected)
            {
                var textBoxSize = Raylib.MeasureTextEx(GameAssets.Misc.Font, Text, Size, 0);
                if (textBoxSize.X % 2 != 0) textBoxSize.X++;
                if (textBoxSize.Y % 2 != 0) textBoxSize.Y++;
                Raylib.DrawTexturePro(arrow, new(0, 0, arrow.width, arrow.height), new((int)Position.X + textBoxSize.X + 8 + (int)arrowAnimationTimer, Position.Y + 8, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Raylib.DrawTexturePro(arrow, new(0, 0, -arrow.width, arrow.height), new((int)Position.X - 16 - (int)arrowAnimationTimer - arrow.width, Position.Y + 8, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
            }

        }
    }


    private static void DrawInputSelection()
    {
        Raylib.DrawRectangle(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight, new(0, 0, 0, 100)); // Overlay

        Vector2 screenCenter = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f);

        // Render BoxPlayerOne
        Vector2 boxPlayerOne = new(screenCenter.X - 316, screenCenter.Y - 200);
        Raylib.DrawTextureEx(box, boxPlayerOne, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerOne, box.width, box.height, GameCore.PlayerOneProfile.Gamepad, GameCore.PlayerOneProfile.Name, GameCore.PlayerOneProfile.Skin.Texture);

        Vector2 boxPlayerTwo = new(screenCenter.X + 16, screenCenter.Y - 200);
        Raylib.DrawTextureEx(box, boxPlayerTwo, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerTwo, box.width, box.height, GameCore.PlayerTwoProfile.Gamepad, GameCore.PlayerTwoProfile.Name, GameCore.PlayerTwoProfile.Skin.Texture);

        Vector2 boxPlayerThree = new(screenCenter.X - 316, screenCenter.Y + 00);
        Raylib.DrawTextureEx(box, boxPlayerThree, 0, 1, Raylib.WHITE);
        DrawPlayerCard(boxPlayerThree, box.width, box.height, GameCore.PlayerThreeProfile.Gamepad, GameCore.PlayerThreeProfile.Name, GameCore.PlayerThreeProfile.Skin.Texture);

        Vector2 boxPlayerFour = new(screenCenter.X + 16, screenCenter.Y + 00);
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

                Raylib.DrawTexturePro(arrow, new(0, 0, arrow.width, arrow.height), new(skinPosition.X + 54 + (int)arrowAnimationTimer, skinPosition.Y, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Raylib.DrawTexturePro(arrow, new(0, 0, -arrow.width, arrow.height), new(skinPosition.X - 54 - (int)arrowAnimationTimer - arrow.width, skinPosition.Y, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Utils.DrawTextCentered(profileName, profileNamePosition, GameCore.MenuFontSize, Raylib.WHITE);
            }
        }

    }
}


