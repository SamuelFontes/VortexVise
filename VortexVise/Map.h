#pragma once
#include <raylib.h>
#include <string>
#include <iostream>
#include <list>
class Map {
private:
	std::string _mapName;
	std::string _mapDescription;
	std::string _texturePath;
	Texture2D _mapTexture; // This is the whole map backed in an image
	std::list<Rectangle> _collisions;

public:
	void LoadMap(std::string mapName);
	void Draw();
	std::list<Rectangle> GetCollisions();
	Vector2 GetMapSize();

};
