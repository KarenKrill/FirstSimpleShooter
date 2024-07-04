using System;
using UnityEngine;

[Serializable]
public class PotionStats
{
    public float RestoreValue;
}
[Serializable]
[CreateAssetMenu(fileName = nameof(Potion), menuName = "Scriptable Objects/" + nameof(Potion))]
public class Potion : GameItem
{
    public PotionStats PotionStats;
}
