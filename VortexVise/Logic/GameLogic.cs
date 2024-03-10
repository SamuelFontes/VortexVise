
namespace VortexVise;

public static class GameLogic
{
    public static GameState SimulateState(GameState lastState, double currentTime, Guid playerId, float deltaTime, bool isNetworkFrame)
    {
        GameState state = new()
        {
            Gravity = lastState.Gravity,
            CurrentTime = currentTime
        };

        // Simulate Player State
        foreach (var lastPlayerState in lastState.PlayerStates)
        {
            PlayerState currentPlayerState = new PlayerState(lastPlayerState.Id);
            if (lastPlayerState.Id == playerId)
            {
                //if(isNetworkFrame)
                    currentPlayerState.Input = PlayerLogic.GetInput(); // Only read new inputs on frames we send to the server, the other frames are only for rendering 
                //else 
                    //currentPlayerState.Input = lastPlayerState.Input;
            }else
            {
                currentPlayerState.Input = lastPlayerState.Input;
            }
            currentPlayerState.Direction = PlayerLogic.ProcessDirection(deltaTime, currentPlayerState.Input, lastPlayerState);
            (currentPlayerState.Velocity, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ProcessVelocity(deltaTime, currentPlayerState.Input, lastPlayerState, state.Gravity);
            currentPlayerState.HookState = lastPlayerState.HookState;
            currentPlayerState.Position = PlayerLogic.ProcessPosition(deltaTime, currentPlayerState, lastPlayerState.Position);
            currentPlayerState.HookState = HookLogic.SimulateState(state.Gravity,deltaTime, currentPlayerState);

            (currentPlayerState.Position, currentPlayerState.Velocity, currentPlayerState.Collision, currentPlayerState.IsTouchingTheGround) = PlayerLogic.ApplyCollisions(currentPlayerState.Position, currentPlayerState.Velocity, lastPlayerState.Collision);

            currentPlayerState.Animation = lastPlayerState.Animation;

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
