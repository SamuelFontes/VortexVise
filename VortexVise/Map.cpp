#include "Map.h"

void Map::LoadMap(std::string mapName)
{
	std::string mapFolder = "Resources/Sprites/Maps/";
	mapTexture = LoadTexture((mapFolder + mapName + ".png").c_str());
}

void Map::Draw()
{
	DrawTextureEx(mapTexture, Vector2{ 0, 0 }, 0, 0.27, WHITE);
}
