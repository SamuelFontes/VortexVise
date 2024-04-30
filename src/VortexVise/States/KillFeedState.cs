namespace VortexVise.States;

public class KillFeedState
{
    public KillFeedState(int killerId, int killedId)
    {
        KillerId = killerId;
        KilledId = killedId;
        Timer = 5;
    }
    public int KillerId { get; set; }
    public int KilledId { get; set; }
    public float Timer { get; set; }
}
