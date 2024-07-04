using System;
using UnityEngine;

[Serializable]
public class GameItemStats
{
    public string Name;
    public float Weight;
    public string Description;
    public uint StackCount;
    public uint MaxStackCount;
}
[Serializable]
public class GameItem : ScriptableObject
{
    public GameItemStats Stats;
    public Sprite Icon;
    public GameObject Prefab;
}
