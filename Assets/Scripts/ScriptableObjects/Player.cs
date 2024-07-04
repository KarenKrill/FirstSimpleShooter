using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = nameof(Player), menuName = "Scriptable Objects/" + nameof(Player))]
public class Player : ScriptableObject
{
    public string Name;
    public float Health;
    public float MaxHealth;
    public float HeadDefence;
    public float BodyDefence;
    public float HeadDamageMultiplier;
    public float BodyDamageMultiplier;
    public Inventory Inventory;
    public void Damage(float healthPoints) => Health -= healthPoints;
    public void Heal(float healthPoints) => Health += healthPoints;
    public bool IsAlive => Health > 0;
    public bool IsDead => Health <= 0;
}
