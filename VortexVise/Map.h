#pragma once
#include <raylib.h>
#include <string>
#include <iostream>
#include <list>
class Map {
public:
	// Constructor
	Map() {

	};

private:
	Vector2 topRight;
	Vector2 bottomRight;
	std::string mapName;
	std::string mapDescription;
	std::string texturePath;
	Texture2D mapTexture; // This is the whole map backed in an image
	std::list<Rectangle> collisions;

public:
	void LoadMap(std::string mapName);
	void Draw();
	std::list<Rectangle> GetCollisions();

};
