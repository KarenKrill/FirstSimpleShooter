using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public string Name;
    public string[] SupportedWeaponAmmoNames;
    public float Damage;
    public bool IsReloadable;
    public uint AmmoCount;
    public uint MaxAmmoCount;
    public uint Reload(uint ammoCount)
    {
        if (IsReloadable)
        {
            ammoCount += AmmoCount;
            if (ammoCount > MaxAmmoCount)
            {
                AmmoCount = MaxAmmoCount;
                return ammoCount - MaxAmmoCount;
            }
            return ammoCount;
        }
        return 0;
    }
}
