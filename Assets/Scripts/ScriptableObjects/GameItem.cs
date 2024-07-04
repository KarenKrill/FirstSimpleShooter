using UnityEngine;

public class GameItem : ScriptableObject
{
    public string Name;
    public float Weight;
    public Sprite Icon;
    public string Description;
    public GameObject Prefab;
    public uint StackCount;
    public uint MaxStackCount;
}
