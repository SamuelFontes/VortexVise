﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;
using VortexVise.States;

namespace VortexVise.Logic
{
    public static class GameLogic
    {
        public static GameState SimulateState(GameState lastState, double currentTime, Guid playerId, float deltaTime)
        {
            GameState state = new()
            {
                Gravity = lastState.Gravity,
                CurrentTime = currentTime
            };
            if(lastState.PlayerStates.Count < 1)
            {
                throw new Exception("There are no players, this shouldn't happen on the client");
            }

            // Simulate Player State
            foreach (var lastPlayerState in lastState.PlayerStates)
            {
                PlayerState currentPlayerState = new PlayerState(lastPlayerState.Id);
                if (lastPlayerState.Id == playerId)
                {
                    currentPlayerState.Input = PlayerLogic.GetInput();
                }
                currentPlayerState.Direction = PlayerLogic.ProcessDirection(deltaTime, currentPlayerState.Input, lastPlayerState);
                (currentPlayerState.Velocity, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ProcessVelocity(deltaTime, currentPlayerState.Input, lastPlayerState, state.Gravity);
                currentPlayerState.HookState = lastPlayerState.HookState;
                currentPlayerState.Position = PlayerLogic.ProcessPosition(deltaTime, currentPlayerState, lastPlayerState.Position);
                currentPlayerState.HookState = HookLogic.SimulateState(state.Gravity,deltaTime, currentPlayerState);

                (currentPlayerState.Position, currentPlayerState.Velocity, currentPlayerState.Collision, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ApplyCollisions(currentPlayerState.Position, currentPlayerState.Velocity, lastPlayerState.Collision);

                state.PlayerStates.Add(currentPlayerState);
            }

            return state;
        }
        public static void DrawState(GameState state)
        {
            // All rendering logic should go here
            MapLogic.Draw();
            foreach(var playerState in state.PlayerStates)
            {
                HookLogic.DrawState(playerState);
                PlayerLogic.DrawState(playerState);
            }


        }
    }
}
