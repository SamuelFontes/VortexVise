#pragma once
#include <raylib.h>
#include "../Models/PlayerProfile.h"
class GameCore
{
public:
	int GameTickRate = 60;
	RenderTexture GameRendering = { 0 };
	int GameScreenWidth = 960;
	int GameScreenHeight = 540;
	int MenuFontSize = 32;
	float GameScreenScale = 0;
	bool GameShouldClose = false;
	bool IsServer = false;
	bool IsNetworkGame = false;
	//PlayerProfile PlayerOneProfile = PlayerProfile{ Id = 1, Name = "PlayerOne", Gamepad = -9 };
	//PlayerProfile PlayerTwoProfile = PlayerProfile{ Id = 2, Name = "PlayerTwo", Gamepad = -9 };
	//PlayerProfile PlayerThreeProfile = { Id = 3, Name = "PlayerThree", Gamepad = -9 };
	//PlayerProfile PlayerFourProfile = PlayerProfile{ Id = 3, Name = "PlayerFour", Gamepad = -9 };
	int MaxWeapons = 4;
};