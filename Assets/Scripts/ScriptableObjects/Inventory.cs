using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = nameof(Inventory), menuName = "Scriptable Objects/" + nameof(Inventory))]
public class Inventory : ScriptableObject
{
    public List<GameItem> Items;
}
