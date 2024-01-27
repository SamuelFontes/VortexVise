#pragma once
#include "libs/yojimbo/include/yojimbo.h"
#include "libs/yojimbo/shared.h"
#include <signal.h>
#include <iostream>
using namespace yojimbo;
class GameServer {
	int m_port = 6969;

public:
	void RunServer(float deltaTime);
};