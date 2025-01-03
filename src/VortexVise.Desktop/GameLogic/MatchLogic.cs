using VortexVise.Core.Enums;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Scenes;
using VortexVise.Desktop.States;

namespace VortexVise.Desktop.Logic;

public static class MatchLogic
{
    public static void HandleMatchState(GameState gameState, float deltaTime, SceneManager sceneManager, GameCore gameCore)
    {
        gameState.MatchTimer -= deltaTime;

        if (gameState.MatchTimer < 0)
        {
            if (gameState.MatchState == MatchStates.Warmup)
            {
                gameState.MatchTimer = GameMatch.MatchLengthInSeconds;
                gameState.MatchState = MatchStates.Playing;

                if (GameMatch.CurrentMap.BGM != "") GameAssets.MusicAndAmbience.PlayCustomMusic(GameMatch.CurrentMap.BGM,gameCore);
                else GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetNotGonnaLeoThis,gameCore);
            }
            else if (gameState.MatchState == MatchStates.Playing)
            {
                GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance,gameCore);
                gameState.MatchTimer = 10;
                gameState.MatchState = MatchStates.EndScreen;
            }
            else if (gameState.MatchState == MatchStates.EndScreen)
            {
                if (!gameCore.IsNetworkGame) gameState.IsRunning = false;
                else
                {
                    gameState.MatchTimer = 10;
                    gameState.MatchState = MatchStates.Voting;
                }
            }
            else if (gameState.MatchState == MatchStates.Voting)
            {
                gameState.MatchTimer = 5;
                gameState.MatchState = MatchStates.Warmup;
                MapLogic.LoadNextMap(sceneManager);
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
