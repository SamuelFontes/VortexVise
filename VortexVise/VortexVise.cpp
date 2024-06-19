#include "VortexVise.h"
#include "Game/GameCore.h"

int main(int argc, char* argv[])
{
	// Initialization
	//---------------------------------------------------------
	SetConfigFlags(FLAG_WINDOW_RESIZABLE);                                               // Make game window resizeble
	//var monitorHeight = GetMonitorHeight(0);
	//var monitorWidth = GetMonitorWidth(0);
	/*if (false)// Turn on to compile to steam deck
	{
		GameCore.GameScreenHeight = 400;
		GameCore.GameScreenWidth = 640;
		GameCore.MenuFontSize = 20;
	}
	*/
	InitWindow(GameCore.GameScreenWidth, GameCore.GameScreenHeight, "Vortex Vise");                  // Create game window
	SetWindowMinSize(GameCore.GameScreenWidth, GameCore.GameScreenHeight);                           // Set minimal window size
	InitAudioDevice();                                                                               // Initialize audio device
	HideCursor();                                                                                    // Hide windows cursor
	SetTargetFPS(GameSettings.TargetFPS);                                                            // Set game target FPS
	SetExitKey(0);                                                                                   // Disable escape closing the game
	GameAssets.InitializeAssets();                                                                          // Load global data 
	GameCore.GameRendering = LoadRenderTexture(GameCore.GameScreenWidth, GameCore.GameScreenHeight); // Game will be rendered to this texture
	Image icon = LoadImage("Resources/Skins/afatso.png");                                            // Load icon at runtime
	SetWindowIcon(icon);                                                                             // Set icon
	UnloadImage(icon);                                                                               // Unload icon from memory



	// Initiate music
	GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance);      // Play main menu song

	// Setup and init first screen
	SceneManager.CurrentScene = GameScene.MENU;
	MenuScene.InitMenuScene();


	// Main Game Loop
	//--------------------------------------------------------------------------------------
	while(!(WindowShouldClose() || GameCore.GameShouldClose))
	{
		// Read PC Keys
		//----------------------------------------------------------------------------------
		if(IsKeyPressed(KeyboardKey.KEY_F11))
		{
			if(GameSettings.BorderlessFullScreen)
				ToggleBorderlessWindowed();
			else
				ToggleFullscreen();
		}
		if(IsKeyPressed(KeyboardKey.KEY_F7)) Utils.SwitchDebug();

		// Update music
		//----------------------------------------------------------------------------------
		if(GameAssets.MusicAndAmbience.IsMusicPlaying) UpdateMusicStream(GameAssets.MusicAndAmbience.Music);       // NOTE: Music keeps playing between screens

		// Update game
		//----------------------------------------------------------------------------------
		SceneManager.UpdateScene();


		// Update user interface
		//----------------------------------------------------------------------------------
		GameUserInterface.UpdateUserInterface();


		// Deal with resolution
		//----------------------------------------------------------------------------------
		// Setup scalling
		GameCore.GameScreenScale = Utils.MIN((float) GetScreenWidth() / GameCore.GameScreenWidth, (float) GetScreenHeight() / GameCore.GameScreenHeight); // TODO: This should be calculated only on screen size change
		TextureFilter screenFiltering;
		if(GameSettings.IntegerScalling && GameCore.GameScreenScale == (int) GameCore.GameScreenScale)
		{
			screenFiltering = TextureFilter.TEXTURE_FILTER_POINT;
		}
		else
		{
			screenFiltering = TextureFilter.TEXTURE_FILTER_BILINEAR;
		}
		SetTextureFilter(GameCore.GameRendering.texture, screenFiltering);  // Texture scale filter to use

		BeginTextureMode(GameCore.GameRendering);


		ClearBackground(BLACK);

		// Draw Game
		//----------------------------------------------------------------------------------
		SceneManager.DrawScene();

		// Draw full screen rectangle in front of everything
		if(SceneManager.OnTransition) SceneManager.DrawTransition();

		GameUserInterface.DrawUserInterface();

		EndTextureMode();
		BeginDrawing();
		ClearBackground(BLACK);     // Clear screen background

		// Draw render texture to screen, properly scaled
		if(GameSettings.MaintainAspectRatio)
		{
			DrawTexturePro(GameCore.GameRendering.texture, new(0.0f, 0.0f, GameCore.GameRendering.texture.width, -GameCore.GameRendering.texture.height), new(
				(GetScreenWidth() - (GameCore.GameScreenWidth * GameCore.GameScreenScale)) * 0.5f, (GetScreenHeight() - ((float) GameCore.GameScreenHeight * GameCore.GameScreenScale)) * 0.5f, (float) GameCore.GameScreenWidth * GameCore.GameScreenScale, (float) GameCore.GameScreenHeight * GameCore.GameScreenScale), new Vector2(0, 0), 0.0f, WHITE);
		}
		else
		{

			DrawTexturePro(GameCore.GameRendering.texture, new Rectangle(0.0f, 0.0f, (float) GameCore.GameRendering.texture.width, (float) -GameCore.GameRendering.texture.height), new Rectangle(0, 0, GetScreenWidth(), GetScreenHeight()), new Vector2(0, 0), 0.0f, WHITE);

		}


		EndDrawing();
		//----------------------------------------------------------------------------------
	}

	// Fade screen to black when exit
	//--------------------------------------------------------------------------------------
	SceneManager.TransitionToNewScene(GameScene.UNKNOWN);
	while(!SceneManager.TransitionFadeOut)
	{
		SceneManager.UpdateTransition();
	}

	// De-Initialization
	//--------------------------------------------------------------------------------------
	GameAssets.UnloadAssets();

	CloseAudioDevice();     // Close audio context
	CloseWindow();          // Close window and OpenGL context

}
