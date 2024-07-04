using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ArmorType
{
    Headgear,
    Cuirass
}
[Serializable]
[CreateAssetMenu(fileName = nameof(Armor), menuName = "Scriptable Objects/" + nameof(Armor))]
public class Armor : GameItem
{
    public ArmorType Type;
    public float Defence;
}
