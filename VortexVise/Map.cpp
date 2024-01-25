#include "Map.h"

void Map::LoadMap(std::string mapName)
{
	// TODO: Load collisions and image from a file
	std::string mapFolder = "Resources/Sprites/Maps/";
	m_mapTexture = LoadTexture((mapFolder + mapName + ".png").c_str());


	m_collisions.clear();
	m_collisions.push_front(Rectangle{ 170,687,491,32 });
	m_collisions.push_front(Rectangle{ 27,896,76,32 });
	m_collisions.push_front(Rectangle{ 109,932,359,32 });
	m_collisions.push_front(Rectangle{ 479,922,133,32 });
	m_collisions.push_front(Rectangle{ 611,1003,143,32 });
	m_collisions.push_front(Rectangle{ 723,594,149,32 });
	m_collisions.push_front(Rectangle{ 885,616,270,32 });
	m_collisions.push_front(Rectangle{ 1154,298,504,33 });
	m_collisions.push_front(Rectangle{ 1491,705,453,36 });
	m_collisions.push_front(Rectangle{ 1335,912,694,39 });
	m_collisions.push_front(Rectangle{ 830,1380,516,33 });
	m_collisions.push_front(Rectangle{ 380,1392,127,32 });
	m_collisions.push_front(Rectangle{ 504,1456,78,32 });
	m_collisions.push_front(Rectangle{ 222,1626,563,32 });
	m_collisions.push_front(Rectangle{ 674,1711,122,32 });
	m_collisions.push_front(Rectangle{ 1542,1365,184,32 });
	m_collisions.push_front(Rectangle{ 1425,1612,259,32 });
	m_collisions.push_front(Rectangle{ 1687,1639,173,32 });
	m_collisions.push_front(Rectangle{ 1219,1708,267,34 });

}

void Map::Draw()
{
	DrawTextureEx(m_mapTexture, Vector2{ 0, 0 }, 0, 1, WHITE);
	if (Utils::Debug()){
		for (const auto& collision : m_collisions) // DEBUG
		{
			DrawRectangleRec(collision,BLUE);
		}
	}

}

std::list<Rectangle> Map::GetCollisions()
{
	return m_collisions;
}

Vector2 Map::GetMapSize()
{
	return Vector2{ (float)m_mapTexture.width,(float)m_mapTexture.height };
}
