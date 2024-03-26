using VortexVise.Models;

namespace VortexVise.States;

public class WeaponState
{
    public Weapon Weapon { get; set; } 
    public int CurrentAmmo { get; set; } 
    public int MaxAmmo { get; set; }
    public bool IsEquipped { get; set; }
    public float ReloadTimer { get; set; }
    public int WeaponSlot { get; set; }
}
