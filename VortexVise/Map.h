#pragma once
#include <raylib.h>
#include <string>
#include <iostream>
#include <list>
#include "Utils.h"
class Map {
private:
	std::string m_mapName;
	std::string m_mapDescription;
	std::string m_texturePath;
	Texture2D m_mapTexture; // This is the whole map backed in an image
	std::list<Rectangle> m_collisions;

public:
	void LoadMap(std::string mapName);
	void Draw();
	std::list<Rectangle> GetCollisions();
	Vector2 GetMapSize();

};
