namespace VortexVise.Desktop.States;

/// <summary>
/// Used to display kill log
/// </summary>
public class KillFeedState
{
    public KillFeedState(Guid killerId, Guid killedId)
    {
        KillerId = killerId;
        KilledId = killedId;
        Timer = 5;
    }
    public Guid KillerId { get; set; }
    public Guid KilledId { get; set; }
    public float Timer { get; set; }
}
