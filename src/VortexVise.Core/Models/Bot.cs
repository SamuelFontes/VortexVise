using VortexVise.Core.States;

namespace VortexVise.Core.Models
{
    public class Bot
    {
        public Guid Id { get; set; }
        public WeaponDropState? DropTarget { get; set; }
        public PlayerState? EnemyTarget { get; set; }
        public int TickCounter { get; set; }
    }
}
