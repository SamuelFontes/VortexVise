#include "Utils.h"

bool debug = false;
int fps = 0;

float Utils::Roundf(float var)
{
	// 37.66666 * 100 =3766.66
	// 3766.66 + .5 =3767.16    for rounding off value
	// then type cast to int so value is 3767
	// then divided by 100 so the value converted into 37.67
	float value = (int)(var * 100 + .5);
	return (float)value / 100;
}

Vector2 Utils::GetVector2Direction(Vector2 from, Vector2 to)
{
	Vector2 direction = { to.x - from.x, to.y - from.y };
	direction = Vector2Normalize(direction);
	return direction;
}

bool Utils::Debug() {
	return debug;
}

void Utils::SwitchDebug() {
	debug = !debug;
}

void Utils::UnlockFPS() {
	if (fps == 0)
		fps = 60;
	else
		fps = 0;
}

int Utils::GetFPS()
{
	return fps;
}

