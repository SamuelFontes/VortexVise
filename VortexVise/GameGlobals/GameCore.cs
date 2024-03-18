﻿using Microsoft.AspNetCore.SignalR.Client;
using ZeroElectric.Vinculum;

namespace VortexVise.GameGlobals;


public static class GameCore
{
    public static int GameTickRate { get; private set; } = 128;                  // Defines tickrate the game and server runs at
    public static int GameScreenWidth { get; private set; } = 960;              // Defines internal game resolution
    public static int GameScreenHeight { get; private set; } = 540;             // Defines internal game resolution
    public static float GameScreenScale { get; set; }                           // How much the screen is scalling 
    public static bool GameShouldClose { get; set; } = false;                   // If true the game will close 
    public static bool IsServer { get; set; } = false;                          // Make sure some things only happen client side, playing sounds for example

    public static Font Font;                                                    // Global font
    public static int TargetFPS { get; private set; } = 0;                      // Simulate game fps
    public static string ServerIPAddress { get; set; } = "";                    // Currently connected server ip
    public static bool IsNetworkGame { get; set; } = false;                     // Defines if it's a network game
    public static int NetworkPort { get; set; } = 9050;                         // Defines the game port
    public static HubConnection HubConnection { get; set; }                     // Network connection
    public static int PlayerOneGamepad { get; set; } = -9;                       // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
    public static int PlayerTwoGamepad { get; set; } = -9;                       // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
    public static int PlayerThreeGamepad { get; set; } = -9;                     // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
    public static int PlayerFourGamepad { get; set; } = -9;                      // Player gamepad sequence, if -1 is mouse and keyboard, -9 is disconnected
}
