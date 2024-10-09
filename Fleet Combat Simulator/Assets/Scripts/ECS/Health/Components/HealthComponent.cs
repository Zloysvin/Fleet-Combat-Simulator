using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : IComponent
{
    public int HP;
    public int MaxHealth;

    public HealthComponent(int hp, int maxHealth)
    {
        HP = hp;
        MaxHealth = maxHealth;
    }
}
