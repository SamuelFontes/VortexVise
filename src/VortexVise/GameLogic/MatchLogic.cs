using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameGlobals;
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
                gameState.IsRunning = false;
                //gameState.MatchTimer = 10;
                //gameState.MatchState = Enums.MatchStates.Voting;
            }
            else if (gameState.MatchState == Enums.MatchStates.Voting)
            {
                gameState.MatchTimer = 5;
                gameState.MatchState = Enums.MatchStates.Warmup;
                MapLogic.LoadNextMap();
            }
        }
    }
}
