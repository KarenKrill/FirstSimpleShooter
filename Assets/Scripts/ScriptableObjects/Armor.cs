using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmorType
{
    Headgear,
    Cuirass
}
[CreateAssetMenu(fileName = nameof(Armor), menuName = "Scriptable Objects/" + nameof(Armor))]
public class Armor : GameItem
{
    public ArmorType Type;
    public float Defence;
}
