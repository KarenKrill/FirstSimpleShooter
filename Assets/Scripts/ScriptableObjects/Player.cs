using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public string Name;
    public float Health;
    public float MaxHealth;
    public float HeadDefence;
    public float BodyDefence;
    public float HeadDamageMultiplier;
    public float BodyDamageMultiplier;
}
[Serializable]
[CreateAssetMenu(fileName = nameof(Player), menuName = "Scriptable Objects/" + nameof(Player))]
public class Player : ScriptableObject
{
    public PlayerStats Stats;
    public Inventory Inventory;
    public void Damage(float healthPoints) => Stats.Health -= healthPoints;
    public void Heal(float healthPoints) => Stats.Health += healthPoints;
    public bool IsAlive => Stats.Health > 0;
    public bool IsDead => Stats.Health <= 0;
}
