using VortexVise.States;

namespace VortexVise.Models;

public class Bot
{
    public int Id { get; set; }
    public WeaponDropState? DropTarget { get; set; }
    public PlayerState? EnemyTarget { get; set; }
    public int TickCounter { get; set; }
}
