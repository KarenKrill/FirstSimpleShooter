using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AmmoStats
{
    public string WeaponName;
    public float Damage;
}
[Serializable]
[CreateAssetMenu(fileName = nameof(Ammo), menuName = "Scriptable Objects/" + nameof(Ammo))]
public class Ammo : GameItem
{
    public AmmoStats AmmoStats;
}
