using System;
using UnityEngine;

[Serializable]
public class Optional<T>
{
    [SerializeField]
    public T Value;
    [SerializeField]
    public bool Enabled;
    public Optional(T value, bool enabled = true)
    {
        Value = value;
        Enabled = enabled;
    }
}
