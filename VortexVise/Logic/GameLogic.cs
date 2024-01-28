using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;
using VortexVise.States;

namespace VortexVise.Logic
{
    public class GameLogic
    {
        public GameState SimulateState(GameState lastState, double currentTime, Guid playerId, float deltaTime)
        {
            GameState state = new()
            {
                Gravity = lastState.Gravity,
                CurrentTime = currentTime
            };

            // Simulate Player State
            foreach (var lastPlayerState in lastState.PlayerStates)
            {
                PlayerState currentPlayerState = new PlayerState();
                if (lastPlayerState.Id == playerId)
                {
                    currentPlayerState.Input = PlayerLogic.GetInput();
                }
                currentPlayerState.Direction = PlayerLogic.ProcessDirection(deltaTime, currentPlayerState.Input, lastPlayerState);
                (currentPlayerState.Velocity, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ProcessVelocity(deltaTime, currentPlayerState.Input, lastPlayerState, state.Gravity);
                // TODO: ProcessHook
                currentPlayerState.Position = PlayerLogic.ProcessPosition(deltaTime, currentPlayerState, lastPlayerState.Position);

                (currentPlayerState.Position, currentPlayerState.Velocity, currentPlayerState.Collision, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ApplyCollisions(currentPlayerState.Position, currentPlayerState.Velocity, lastPlayerState.Collision);


            }

        }
        public void DrawState(GameState state)
        {
            // All rendering logic should go here

        }
    }
}
