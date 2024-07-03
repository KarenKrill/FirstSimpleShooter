using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(Ammo), menuName = "Scriptable Objects/" + nameof(Ammo))]
public class Ammo : GameItem
{
    public string WeaponName;
    public float Damage;
}
