using VortexVise.Core.Models;

namespace VortexVise.Core.States;

/// <summary>
/// State for weapon.
/// </summary>
public class WeaponState
{
    public WeaponState(Weapon weapon, int currentAmmo, int maxAmmo, bool isEquipped, float reloadTimer, int weaponSlot)
    {
        Weapon = weapon;
        CurrentAmmo = currentAmmo;
        MaxAmmo = maxAmmo;
        IsEquipped = isEquipped;
        ReloadTimer = reloadTimer;
        WeaponSlot = weaponSlot;
    }

    public Weapon Weapon { get; set; }
    public int CurrentAmmo { get; set; }
    public int MaxAmmo { get; set; }
    public bool IsEquipped { get; set; }
    public float ReloadTimer { get; set; }
    public int WeaponSlot { get; set; }
}
