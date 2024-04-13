using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VortexVise.States;

namespace VortexVise.Models;

public class Bot
{
    public int Id { get; set; }
    public WeaponDropState? DropTarget { get; set; }
    public PlayerState? EnemyTarget { get; set; }
    public int TickCounter { get; set; }
}
