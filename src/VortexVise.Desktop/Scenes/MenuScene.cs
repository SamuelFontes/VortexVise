using System.Numerics;
using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.Extensions;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Logic;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.Networking;
using VortexVise.Desktop.Utilities;

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
    ITextureAsset logo = new TextureAsset();
    private ITextureAsset background { get; set; } = new TextureAsset();
    ITextureAsset box = new TextureAsset();
    ITextureAsset keyboard = new TextureAsset();
    ITextureAsset gamepad = new TextureAsset();
    ITextureAsset gamepadSlotOn = new TextureAsset();
    ITextureAsset gamepadSlotOff = new TextureAsset();
    ITextureAsset disconnected = new TextureAsset();
    ITextureAsset arrow = new TextureAsset();
    public Guid selected = Guid.Empty;
    public Guid lastSelected;
    MenuState currentState = MenuState.PressStart;
    float arrowAnimationTimer = 0;
    bool arrowExpanding = true;
    private readonly IInputService _inputService;
    private SceneManager _sceneManager;

    public MenuScene(IInputService inputService, SceneManager sceneManager, IRendererService rendererService, ICollisionService collisionService)
    {
        _inputService = inputService;
        _sceneManager = sceneManager;

        GameUserInterface.DisableCursor = false;
        // Initialize menu
        //----------------------------------------------------------------------------------
        finishScreen = 0;

        // Load textures
        //----------------------------------------------------------------------------------
        logo.Load("Resources/Common/vortex-vise-logo.png");
        background.Load("Resources/Common/MenuBackground.png");
        box.Load("Resources/Common/rounded_box.png");
        keyboard.Load("Resources/Common/keyboard.png");
        gamepad.Load("Resources/Common/xbox_gamepad.png");
        disconnected.Load("Resources/Common/xbox_gamepad_disconnected.png");
        gamepadSlotOn.Load("Resources/Common/gamepad_slot_on.png");
        gamepadSlotOff.Load("Resources/Common/gamepad_slot_off.png");
        arrow.Load("Resources/Common/arrow.png");

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
        menuItems.Add(new UIMenuItem(this, "PRESS START", MenuItem.PressStart, state, true, MenuItemType.Button, mainMenuTextPosition));
        menuItems[0].IsEnabled = true;
        if (selected == Guid.Empty) selected = menuItems[0].Id;
        state = MenuState.MainMenu;

        // MAIN MENU
        var yOffset = GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "LOCAL", MenuItem.Local, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "ONLINE", MenuItem.Online, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "EXIT", MenuItem.Exit, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));

        // LOBBY
        state = MenuState.Lobby;
        Vector2 lobbyButtonPosition = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.6f);
        yOffset = GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, $"MAP: {GameMatch.CurrentMap.Name}", MenuItem.ChangeMap, state, true, MenuItemType.Selection, lobbyButtonPosition));
        //menuItems.Add(new UIMenuItem("MODE: DEATHMATCH", MenuItem.ChangeGameMode, state, true, MenuItemType.Selection, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        //yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, $"BOTS: {GameMatch.NumberOfBots}", MenuItem.ChangeNumberOfBots, state, true, MenuItemType.Selection, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize * 2;
        menuItems.Add(new UIMenuItem(this, "START GAME", MenuItem.StartGame, state, true, MenuItemType.Button, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "GO BACK", MenuItem.Return, state, true, MenuItemType.Button, new(lobbyButtonPosition.X, lobbyButtonPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;

        // ONLINE
        state = MenuState.Online;
        yOffset = GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "192.168.1.166:9999", MenuItem.IP, state, true, MenuItemType.TextInput, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "Connect", MenuItem.Connect, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;
        menuItems.Add(new UIMenuItem(this, "GO BACK", MenuItem.Return, state, true, MenuItemType.Button, new(mainMenuTextPosition.X, mainMenuTextPosition.Y + yOffset)));
        yOffset += GameCore.MenuFontSize;

        UpdateMenuScene(rendererService, collisionService, inputService);
    }

    public void UpdateMenuScene(IRendererService rendererService, ICollisionService collisionService, IInputService inputService)
    {
        // Update
        //----------------------------------------------------------------------------------
        if (currentState == MenuState.PressStart) // MAIN MENU PRESS START 
        {
            GameCore.PlayerOneProfile.Gamepad = inputService.GetPressStart();
            if (GameCore.PlayerOneProfile.Gamepad != GamepadSlot.Disconnected)
            {
                GameUserInterface.IsCursorVisible = false;
                GameAssets.Sounds.Click.Play(pitch: 0.8f);
                currentState = MenuState.MainMenu;
                selected = menuItems.First(x => x.Item == MenuItem.Local && x.State == currentState).Id;
            }
        }
        else // MAIN MENU
        {
            var input = _inputService.ReadPlayerInput(GameCore.PlayerOneProfile.Gamepad);
            if (input.Confirm)
            {
                if (currentState == MenuState.InputSelection)
                {
                    GameAssets.Sounds.Click.Play();
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
                    GameAssets.Sounds.Click.Play(pitch: 0.5f);
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
                                GameAssets.Sounds.Click.Play();
                                break;
                            }
                        case MenuItem.Local:
                            {

                                //finishScreen = 2;   // GAMEPLAY
                                GameCore.IsNetworkGame = false;
                                GameAssets.Sounds.Click.Play();
                                currentState = MenuState.InputSelection;
                                break;
                            }
                        case MenuItem.Online:
                            {
                                //finishScreen = 2;   // GAMEPLAY
                                GameCore.IsNetworkGame = true;
                                GameAssets.Sounds.Click.Play();
                                currentState = MenuState.InputSelection;
                                break;
                            }
                        case MenuItem.Return:
                            {
                                GameAssets.Sounds.Click.Play();
                                currentState = MenuState.MainMenu;
                                selected = menuItems.Where(x => x.State == currentState && x.IsEnabled).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                                break;
                            }
                        case MenuItem.Connect:
                            {
                                currentState = MenuState.Connecting;
                                GameAssets.Sounds.Click.Play();
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
                    GameAssets.Sounds.Selection.Play(pitch: 2);
                    MapLogic.LoadNextMap(_sceneManager);
                    menuItems.First(x => selected == x.Id).Text = $"MAP: {GameMatch.CurrentMap.Name}";
                }
                else if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeNumberOfBots && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.Selection.Play(pitch: 2);
                    GameMatch.NumberOfBots--;
                    if (GameMatch.NumberOfBots < 0) GameMatch.NumberOfBots = 0;
                    menuItems.First(x => selected == x.Id).Text = $"BOTS: {GameMatch.NumberOfBots}";
                }
            }
            else if (input.UIRight)
            {
                if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeMap && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.Selection.Play(pitch: 2);
                    MapLogic.LoadPreviousMap(_sceneManager);
                    menuItems.First(x => selected == x.Id).Text = $"MAP: {GameMatch.CurrentMap.Name}";
                }
                else if (selected == menuItems.FirstOrDefault(x => x.Item == MenuItem.ChangeNumberOfBots && x.State == currentState)?.Id)
                {
                    GameAssets.Sounds.Selection.Play(pitch: 2);
                    GameMatch.NumberOfBots++;
                    if (GameMatch.NumberOfBots > 10) GameMatch.NumberOfBots = 10;
                    menuItems.First(x => selected == x.Id).Text = $"BOTS: {GameMatch.NumberOfBots}";
                }

            }
            else if (input.Back)
            {
                GameAssets.Sounds.Click.Play(pitch: 0.5f);
                if (currentState == MenuState.MainMenu)
                {
                    currentState = MenuState.PressStart;
                    menuItems[0].IsEnabled = true;
                    selected = menuItems.Where(x => x.IsEnabled && x.State == currentState).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                    GameCore.PlayerOneProfile.Gamepad = GamepadSlot.Disconnected;
                }
                if (currentState == MenuState.InputSelection || currentState == MenuState.Lobby)
                {
                    currentState = MenuState.MainMenu;
                    selected = menuItems.Where(x => x.IsEnabled && x.State == currentState).Select(x => x.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();
                    GameCore.PlayerTwoProfile.Gamepad = GamepadSlot.Disconnected;
                    GameCore.PlayerThreeProfile.Gamepad = GamepadSlot.Disconnected;
                    GameCore.PlayerFourProfile.Gamepad = GamepadSlot.Disconnected;
                }
                else if (currentState == MenuState.Connecting)
                {
                    currentState = MenuState.Online;
                    GameAssets.Sounds.VinylScratch.Play();
                    selected = menuItems.First(x => x.Item == MenuItem.Connect && x.State == currentState).Id;
                }
            }
        }

        // Handle local player joining
        if (currentState == MenuState.InputSelection)
        {
            for (int i = -1; i < 4; i++)
            {
                GamepadSlot gamepad = (GamepadSlot)i;
                var input = _inputService.ReadPlayerInput(gamepad);
                if (input.Back)
                {
                    // Disconnect or go back one screen 
                    if (gamepad == GameCore.PlayerOneProfile.Gamepad)
                    {
                        currentState = MenuState.MainMenu;
                        break;
                    }
                    else if (gamepad == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameCore.PlayerTwoProfile.Gamepad = GamepadSlot.Disconnected;
                    }
                    else if (gamepad == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameCore.PlayerThreeProfile.Gamepad = GamepadSlot.Disconnected;
                    }
                    else if (gamepad == GameCore.PlayerFourProfile.Gamepad)
                    {
                        GameCore.PlayerFourProfile.Gamepad = GamepadSlot.Disconnected;
                    }

                }
                else if (input.UIRight)
                {
                    if (gamepad == GameCore.PlayerOneProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerOneProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (gamepad == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerTwoProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (gamepad == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerThreeProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (gamepad == GameCore.PlayerFourProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.SkipWhile(item => item.Id != GameCore.PlayerFourProfile.Skin.Id).Skip(1).FirstOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.First();
                        GameCore.PlayerFourProfile.Skin = skin;
                    }
                }
                else if (input.UILeft)
                {
                    if (gamepad == GameCore.PlayerOneProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerOneProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerOneProfile.Skin = skin;
                    }
                    else if (gamepad == GameCore.PlayerTwoProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerTwoProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerTwoProfile.Skin = skin;
                    }
                    else if (gamepad == GameCore.PlayerThreeProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerThreeProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerThreeProfile.Skin = skin;
                    }
                    else if (gamepad == GameCore.PlayerFourProfile.Gamepad)
                    {
                        GameAssets.Sounds.Selection.Play(pitch: 2);
                        var skin = GameAssets.Gameplay.Skins.TakeWhile(item => item.Id != GameCore.PlayerFourProfile.Skin.Id).LastOrDefault();
                        if (skin == null) skin = GameAssets.Gameplay.Skins.Last();
                        GameCore.PlayerFourProfile.Skin = skin;
                    }

                }
                else if (input.Confirm)
                {
                    if (gamepad != GameCore.PlayerOneProfile.Gamepad && gamepad != GameCore.PlayerTwoProfile.Gamepad && gamepad != GameCore.PlayerThreeProfile.Gamepad && gamepad != GameCore.PlayerFourProfile.Gamepad)
                    {
                        if (GameCore.PlayerTwoProfile.Gamepad == GamepadSlot.Disconnected)
                            GameCore.PlayerTwoProfile.Gamepad = gamepad;
                        else if (GameCore.PlayerThreeProfile.Gamepad == GamepadSlot.Disconnected)
                            GameCore.PlayerThreeProfile.Gamepad = gamepad;
                        else if (GameCore.PlayerFourProfile.Gamepad == GamepadSlot.Disconnected)
                            GameCore.PlayerFourProfile.Gamepad = gamepad;
                    }
                }
            }
            if (inputService.GetDebugCommand() == DebugCommand.AddDummyGamepad)
            {
                if (GameCore.PlayerTwoProfile.Gamepad == GamepadSlot.Disconnected)
                    GameCore.PlayerTwoProfile.Gamepad = GamepadSlot.Dummy;
                else if (GameCore.PlayerThreeProfile.Gamepad == GamepadSlot.Disconnected)
                    GameCore.PlayerThreeProfile.Gamepad = GamepadSlot.Dummy;
                else if (GameCore.PlayerFourProfile.Gamepad == GamepadSlot.Disconnected)
                    GameCore.PlayerFourProfile.Gamepad = GamepadSlot.Dummy;
            }

        }


        if (currentState == MenuState.Connecting && !GameClient.IsConnecting && !GameClient.IsConnected)
        {
            currentState = MenuState.Online;
            GameAssets.Sounds.VinylScratch.Play();
            selected = menuItems.First(x => x.Item == MenuItem.Connect && x.State == currentState).Id;
        }

        // Play selection sound
        PlaySelectionSound();

        // Update menu
        foreach (var item in menuItems) if (item.State == currentState) item.Update(rendererService, collisionService);
        var s = menuItems.FirstOrDefault(x => x.IsSelected);
        if (s == null) selected = Guid.Empty;
        else selected = s.Id;

        // Update visual things
        if (arrowAnimationTimer > 10) arrowExpanding = false;
        else if (arrowAnimationTimer < 0) arrowExpanding = true;

        if (arrowExpanding)
            arrowAnimationTimer += rendererService.GetFrameTime() * 8;
        else
            arrowAnimationTimer -= rendererService.GetFrameTime() * 8;
    }

    public void DrawMenuScene(IRendererService rendererService)
    {
        // Draw Background, Logo and Misc
        Vector2 backgroundPos = new(0, 0); // Can use this to move the background around
        rendererService.DrawTextureEx(background, backgroundPos, 0, 2, System.Drawing.Color.DarkGray);
        if (currentState == MenuState.PressStart || currentState == MenuState.MainMenu)
            rendererService.DrawTextureEx(logo, new Vector2(GameCore.GameScreenWidth * 0.5f - logo.Width * 0.5f, GameCore.GameScreenHeight * 0.3f - logo.Width * 0.5f), 0, 1, System.Drawing.Color.White);
        else if (currentState == MenuState.Online)
        {
            // TODO: add here input with text
            rendererService.DrawTextCentered(GameAssets.Misc.Font, "CONNECT TO: ", new(GameCore.GameScreenWidth * 0.5f, 64), 64, System.Drawing.Color.White);
        }
        else if (currentState == MenuState.Connecting)
        {
            rendererService.DrawTextCentered(GameAssets.Misc.Font, "CONNECTING", new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f), 64, System.Drawing.Color.White);
        }
        else if (currentState == MenuState.Lobby && menuItems.Count > 0)
        {
            // Draw map
            var mapCenterPostion = new Vector2(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.37f);
            var size = 176;
            var rec = new System.Drawing.Rectangle((int)(mapCenterPostion.X - size / 2), (int)(mapCenterPostion.Y - size / 2), size, size);
            rendererService.DrawRectangleRec(new System.Drawing.Rectangle((int)rec.X - 8, (int)rec.Y - 8, (int)rec.Width + 16, (int)rec.Height + 16), System.Drawing.Color.Black);
            rendererService.DrawTexturePro(GameMatch.CurrentMap.Texture, new(0, 0, GameMatch.CurrentMap.Texture.Width, GameMatch.CurrentMap.Texture.Height), rec, new(0, 0), 0, System.Drawing.Color.White);

            if (!GameCore.IsNetworkGame)
            {
                rendererService.DrawTextCentered(GameAssets.Misc.Font, "ARCADE", new(GameCore.GameScreenWidth * 0.5f, 64), 64, System.Drawing.Color.White);
            }
        }

        // Draw menu
        //----------------------------------------------------------------------------------
        foreach (var item in menuItems) if (item.State == currentState) item.Draw(rendererService);

        // Input Selection
        if (currentState == MenuState.InputSelection)
        {
            DrawInputSelection(rendererService);
        }
        if (Utils.Debug())
            rendererService.DrawRectangleRec(new System.Drawing.Rectangle(80, 0, 800, 600), System.Drawing.Color.FromArgb(20, 255, 0, 0));


        // Play selection sound
        //----------------------------------------------------------------------------------
        PlaySelectionSound();

    }

    public void UnloadMenuScene()
    {
        logo.Unload();
        background.Unload();
        box.Unload();
        keyboard.Unload();
        gamepad.Unload();
        disconnected.Unload();
        gamepadSlotOn.Unload();
        gamepadSlotOff.Unload();
        arrow.Unload();
        menuItems.Clear();
        GC.Collect();

    }
    public int FinishMenuScene()
    {
        return finishScreen;
    }

    void PlaySelectionSound()
    {
        // Play selection sound when change selection
        //----------------------------------------------------------------------------------
        if (lastSelected != selected && selected != Guid.Empty) GameAssets.Sounds.Selection.Play();
        lastSelected = selected;
    }
    class UIMenuItem
    {
        private readonly MenuScene scene;
        public UIMenuItem(MenuScene menuScene, string text, MenuItem item, MenuState state, bool isEnabled, MenuItemType type, Vector2 centerPosition, string value = "")
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
        System.Drawing.Color Color { get; set; }
        public MenuItemType Type { get; set; }
        Vector2 TextSize;
        public void Update(IRendererService rendererService, ICollisionService collisionService)
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
            if (IsEnabled && GameUserInterface.IsCursorVisible && collisionService.CheckCollisionRecs(new System.Drawing.Rectangle((int)pos.X, (int)pos.Y, (int)TextSize.X, (int)TextSize.Y), new System.Drawing.Rectangle((int)GameUserInterface.CursorPosition.X, (int)GameUserInterface.CursorPosition.Y, 1, 1)))
            {
                scene.selected = Id;
            }

            // Make press start always selected
            if (scene.currentState == MenuState.PressStart && Item == MenuItem.PressStart)
            {
                scene.selected = Id;
            }

            // Paint the text
            if (!IsEnabled) Color = System.Drawing.Color.Gray;
            else if (IsSelected) Color = System.Drawing.Color.Orange;
            else Color = System.Drawing.Color.White;

            Position = pos;
        }

        public void Draw(IRendererService rendererService)
        {
            // Draw input box
            if (Type == MenuItemType.TextInput)
                rendererService.DrawRectangleRec(new System.Drawing.Rectangle((int)Position.X - 4, (int)Position.Y - 2, (int)TextSize.X + 8, (int)TextSize.Y + 4), System.Drawing.Color.FromArgb(100, 0, 0, 0));
            // Draw the text
            rendererService.DrawTextEx(GameAssets.Misc.Font, Text, Position, Size, 0, Color);

            if (Type == MenuItemType.Selection && IsSelected)
            {
                var textBoxSize = rendererService.MeasureTextEx(GameAssets.Misc.Font, Text, Size, 0);
                if (textBoxSize.X % 2 != 0) textBoxSize.X++;
                if (textBoxSize.Y % 2 != 0) textBoxSize.Y++;
                rendererService.DrawTexturePro(scene.arrow, new(0, 0, scene.arrow.Width, scene.arrow.Height), new((int)Position.X + (int)textBoxSize.X + 8 + (int)scene.arrowAnimationTimer, (int)Position.Y + 8, scene.arrow.Width * 2, scene.arrow.Height * 2), new(0, 0), 0, System.Drawing.Color.White);
                rendererService.DrawTexturePro(scene.arrow, new(0, 0, -scene.arrow.Width, scene.arrow.Height), new((int)Position.X - 16 - (int)scene.arrowAnimationTimer - scene.arrow.Width, (int)Position.Y + 8, scene.arrow.Width * 2, scene.arrow.Height * 2), new(0, 0), 0, System.Drawing.Color.White);
            }

        }
    }


    private void DrawInputSelection(IRendererService rendererService)
    {
        rendererService.DrawRectangleRec(new(0, 0, GameCore.GameScreenWidth, GameCore.GameScreenHeight), System.Drawing.Color.FromArgb(100, 0, 0, 0)); // Overlay

        Vector2 screenCenter = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.5f);

        // Render BoxPlayerOne
        Vector2 boxPlayerOne = new(screenCenter.X - 316, screenCenter.Y - 200);
        rendererService.DrawTextureEx(box, boxPlayerOne, 0, 1, System.Drawing.Color.White);
        DrawPlayerCard(rendererService, boxPlayerOne, box.Width, box.Height, GameCore.PlayerOneProfile.Gamepad, GameCore.PlayerOneProfile.Name, GameCore.PlayerOneProfile.Skin.Texture);

        Vector2 boxPlayerTwo = new(screenCenter.X + 16, screenCenter.Y - 200);
        rendererService.DrawTextureEx(box, boxPlayerTwo, 0, 1, System.Drawing.Color.White);
        DrawPlayerCard(rendererService, boxPlayerTwo, box.Width, box.Height, GameCore.PlayerTwoProfile.Gamepad, GameCore.PlayerTwoProfile.Name, GameCore.PlayerTwoProfile.Skin.Texture);

        Vector2 boxPlayerThree = new(screenCenter.X - 316, screenCenter.Y + 00);
        rendererService.DrawTextureEx(box, boxPlayerThree, 0, 1, System.Drawing.Color.White);
        DrawPlayerCard(rendererService, boxPlayerThree, box.Width, box.Height, GameCore.PlayerThreeProfile.Gamepad, GameCore.PlayerThreeProfile.Name, GameCore.PlayerThreeProfile.Skin.Texture);

        Vector2 boxPlayerFour = new(screenCenter.X + 16, screenCenter.Y + 00);
        rendererService.DrawTextureEx(box, boxPlayerFour, 0, 1, System.Drawing.Color.White);
        DrawPlayerCard(rendererService, boxPlayerFour, box.Width, box.Height, GameCore.PlayerFourProfile.Gamepad, GameCore.PlayerFourProfile.Name, GameCore.PlayerFourProfile.Skin.Texture);

        void DrawPlayerCard(IRendererService rendererService, Vector2 cardPosition, int cardWidth, int cardHeight, GamepadSlot playerGamepadNumber, string profileName, ITextureAsset player)
        {
            Vector2 skinPosition = new(cardPosition.X + cardWidth * 0.3f, cardPosition.Y + cardHeight * 0.6f);
            Vector2 inputDevicePosition = new(cardPosition.X + cardWidth * 0.7f, cardPosition.Y + cardHeight * 0.7f);
            Vector2 profileNamePosition = new(cardPosition.X + cardWidth * 0.5f, cardPosition.Y + cardHeight * 0.2f);
            Vector2 gamepadSlotPostion = new(cardPosition.X + cardWidth * 0.59f, cardPosition.Y + cardHeight * 0.5f);
            if (playerGamepadNumber == GamepadSlot.MouseAndKeyboard)
            {
                // mouse and keyboard
                rendererService.DrawTextureEx(keyboard, new(inputDevicePosition.X - keyboard.Width * 1f, inputDevicePosition.Y - keyboard.Height * 1f), 0, 2, System.Drawing.Color.White);
            }
            else if (playerGamepadNumber == GamepadSlot.GamepadOne)
            {
                rendererService.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.Width * 1f, inputDevicePosition.Y - gamepad.Height * 1f), 0, 2, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 1f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 2f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 3f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
            }
            else if (playerGamepadNumber == GamepadSlot.GamepadTwo)
            {
                rendererService.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.Width * 1f, inputDevicePosition.Y - gamepad.Height * 1f), 0, 2, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 1f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 2f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 3f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);

            }
            else if (playerGamepadNumber == GamepadSlot.GamepadThree)
            {
                rendererService.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.Width * 1f, inputDevicePosition.Y - gamepad.Height * 1f), 0, 2, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 1f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 2f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 3f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
            }
            else if (playerGamepadNumber == GamepadSlot.GamepadFour)
            {
                rendererService.DrawTextureEx(gamepad, new(inputDevicePosition.X - gamepad.Width * 1f, inputDevicePosition.Y - gamepad.Height * 1f), 0, 2, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 1f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOff, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 2f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
                rendererService.DrawTextureEx(gamepadSlotOn, new(gamepadSlotPostion.X + gamepadSlotOn.Width * 3f, gamepadSlotPostion.Y), 0, 1, System.Drawing.Color.White);
            }
            else if (playerGamepadNumber == GamepadSlot.Disconnected)
            {
                // Disconnected
                Vector2 disconnectedPosition = new(cardPosition.X + cardWidth * 0.5f, cardPosition.Y + cardHeight * 0.5f);
                rendererService.DrawTextureEx(disconnected, new(disconnectedPosition.X - disconnected.Width * 2f, disconnectedPosition.Y - disconnected.Height * 2f), 0, 4, System.Drawing.Color.White);
            }

            // Draw player skin
            if (playerGamepadNumber != GamepadSlot.Disconnected)
            {
                rendererService.DrawTextureEx(player, new(skinPosition.X - player.Width * 2f, skinPosition.Y - player.Height * 2f), 0, 4, System.Drawing.Color.White);

                rendererService.DrawTexturePro(arrow, new(0, 0, arrow.Width, arrow.Height), new((int)skinPosition.X + 54 + (int)arrowAnimationTimer, (int)skinPosition.Y, arrow.Width * 2, arrow.Height * 2), new(0, 0), 0, System.Drawing.Color.White);
                rendererService.DrawTexturePro(arrow, new(0, 0, -arrow.Width, arrow.Height), new((int)skinPosition.X - 54 - (int)arrowAnimationTimer - arrow.Width, (int)skinPosition.Y, arrow.Width * 2, arrow.Height * 2), new(0, 0), 0, System.Drawing.Color.White);
                rendererService.DrawTextCentered(GameAssets.Misc.Font, profileName, profileNamePosition, GameCore.MenuFontSize, System.Drawing.Color.White);
            }
        }

    }
}


