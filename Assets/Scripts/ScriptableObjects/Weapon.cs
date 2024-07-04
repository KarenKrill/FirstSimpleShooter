using System;
using UnityEngine;

[Serializable]
public class WeaponStats
{
    public float Damage;
    public bool IsReloadable;
    public uint AmmoCountPerShoot;
    public uint AmmoCount;
    public uint MaxAmmoCount;
}

[Serializable]
[CreateAssetMenu(fileName = nameof(Weapon), menuName = "Scriptable Objects/" + nameof(Weapon))]
public class Weapon : GameItem
{
    public WeaponStats WeaponStats;
    public uint Reload(uint ammoCount)
    {
        if (WeaponStats.IsReloadable)
        {
            ammoCount += WeaponStats.AmmoCount;
            if (ammoCount > WeaponStats.MaxAmmoCount)
            {
                WeaponStats.AmmoCount = WeaponStats.MaxAmmoCount;
                return ammoCount - WeaponStats.MaxAmmoCount;
            }
            return ammoCount;
        }
        return 0;
    }
}
