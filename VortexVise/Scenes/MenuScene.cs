using System.Numerics;
using VortexVise.GameGlobals;
using ZeroElectric.Vinculum;

namespace VortexVise.Scenes;

public static class MenuScene
{
    enum MainMenuItens { MENU_NONE = 0, MENU_VERSUS, MENU_WILDLANDS, MENU_SURVIVAL, MENU_OPTIONS, MENU_LOCAL, MENU_ONLINE, MENU_EXIT, MENU_RETURN };
    enum MainMenuState { MENU_STATE_GAMEMODE = 0, MENU_STATE_INPUT, MENU_STATE_NETWORK };
    enum MainMenuGameMode { VERSUS, SURVIVAL, WILDLANDS };
    static int framesCounter = 0;
    static int finishScreen = 0;
    static Texture2D logo;
    static Texture2D background;
    static Texture2D box;
    static Texture2D xboxController;
    static Texture2D keyboard;
    static MainMenuItens selected;
    static MainMenuItens lastSelected;
    static MainMenuState state;
    static MainMenuGameMode menuGamemode;


    static public void InitMenuScene()
    {
        // Initialize menu
        //----------------------------------------------------------------------------------
        framesCounter = 0;
        finishScreen = 0;
        ResetMenu();

        // Load textures
        //----------------------------------------------------------------------------------
        logo = Raylib.LoadTexture("Resources/Common/vortex-vise-logo.png");
        background = Raylib.LoadTexture("resources/Common/MenuBackground.png");// TODO: load random map 
        /*        box = LoadTexture("resources/common/box.png");
                xboxController = LoadTexture("resources/common/xboxController.png");
                keyboard = LoadTexture("resources/common/keyboard.png");
        */

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
                case MainMenuItens.MENU_OPTIONS:
                    {
                        finishScreen = 1;   // OPTIONS
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.MENU_VERSUS:
                    {
                        menuGamemode = MainMenuGameMode.VERSUS;
                        state = MainMenuState.MENU_STATE_NETWORK;
                        selected = MainMenuItens.MENU_LOCAL;
                        GameSounds.PlaySound(GameSounds.Click);
                        //numberOfLocalPlayers = 1; // TODO: Base this on the input setup
                        break;
                    }
                case MainMenuItens.MENU_LOCAL:
                    {
                        // TODO: Load correct gamemode
                        //if (menuGamemode == MainMenuGameMode.VERSUS) gamemode = DeathMatch;
                        finishScreen = 2;   // GAMEPLAY
                        GameSounds.PlaySound(GameSounds.Click);
                        break;
                    }
                case MainMenuItens.MENU_RETURN:
                    {
                        ResetMenu();
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
                case MainMenuItens.MENU_WILDLANDS: break;
                case MainMenuItens.MENU_SURVIVAL: break;
                case MainMenuItens.MENU_OPTIONS: break;
                case MainMenuItens.MENU_LOCAL: selected = MainMenuItens.MENU_RETURN; break;
                case MainMenuItens.MENU_RETURN: selected = MainMenuItens.MENU_LOCAL; break;
                case MainMenuItens.MENU_ONLINE: break;
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
                case MainMenuItens.MENU_WILDLANDS: break;
                case MainMenuItens.MENU_SURVIVAL: break;
                case MainMenuItens.MENU_OPTIONS: break;
                case MainMenuItens.MENU_LOCAL: selected = MainMenuItens.MENU_RETURN; break;
                case MainMenuItens.MENU_RETURN: selected = MainMenuItens.MENU_LOCAL; break;
                case MainMenuItens.MENU_ONLINE: break;
                case MainMenuItens.MENU_EXIT: selected = MainMenuItens.MENU_VERSUS; break;
                default: break;
            }
        }

        // Play selection sound
        //----------------------------------------------------------------------------------
        PlaySelectionSound();
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
        Vector2 textPosition = new(GameCore.GameScreenWidth * 0.5f, GameCore.GameScreenHeight * 0.7f);
        int size = 32;
        if (state == MainMenuState.MENU_STATE_GAMEMODE)
        {
            if (GameUserInterface.IsCursorVisible) selected = MainMenuItens.MENU_NONE;
            bool wasSelected = false;
            wasSelected = RenderTextCentered(textPosition, "WILDLANDS", size, true, MainMenuItens.MENU_WILDLANDS);
            if (wasSelected) selected = MainMenuItens.MENU_WILDLANDS;
            textPosition.Y += size;
            wasSelected = RenderTextCentered(textPosition, "SURVIVAL MODE", size, true, MainMenuItens.MENU_SURVIVAL);
            if (wasSelected) selected = MainMenuItens.MENU_SURVIVAL;
            textPosition.Y += size;
            wasSelected = RenderTextCentered(textPosition, "VERSUS MODE", size, false, MainMenuItens.MENU_VERSUS);
            if (wasSelected) selected = MainMenuItens.MENU_VERSUS;
            textPosition.Y += size;
            wasSelected = RenderTextCentered(textPosition, "OPTIONS", size, true, MainMenuItens.MENU_OPTIONS);
            if (wasSelected) selected = MainMenuItens.MENU_OPTIONS;
            textPosition.Y += size;
            wasSelected = RenderTextCentered(textPosition, "EXIT", size, false, MainMenuItens.MENU_EXIT);
            if (wasSelected) selected = MainMenuItens.MENU_EXIT;
            textPosition.Y += size;
        }
        else if (state == MainMenuState.MENU_STATE_INPUT)
        {
            float originalX = textPosition.X;
            textPosition.Y -= size * 2;
            textPosition.X -= box.width * 2.5f; // P1
            Raylib.DrawTexture(box, (int)textPosition.X, (int)textPosition.Y, Raylib.WHITE);
            textPosition.X += box.width * 0.25f;
            textPosition.Y += box.width * 0.25f;
            Raylib.DrawTextEx(GameCore.Font, "P1", textPosition, size, 0, Raylib.LIGHTGRAY);
            textPosition.Y -= box.width * 0.25f;
            textPosition.X -= box.width * 0.25f;
            Raylib.DrawTextureEx(keyboard, textPosition, 0, 2, Raylib.WHITE);
            textPosition.X += box.width * 1.25f; // P2
            Raylib.DrawTexture(box, (int)textPosition.X, (int)textPosition.Y, Raylib.LIGHTGRAY);
            textPosition.Y += box.width * 0.25f;
            Raylib.DrawTextEx(GameCore.Font, "press\nenter", textPosition, 16, 0, Raylib.LIGHTGRAY);
            textPosition.Y -= box.width * 0.25f;
            textPosition.X += box.width * 1; // P3
            Raylib.DrawTexture(box, (int)textPosition.X, (int)textPosition.Y, Raylib.LIGHTGRAY);
            textPosition.X += box.width * 0.25f;
            textPosition.Y += box.width * 0.25f;
            Raylib.DrawTextEx(GameCore.Font, "P3", textPosition, size, 0, Raylib.LIGHTGRAY);
            textPosition.Y -= box.width * 0.25f;
            textPosition.X += box.width * 1; // P4
            Raylib.DrawTexture(box, (int)textPosition.X, (int)textPosition.Y, Raylib.LIGHTGRAY);
            textPosition.X += box.width * 0.25f;
            textPosition.Y += box.width * 0.25f;
            Raylib.DrawTextEx(GameCore.Font, "P4", textPosition, size, 0, Raylib.LIGHTGRAY);
            textPosition.Y -= box.width * 0.25f;
            textPosition.Y += size * 4;

            textPosition.X = originalX;
            bool wasSelected = RenderTextCentered(textPosition, "START", size, false, MainMenuItens.MENU_RETURN);
            if (wasSelected) selected = MainMenuItens.MENU_RETURN;
            textPosition.Y += size;
            textPosition.Y += size;
            wasSelected = RenderTextCentered(textPosition, "GO BACK", size, false, MainMenuItens.MENU_RETURN);
            if (wasSelected) selected = MainMenuItens.MENU_RETURN;
            textPosition.Y += size;

        }
        else if (state == MainMenuState.MENU_STATE_NETWORK)
        {
            bool wasSelected = false;
            wasSelected = RenderTextCentered(textPosition, "LOCAL", size, false, MainMenuItens.MENU_LOCAL);
            if (wasSelected) selected = MainMenuItens.MENU_LOCAL;
            textPosition.Y += size;
            wasSelected = RenderTextCentered(textPosition, "ONLINE", size, true, MainMenuItens.MENU_ONLINE);
            if (wasSelected) selected = MainMenuItens.MENU_ONLINE;
            textPosition.Y += size;
            textPosition.Y += size;
            wasSelected = RenderTextCentered(textPosition, "GO BACK", size, false, MainMenuItens.MENU_RETURN);
            if (wasSelected) selected = MainMenuItens.MENU_RETURN;
            textPosition.Y += size;

        }


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
    static bool RenderTextCentered(Vector2 pos, string text, int size, bool disabled, MainMenuItens item)
    {
        bool wasSelected = false;
        Vector2 textSize = Raylib.MeasureTextEx(GameCore.Font, text, size, 0);
        pos.X -= textSize.X * 0.5f;
        pos.Y -= textSize.Y * 0.5f;
        Color color = Raylib.RAYWHITE;
        if (disabled) color = Raylib.GRAY;
        else if ((Raylib.CheckCollisionRecs(new Rectangle(pos.X, pos.Y, textSize.X, textSize.Y), new Rectangle(GameUserInterface.CursorPosition.X, GameUserInterface.CursorPosition.Y, 1, 1))

        && GameUserInterface.IsCursorVisible) || selected == item)
        {
            wasSelected = true;
            color = Raylib.ORANGE;
        }
        Raylib.DrawTextEx(GameCore.Font, text, pos, size, 0, color);
        return wasSelected;
    }

    static void PlaySelectionSound()
    {
        // Play selection sound when change selection
        //----------------------------------------------------------------------------------
        if (lastSelected != selected && selected != MainMenuItens.MENU_NONE) GameSounds.PlaySound(GameSounds.Selection);
        lastSelected = selected;
    }

    static void ResetMenu()
    {
        selected = MainMenuItens.MENU_VERSUS;
        lastSelected = selected;
        state = MainMenuState.MENU_STATE_GAMEMODE;
    }

}
