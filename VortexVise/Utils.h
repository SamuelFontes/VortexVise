#pragma once
#include <raylib.h>
#include <raymath.h>
class Utils
{
private:
	Utils(){}
public:
	static float Roundf(float var);
	static Vector2 GetVector2Direction(Vector2 from, Vector2 to);
	static const bool Debug = false;
};

