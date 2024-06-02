#include <raylib.h>
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
	PlayerProfile PlayerOneProfile = new(){ Id = Guid.NewGuid(), Name = "PlayerOne", Gamepad = -9 };
	PlayerProfile PlayerTwoProfile = new(){ Id = Guid.NewGuid(), Name = "PlayerTwo", Gamepad = -9 };
	PlayerProfile PlayerThreeProfile = new(){ Id = Guid.NewGuid(), Name = "PlayerThree", Gamepad = -9 };
	PlayerProfile PlayerFourProfile = new(){ Id = Guid.NewGuid(), Name = "PlayerFour", Gamepad = -9 };
	int MaxWeapons = 4;
};