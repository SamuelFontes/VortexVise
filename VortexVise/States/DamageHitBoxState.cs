using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Models;
using ZeroElectric.Vinculum;

namespace VortexVise.States;

public class DamageHitBoxState
{
    public DamageHitBoxState(int playerId, Rectangle hitBox, Weapon weapon, float hitBoxTimer)
    {
        PlayerId = playerId;
        HitBox = hitBox;
        Weapon = weapon;
        HitBoxTimer = hitBoxTimer;
    }

    public int PlayerId { get; set; }
    public Rectangle HitBox { get; set; }
    public Weapon Weapon { get; set; }
    public float HitBoxTimer { get; set; }
}
