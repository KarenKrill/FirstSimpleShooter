using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = nameof(Weapon), menuName = "Scriptable Objects/" + nameof(Weapon))]
public class Weapon : GameItem
{
    public float Damage;
    public bool IsReloadable;
    public uint AmmoCountPerShoot;
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
