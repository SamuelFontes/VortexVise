using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Scenes;
using VortexVise.Desktop.States;

namespace VortexVise.Desktop.Logic;

public static class MatchLogic
{
    public static void HandleMatchState(GameState gameState, float deltaTime, SceneManager sceneManager)
    {
        gameState.MatchTimer -= deltaTime;

        if (gameState.MatchTimer < 0)
        {
            if (gameState.MatchState == MatchStates.Warmup)
            {
                gameState.MatchTimer = GameMatch.MatchLengthInSeconds;
                gameState.MatchState = MatchStates.Playing;

                if (GameMatch.CurrentMap.BGM != "") GameAssets.MusicAndAmbience.PlayCustomMusic(GameMatch.CurrentMap.BGM);
                else GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetNotGonnaLeoThis);
            }
            else if (gameState.MatchState == MatchStates.Playing)
            {
                GameAssets.MusicAndAmbience.PlayMusic(GameAssets.MusicAndAmbience.MusicAssetPixelatedDiscordance);
                gameState.MatchTimer = 10;
                gameState.MatchState = MatchStates.EndScreen;
            }
            else if (gameState.MatchState == MatchStates.EndScreen)
            {
                if (!GameCore.IsNetworkGame) gameState.IsRunning = false;
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
