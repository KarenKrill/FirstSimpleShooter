using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = nameof(Potion), menuName = "Scriptable Objects/" + nameof(Potion))]
public class Potion : GameItem
{
    public float RestoreValue;
}
