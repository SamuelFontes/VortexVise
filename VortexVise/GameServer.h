#pragma once
#include "libs/yojimbo/include/yojimbo.h"
#include <iostream>
using namespace yojimbo;
class GameServer {
	int m_port = 6969;

public:
	void RunServer(float deltaTime);
};