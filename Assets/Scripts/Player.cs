using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public float Health;
    public float MaxHealth;
    public void Damage(float healthPoints) => Health -= healthPoints;
    public void Heal(float healthPoints) => Health += healthPoints;
    public bool IsAlive => Health > 0;
    public bool IsDead => Health < 0;
}
