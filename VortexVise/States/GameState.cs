namespace VortexVise.States;

public class GameState
{
    public double CurrentTime { get; set; }
    public float Gravity { get; set; }
    public List<PlayerState> PlayerStates { get; set; } = [];
}
