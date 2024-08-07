﻿using VortexVise.GameGlobals;
using VortexVise.States;

namespace VortexVise.Logic;

public static class MatchLogic
{
    public static void HandleMatchState(GameState gameState, float deltaTime)
    {
        gameState.MatchTimer -= deltaTime;

        if (gameState.MatchTimer < 0)
        {
            if (gameState.MatchState == Enums.MatchStates.Warmup)
            {
                gameState.MatchTimer = GameMatch.MatchLengthInSeconds;
                gameState.MatchState = Enums.MatchStates.Playing;

                if (GameMatch.CurrentMap.BGM != "") GameAssets.MusicAndAmbience.PlayCustomMusic(GameMatch.CurrentMap.BGM);
                else GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetNotGonnaLeoThis);
            }
            else if (gameState.MatchState == Enums.MatchStates.Playing)
            {
                GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance);
                gameState.MatchTimer = 10;
                gameState.MatchState = Enums.MatchStates.EndScreen;
            }
            else if (gameState.MatchState == Enums.MatchStates.EndScreen)
            {
                if (!GameCore.IsNetworkGame) gameState.IsRunning = false;
                else
                {
                    gameState.MatchTimer = 10;
                    gameState.MatchState = Enums.MatchStates.Voting;
                }
            }
            else if (gameState.MatchState == Enums.MatchStates.Voting)
            {
                gameState.MatchTimer = 5;
                gameState.MatchState = Enums.MatchStates.Warmup;
                MapLogic.LoadNextMap();
            }
        }
    }

    public static void ProcessKillFeed(GameState state, float deltaTime)
    {
        foreach (var kill in state.KillFeedStates)
        {
            kill.Timer -= deltaTime;
        }
        state.KillFeedStates.RemoveAll(x => x.Timer <= 0);
    }
}
