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
public class ArmorStats
{
    public ArmorType Type;
    public float Defence;
}
[Serializable]
[CreateAssetMenu(fileName = nameof(Armor), menuName = "Scriptable Objects/" + nameof(Armor))]
public class Armor : GameItem
{
    public ArmorStats ArmorStats;
}
