#include "GameServer.h"
void interrupt_handler( int /*dummy*/ )
{
    //quit = 1;
}

void GameServer::RunServer(float deltaTime)
{
    if (!InitializeYojimbo())
    {
        std::cout << "ERROR: Error initializing server!" << std::endl;
        // TODO: Stop program
    }
	yojimbo_log_level( YOJIMBO_LOG_LEVEL_INFO );
	srand( (unsigned int) time( NULL ) ); // wut

    std::cout << "Started server on port " << ServerPort << std::endl;

    double time = 100.0;

    ClientServerConfig config;

    uint8_t privateKey[KeyBytes];
    memset(privateKey, 0, KeyBytes);

    Server server(GetDefaultAllocator(), privateKey, Address("127.0.0.1", ServerPort), config, adapter, time);

    server.Start(MaxClients);

    char addressString[256];
    server.GetAddress().ToString(addressString, sizeof(addressString));
    printf("server address is %s\n", addressString);

    const double deltaTime = 0.01f;

    signal(SIGINT, interrupt_handler);

    while (1) // TODO: make this have a way of quitting
    {
        server.SendPackets();

        server.ReceivePackets();

        time += deltaTime;

        server.AdvanceTime(time);

        if (!server.IsRunning())
            break;

        yojimbo_sleep(deltaTime);
    }

    server.Stop();

}
