#include "Map.h"

void Map::LoadMap(std::string mapName)
{
	// TODO: Load collisions and image from a file
	std::string mapFolder = "Resources/Sprites/Maps/";
	_mapTexture = LoadTexture((mapFolder + mapName + ".png").c_str());


	_collisions.clear();
	_collisions.push_front(Rectangle{ 170,687,491,28 });
	_collisions.push_front(Rectangle{ 27,896,76,23 });
	_collisions.push_front(Rectangle{ 109,932,359,27 });
	_collisions.push_front(Rectangle{ 479,922,133,27 });
	_collisions.push_front(Rectangle{ 611,1003,143,31 });
	_collisions.push_front(Rectangle{ 723,594,149,31 });
	_collisions.push_front(Rectangle{ 885,616,270,31 });
	_collisions.push_front(Rectangle{ 1154,298,504,33 });
	_collisions.push_front(Rectangle{ 1491,705,453,36 });
	_collisions.push_front(Rectangle{ 1335,912,694,39 });
	_collisions.push_front(Rectangle{ 830,1380,516,33 });
	_collisions.push_front(Rectangle{ 380,1392,127,22 });
	_collisions.push_front(Rectangle{ 504,1456,78,15 });
	_collisions.push_front(Rectangle{ 222,1626,563,34 });
	_collisions.push_front(Rectangle{ 674,1711,122,24 });
	_collisions.push_front(Rectangle{ 1542,1365,184,32 });
	_collisions.push_front(Rectangle{ 1425,1612,259,24 });
	_collisions.push_front(Rectangle{ 1687,1639,173,24 });
	_collisions.push_front(Rectangle{ 1219,1708,267,34 });

}

void Map::Draw()
{
	DrawTextureEx(_mapTexture, Vector2{ 0, 0 }, 0, 1, WHITE);
	//for (const auto& collision : _collisions) // DEBUG
	//{
	//	DrawRectangleRec(collision,BLUE);
	//}
}

std::list<Rectangle> Map::GetCollisions()
{
	return _collisions;
}

Vector2 Map::GetMapSize()
{
	return Vector2{(float)_mapTexture.width,(float)_mapTexture.height};
}
