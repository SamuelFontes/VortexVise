using VortexVise.Core.Enums;
using VortexVise.Core.GameContext;
using VortexVise.Core.Interfaces;
using VortexVise.Desktop.GameContext;
using VortexVise.Desktop.Scenes;
using VortexVise.Desktop.States;

namespace VortexVise.Desktop.Logic;

/// <summary>
/// Contains all gameplay logic to simulate a state/frame.
/// </summary>
public static class GameLogic
{
    public static GameState SimulateState(ICollisionService collisionService,GameState lastState, double currentTime, float deltaTime, bool isNetworkFrame, IInputService inputService, SceneManager sceneManager, GameCore gameCore)
    {
        GameState state = new()
        {
            Gravity = lastState.Gravity,
            CurrentTime = currentTime,
            DamageHitBoxes = lastState.DamageHitBoxes,
            MatchTimer = lastState.MatchTimer,
            MatchState = lastState.MatchState,
            Animations = lastState.Animations,
            WeaponDrops = lastState.WeaponDrops,
            KillFeedStates = lastState.KillFeedStates,
            Tick = isNetworkFrame ? lastState.Tick + 1 : lastState.Tick,
        };

        MatchLogic.HandleMatchState(state, deltaTime,sceneManager,gameCore);
        MatchLogic.ProcessKillFeed(state, deltaTime);

        if (state.MatchState == MatchStates.Warmup)
        {
            if ((int)state.MatchTimer < (int)lastState.MatchTimer) GameAssets.Sounds.PlaySound(GameAssets.Sounds.HitMarker,gameCore, pitch: 0.5f);
            state.PlayerStates = lastState.PlayerStates;
        }
        else if (state.MatchState == MatchStates.Playing)
        {
            WeaponLogic.ProcessHitBoxes(collisionService,state, lastState, deltaTime, state.Gravity, gameCore);

            // Update state animations
            state.Animations.RemoveAll(x => x.ShouldDisappear);
            foreach (var animation in state.Animations) animation.Animate(deltaTime);

            // Simulate Player State
            foreach (var lastPlayerState in lastState.PlayerStates)
            {
                PlayerState currentPlayerState = new(lastPlayerState.Id, lastPlayerState.Skin);
                PlayerLogic.CopyLastPlayerState(currentPlayerState, lastPlayerState);

                // Either read player input or get input from last frame 
                if (!GameInput.ReadLocalPlayerInput(isNetworkFrame, currentPlayerState, lastPlayerState,inputService,gameCore))
                    currentPlayerState.Input = lastPlayerState.Input;
                // TODO: Get input from network players here for the corresponding tick
                if (currentPlayerState.IsBot && isNetworkFrame) currentPlayerState.Input = BotLogic.GenerateBotInput(state, currentPlayerState,gameCore);


                // Handle Player Behavior
                PlayerLogic.AddPlayerTimers(currentPlayerState, deltaTime);
                PlayerLogic.HandlePlayerDeath(currentPlayerState, deltaTime, state, lastState,gameCore);
                if (currentPlayerState.IsDead)
                {
                    // Just skip all calculations for this player
                    state.PlayerStates.Add(currentPlayerState);
                    continue;
                }
                PlayerLogic.SetPlayerDirection(currentPlayerState);
                PlayerLogic.ProcessPlayerMovement(currentPlayerState, deltaTime);
                PlayerLogic.ProcessPlayerJump(currentPlayerState, deltaTime,gameCore);
                PlayerLogic.ApplyPlayerGravity(currentPlayerState, deltaTime, state.Gravity);
                PlayerLogic.ProcessPlayerJetPack(currentPlayerState, state, deltaTime,gameCore);
                PlayerHookLogic.SimulateHookState(collisionService,currentPlayerState, state.Gravity, deltaTime,gameCore);
                if (isNetworkFrame) WeaponLogic.ApplyHitBoxesDamage(collisionService,state, currentPlayerState, gameCore); // Only apply damage on tick frames
                PlayerLogic.ApplyPlayerVelocity(currentPlayerState, deltaTime);
                PlayerLogic.ApplyCollisions(currentPlayerState, lastPlayerState, deltaTime,collisionService);

                WeaponLogic.BreakPlayerWeapon(currentPlayerState, gameCore);
                PlayerLogic.ProcessPlayerPickUpItem(state, currentPlayerState,gameCore);

                WeaponLogic.ProcessPlayerShooting(currentPlayerState, state, deltaTime, gameCore);

                // Handle animation
                currentPlayerState.Animation.ProcessDash(deltaTime);
                currentPlayerState.Animation.ProcessAnimationRotation(currentPlayerState.Velocity, currentPlayerState.Input, deltaTime);

                state.PlayerStates.Add(currentPlayerState);
            }

            // Handle Weapon Drops
            WeaponLogic.SpawnWeapons(state, deltaTime, gameCore);
            WeaponLogic.UpdateWeaponDrops(state, deltaTime);
        }
        else if (state.MatchState == MatchStates.EndScreen)
        {
            state.PlayerStates = lastState.PlayerStates;
        }
        else if (state.MatchState == MatchStates.Voting)
        {
            state.PlayerStates = lastState.PlayerStates;
        }

        return state;
    }
}
