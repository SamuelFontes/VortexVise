using System.Numerics;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Core.Models;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Logic;
using VortexVise.Desktop.Networking;
using VortexVise.Desktop.Utilities;
using ZeroElectric.Vinculum;

namespace VortexVise.Desktop.Scenes;

enum MenuItem { Online, Local, Exit, Return, PressStart, StartGame, ChangeMap, ChangeGameMode, ChangeNumberOfBots, IP, Connect };
enum MenuItemType { Button, TextInput, Selection };
enum MenuState { MainMenu, PressStart, InputSelection, Online, Connecting, Lobby };
/// <summary>
/// Main Menu Scene
/// </summary>
public class MenuScene
{
    readonly List<UIMenuItem> menuItems = [];
    int finishScreen = 0;
    Texture logo;
    private Texture background { get; set; }
    Texture box;
    Texture keyboard;
    Texture gamepad;
    Texture gamepadSlotOn;
    Texture gamepadSlotOff;
    Texture disconnected;
    Texture arrow;
    public Guid selected = Guid.Empty;
    public Guid lastSelected;
    MenuState currentState = MenuState.PressStart;
    float arrowAnimationTimer = 0;
    bool arrowExpanding = true;
    private readonly IInputService _inputService;
    private SceneManager _sceneManager;

    public MenuScene(IInputService inputService, SceneManager sceneManager, GameCore gameCore, IRendererService rendererService)
    {
        _inputService = inputService;
        _sceneManager = sceneManager;

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
        if (gameCore.PlayerOneProfile.Skin.Id == "") gameCore.PlayerOneProfile.Skin = GameAssets.Gameplay.Skins.First();
        if (gameCore.PlayerTwoProfile.Skin.Id == "") gameCore.PlayerTwoProfile.Skin = GameAssets.Gameplay.Skins.First();
        if (gameCore.PlayerThreeProfile.Skin.Id == "") gameCore.PlayerThreeProfile.Skin = GameAssets.Gameplay.Skins.First();
        if (gameCore.PlayerFourProfile.Skin.Id == "") gameCore.PlayerFourProfile.Skin = GameAssets.Gameplay.Skins.First();

        // Initialize items
        //----------------------------------------------------------------------------------
        Vector2 mainMenuTextPosition = new(gameCore.GameScreenWidth * 0.5f, gameCore.GameScreenHeight * 0.7f);

        // PRESS START
        var state = MenuState.PressStart;
        menuItems.Add(new UIMenuItem(this, "PRESS START", MenuItem.PressStart, state, true, MenuItemType.Button, mainMenuTextPosition, gameCore));
        menuItems[0].IsEnabled = true;
        if (selected == Guid.Empty) selected = menuItems[0].Id;
        state = MenuState.MainMenu;

        // MAIN MENU
        var yOffset = gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "LOCAL", MenuItem.Local, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "ONLINE", MenuItem.Online, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "EXIT", MenuItem.Exit, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset), gameCore));

        // LOBBY
        state = MenuState.Lobby;
        Vector2 lobbyButtonPosition = new(gameCore.GameScreenWidth * 0.5f, gameCore.GameScreenHeight * 0.6f);
        yOffset = gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, $"MAP: {GameMatch.CurrentMap.Name}", MenuItem.ChangeMap, state, true, MenuItemType.Selection, lobbyButtonPosition, gameCore));
        //menuItems.Add(new UIMenuItem("MODE: DEATHMATCH", MenuItem.ChangeGameMode, state, true, MenuItemType.Selection, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        //yOffset += gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, $"BOTS: {GameMatch.NumberOfBots}", MenuItem.ChangeNumberOfBots, state, true, MenuItemType.Selection, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize * 2;
        menuItems.Add(new UIMenuItem(this, "START GAME", MenuItem.StartGame, state, true, MenuItemType.Button, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "GO BACK", MenuItem.Return, state, true, MenuItemType.Button, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize;

        // ONLINE
        state = MenuState.Online;
        yOffset = gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "192.168.1.166:9999", MenuItem.IP, state, true, MenuItemType.TextInput, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "Connect", MenuItem.Connect, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "GO BACK", MenuItem.Return, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset), gameCore));
        yOffset += gameCore.MenuFontSize;

        UpdateMenuScene(gameCore,rendererService);
    }

    public void UpdateMenuScene(GameCore gameCore, IRendererService rendererService)
    {
        // Update
        //----------------------------------------------------------------------------------
        if (currentState == MenuState.PressStart) // MAIN MENU PRESS START 
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) || Raylib.IsGestureDetected(Gesture.GESTURE_TAP))
                gameCore.PlayerOneProfile.Gamepad = -1; // Mouse and keyboard
            else if (Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(0, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                gameCore.PlayerOneProfile.Gamepad = 0;
            else if (Raylib.IsGamepadButtonPressed(1, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(1, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                gameCore.PlayerOneProfile.Gamepad = 1;
            else if (Raylib.IsGamepadButtonPressed(2, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(2, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                gameCore.PlayerOneProfile.Gamepad = 2;
            else if (Raylib.IsGamepadButtonPressed(3, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT) || Raylib.IsGamepadButtonPressed(3, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN))
                gameCore.PlayerOneProfile.Gamepad = 3;
            if (gameCore.PlayerOneProfile.Gamepad != -9)
            {
                GameUserInterface.IsCursorVisible = false;
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore, pitch: 0.8f);
                currentState = MenuState.MainMenu;
                selected = menuItems.First(x => x.Item == MenuItem.Local && x.State == currentState).Id;
            }
        }
        else // MAIN MENU
        {
            var input = _inputService.ReadPlayerInput(gameCore.PlayerOneProfile.Gamepad);
            if (input.Confirm || Raylib.IsGestureDetected(Gesture.GESTURE_TAP))
            {
                if (currentState == MenuState.InputSelection)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore);
                    if (gameCore.IsNetworkGame)
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
                    GameAssets.MusicAndAmbience.StopMusic(gameCore);
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore, pitch: 0.5f);
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
                                gameCore.GameShouldClose = true;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore);
                                break;
                            }
                        case MenuItem.Local:
                            {

                                //finishScreen = 2;   // GAMEPLAY
                                gameCore.IsNetworkGame = false;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore);
                                currentState = MenuState.InputSelection;
                                break;
                            }
                        case MenuItem.Online:
                            {
                                //finishScreen = 2;   // GAMEPLAY
                                gameCore.IsNetworkGame = true;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore);
                                currentState = MenuState.InputSelection;
                                break;
                            }
                        case MenuItem.Return:
                            {
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore);
                                currentState = MenuState.MainMenu;
                                selected = menuItems.Where(x => x.State == currentState && x.IsEnabled).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                                break;
                            }
                        case MenuItem.Connect:
                            {
                                currentState = MenuState.Connecting;
                                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click, gameCore);
                                var config = menuItems.Where(x => x.State == currentState && x.IsEnabled).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                                string ip = menuItems.First(_ => _.Item == MenuItem.IP).Text;
                                GameClient.Connect(ip);
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
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                    MapLogic.LoadNextMap(_sceneManager);
                    menuItems.First(x => selected == x.Id).Text = $"MAP: {GameMatch.CurrentMap.Name}";
                }
                else if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeNumberOfBots && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                    GameMatch.NumberOfBots--;
                    if (GameMatch.NumberOfBots < 0) GameMatch.NumberOfBots = 0;
                    menuItems.First(x => selected == x.Id).Text = $"BOTS: {GameMatch.NumberOfBots}";
                }
            }
            else if (input.UIRight)
            {
                if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeMap && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                    MapLogic.LoadPreviousMap(_sceneManager);
                    menuItems.First(x => selected == x.Id).Text = $"MAP: {GameMatch.CurrentMap.Name}";
                }
                else if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeNumberOfBots && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                    GameMatch.NumberOfBots++;
                    if (GameMatch.NumberOfBots > 10) GameMatch.NumberOfBots = 10;
                    menuItems.First(x => selected == x.Id).Text = $"BOTS: {GameMatch.NumberOfBots}";
                }

            }
            else if (input.Back)
            {
                GameAssets.Sounds.PlaySound(GameAssets.Sounds.Click,gameCore, pitch: 0.5f);
                if (currentState == MenuState.MainMenu)
                {
                    currentState = MenuState.PressStart;
                    menuItems[0].IsEnabled = true;
                    selected = menuItems.Where(x => x.IsEnabled && x.State == currentState).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                    gameCore.PlayerOneProfile.Gamepad = -9;
                }
                if (currentState == MenuState.InputSelection || currentState == MenuState.Lobby)
                {
                    currentState = MenuState.MainMenu;
                    selected = menuItems.Where(x => x.IsEnabled && x.State == currentState).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                    gameCore.PlayerTwoProfile.Gamepad = -9;
                    gameCore.PlayerThreeProfile.Gamepad = -9;
                    gameCore.PlayerFourProfile.Gamepad = -9;
                }
                else if (currentState == MenuState.Connecting)
                {
                    currentState = MenuState.Online;
                    GameAssets.Sounds.PlaySound(GameAssets.Sounds.VinylScratch,gameCore);
                    selected = menuItems.First(x => x.Item == MenuItem.Connect && x.State == currentState).Id;
                }
            }
        }

        // Handle local player joining
        if (currentState == MenuState.InputSelection)
        {
            for (int i = -1; i < 4; i++)
            {
                var input = _inputService.ReadPlayerInput(i);
                if (input.Back)
                {
                    // Disconnect or go back one screen 
                    if (i == gameCore.PlayerOneProfile.Gamepad)
                    {
                        currentState = MenuState.MainMenu;
                        break;
                    }
                    else if (i == gameCore.PlayerTwoProfile.Gamepad)
                    {
                        gameCore.PlayerTwoProfile.Gamepad = -9;
                    }
                    else if (i == gameCore.PlayerThreeProfile.Gamepad)
                    {
                        gameCore.PlayerThreeProfile.Gamepad = -9;
                    }
                    else if (i == gameCore.PlayerFourProfile.Gamepad)
                    {
                        gameCore.PlayerFourProfile.Gamepad = -9;
                    }

                }
                else if (input.UIRight)
                {
                    if (i == gameCore.PlayerOneProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != gameCore.PlayerOneProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        gameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (i == gameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != gameCore.PlayerTwoProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        gameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (i == gameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != gameCore.PlayerThreeProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        gameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (i == gameCore.PlayerFourProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != gameCore.PlayerFourProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        gameCore.PlayerFourProfile.Skin = skin;
                    }
                }
                else if (input.UILeft)
                {
                    if (i == gameCore.PlayerOneProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != gameCore.PlayerOneProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        gameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (i == gameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != gameCore.PlayerTwoProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        gameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (i == gameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != gameCore.PlayerThreeProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        gameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (i == gameCore.PlayerFourProfile.Gamepad)
                    {
                        GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore, pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != gameCore.PlayerFourProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        gameCore.PlayerFourProfile.Skin = skin;
                    }

                }
                else if (input.Confirm)
                {
                    if (i != gameCore.PlayerOneProfile.Gamepad && i != gameCore.PlayerTwoProfile.Gamepad && i != gameCore.PlayerThreeProfile.Gamepad && i != gameCore.PlayerFourProfile.Gamepad)
                    {
                        if (gameCore.PlayerTwoProfile.Gamepad == -9)
                            gameCore.PlayerTwoProfile.Gamepad = i;
                        else if (gameCore.PlayerThreeProfile.Gamepad == -9)
                            gameCore.PlayerThreeProfile.Gamepad = i;
                        else if (gameCore.PlayerFourProfile.Gamepad == -9)
                            gameCore.PlayerFourProfile.Gamepad = i;
                    }
                }
            }

        }


        if (currentState == MenuState.Connecting && !GameClient.IsConnecting && !GameClient.IsConnected)
        {
            currentState = MenuState.Online;
            GameAssets.Sounds.PlaySound(GameAssets.Sounds.VinylScratch,gameCore);
            selected = menuItems.First(x => x.Item == MenuItem.Connect && x.State == currentState).Id;
        }

        // Play selection sound
        PlaySelectionSound(gameCore);

        // Update menu
        foreach (var item in menuItems) if (item.State == currentState) item.Update(rendererService);
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

    public void DrawMenuScene(IRendererService rendererService, GameCore gameCore)
    {
        // Draw Background, Logo and Misc
        Vector2 backgroundPos = new(0, 0); // Can use this to move the background around
        Raylib.DrawTextureEx(background, backgroundPos, 0, 2, Raylib.DARKGRAY);
        if (currentState == MenuState.PressStart || currentState == MenuState.MainMenu)
            Raylib.DrawTextureEx(logo, new Vector2(gameCore.GameScreenWidth * 0.5f - logo.width * 0.5f, gameCore.GameScreenHeight * 0.3f - logo.width * 0.5f), 0, 1, Raylib.WHITE);
        else if (currentState == MenuState.Online)
        {
            // TODO: add here input with text
            Utils.DrawTextCentered("CONNECT TO: ", new(gameCore.GameScreenWidth * 0.5f, 64), 64, Raylib.WHITE, rendererService);
        }
        else if (currentState == MenuState.Connecting)
        {
            Utils.DrawTextCentered("CONNECTING", new(gameCore.GameScreenWidth * 0.5f, gameCore.GameScreenHeight * 0.5f), 64, Raylib.WHITE, rendererService);
        }
        else if (currentState == MenuState.Lobby && menuItems.Count > 0)
        {
            // Draw map
            var mapCenterPostion = new Vector2(gameCore.GameScreenWidth * 0.5f, gameCore.GameScreenHeight * 0.37f);
            var size = 176;
            var rec = new System.Drawing.Rectangle((int)(mapCenterPostion.X - size / 2), (int)(mapCenterPostion.Y - size / 2), size, size);
            Raylib.DrawRectangle((int)rec.X - 8, (int)rec.Y - 8, (int)rec.Width + 16, (int)rec.Height + 16, Raylib.BLACK);
            rendererService.DrawTexturePro(GameMatch.CurrentMap.Texture, new(0, 0, GameMatch.CurrentMap.Texture.Width, GameMatch.CurrentMap.Texture.Height), rec, new(0, 0), 0, System.Drawing.Color.White);

            if (!gameCore.IsNetworkGame)
            {
                Utils.DrawTextCentered("ARCADE", new(gameCore.GameScreenWidth * 0.5f, 64), 64, Raylib.WHITE, rendererService);
            }
        }

        // Draw menu
        //----------------------------------------------------------------------------------
        foreach (var item in menuItems) if (item.State == currentState) item.Draw(rendererService);

        // Input Selection
        if (currentState == MenuState.InputSelection)
        {
            DrawInputSelection(rendererService, gameCore);
        }
        if (Utils.Debug())
            Raylib.DrawRectangle(80, 0, 800, 600, new(255, 0, 0, 20));


        // Play selection sound
        //----------------------------------------------------------------------------------
        PlaySelectionSound(gameCore);

    }

    public void UnloadMenuScene()
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
    public int FinishMenuScene()
    {
        return finishScreen;
    }

    void PlaySelectionSound(GameCore gameCore)
    {
        // Play selection sound when change selection
        //----------------------------------------------------------------------------------
        if (lastSelected != selected && selected != Guid.Empty) GameAssets.Sounds.PlaySound(GameAssets.Sounds.Selection,gameCore);
        lastSelected = selected;
    }
    class UIMenuItem
    {
        private readonly MenuScene scene;
        public UIMenuItem(MenuScene menuScene, string text, MenuItem item, MenuState state, bool isEnabled, MenuItemType type, Vector2 centerPosition, GameCore gameCore, string value = "")
        {
            Id = Guid.NewGuid();
            Text = text;
            Item = item;
            State = state;
            IsEnabled = isEnabled;
            Size = gameCore.MenuFontSize;
            Type = type;
            CenterPosition = centerPosition;
            Value = value;
            scene = menuScene;
        }
        public Guid Id { get; }
        public string Value { get; set; }
        public string Text;
        public MenuItem Item { get; set; }
        public MenuState State { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSelected { get { return scene.selected == Id; } }
        public Vector2 Position { get; set; }
        public Vector2 CenterPosition { get; set; }
        public int Size { get; set; }
        Color Color { get; set; }
        public MenuItemType Type { get; set; }
        Vector2 TextSize;
        public void Update(IRendererService rendererService)
        {
            if (Type == MenuItemType.TextInput && IsSelected)
            {
                Utils.UpdateTextUsingKeyboard(ref Text);
            }
            else if (Type == MenuItemType.TextInput && IsSelected)
            {
            }

            // Center the text
            TextSize = rendererService.MeasureTextEx(GameAssets.Misc.Font, Text, Size, 0);
            var pos = new Vector2(CenterPosition.X - TextSize.X * 0.5f, CenterPosition.Y - TextSize.Y * 0.5f); // Centers text

            // Check if mouse is selecting the menu
            if (IsEnabled && GameUserInterface.IsCursorVisible && Raylib.CheckCollisionRecs(new Rectangle(pos.X, pos.Y, TextSize.X, TextSize.Y), new Rectangle(GameUserInterface.CursorPosition.X, GameUserInterface.CursorPosition.Y, 1, 1)))
            {
                scene.selected = Id;
            }

            // Make press start always selected
            if (scene.currentState == MenuState.PressStart && Item == MenuItem.PressStart)
            {
                scene.selected = Id;
            }

            // Paint the text
            if (!IsEnabled) Color = Raylib.GRAY;
            else if (IsSelected) Color = Raylib.ORANGE;
            else Color = Raylib.RAYWHITE;

            Position = pos;
        }

        public void Draw(IRendererService rendererService)
        {
            // Draw input box
            if (Type == MenuItemType.TextInput)
                Raylib.DrawRectangle((int)Position.X - 4, (int)Position.Y - 2, (int)TextSize.X + 8, (int)TextSize.Y + 4, new(0, 0, 0, 100));
            // Draw the text
            rendererService.DrawTextEx(GameAssets.Misc.Font, Text, Position, Size, 0, Color.ToDrawingColor());

            if (Type == MenuItemType.Selection && IsSelected)
            {
                var textBoxSize = rendererService.MeasureTextEx(GameAssets.Misc.Font, Text, Size, 0);
                if (textBoxSize.X % 2 != 0) textBoxSize.X++;
                if (textBoxSize.Y % 2 != 0) textBoxSize.Y++;
                Raylib.DrawTexturePro(scene.arrow, new(0, 0, scene.arrow.width, scene.arrow.height), new((int)Position.X + textBoxSize.X + 8 + (int)scene.arrowAnimationTimer, Position.Y + 8, scene.arrow.width * 2, scene.arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Raylib.DrawTexturePro(scene.arrow, new(0, 0, -scene.arrow.width, scene.arrow.height), new((int)Position.X - 16 - (int)scene.arrowAnimationTimer - scene.arrow.width, Position.Y + 8, scene.arrow.width * 2, scene.arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
            }

        }
    }


    private void DrawInputSelection(IRendererService rendererService, GameCore gameCore)
    {
        Raylib.DrawRectangle(0, 0, gameCore.GameScreenWidth, gameCore.GameScreenHeight, new(0, 0, 0, 100)); // Overlay

        Vector2 screenCenter = new(gameCore.GameScreenWidth * 0.5f, gameCore.GameScreenHeight * 0.5f);

        // Render BoxPlayerOne
        Vector2 boxPlayerOne = new(screenCenter.X - 316, screenCenter.Y - 200);
        Raylib.DrawTextureEx(box, boxPlayerOne, 0, 1, Raylib.WHITE);
        DrawPlayerCard(rendererService, boxPlayerOne, box.width, box.height, gameCore.PlayerOneProfile.Gamepad, gameCore.PlayerOneProfile.Name, gameCore.PlayerOneProfile.Skin.Texture);

        Vector2 boxPlayerTwo = new(screenCenter.X + 16, screenCenter.Y - 200);
        Raylib.DrawTextureEx(box, boxPlayerTwo, 0, 1, Raylib.WHITE);
        DrawPlayerCard(rendererService, boxPlayerTwo, box.width, box.height, gameCore.PlayerTwoProfile.Gamepad, gameCore.PlayerTwoProfile.Name, gameCore.PlayerTwoProfile.Skin.Texture);

        Vector2 boxPlayerThree = new(screenCenter.X - 316, screenCenter.Y + 00);
        Raylib.DrawTextureEx(box, boxPlayerThree, 0, 1, Raylib.WHITE);
        DrawPlayerCard(rendererService, boxPlayerThree, box.width, box.height, gameCore.PlayerThreeProfile.Gamepad, gameCore.PlayerThreeProfile.Name, gameCore.PlayerThreeProfile.Skin.Texture);

        Vector2 boxPlayerFour = new(screenCenter.X + 16, screenCenter.Y + 00);
        Raylib.DrawTextureEx(box, boxPlayerFour, 0, 1, Raylib.WHITE);
        DrawPlayerCard(rendererService, boxPlayerFour, box.width, box.height, gameCore.PlayerFourProfile.Gamepad, gameCore.PlayerFourProfile.Name, gameCore.PlayerFourProfile.Skin.Texture);

        void DrawPlayerCard(IRendererService rendererService, Vector2 cardPosition, int cardWidth, int cardHeight, int playerGamepadNumber, string profileName, ITextureAsset player)
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
                rendererService.DrawTextureEx(player, new(skinPosition.X - player.Width * 2f, skinPosition.Y - player.Height * 2f), 0, 4, System.Drawing.Color.White);

                Raylib.DrawTexturePro(arrow, new(0, 0, arrow.width, arrow.height), new(skinPosition.X + 54 + (int)arrowAnimationTimer, skinPosition.Y, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Raylib.DrawTexturePro(arrow, new(0, 0, -arrow.width, arrow.height), new(skinPosition.X - 54 - (int)arrowAnimationTimer - arrow.width, skinPosition.Y, arrow.width * 2, arrow.height * 2), new(0, 0), 0, Raylib.WHITE);
                Utils.DrawTextCentered(profileName, profileNamePosition, gameCore.MenuFontSize, Raylib.WHITE, rendererService);
            }
        }

    }
}


