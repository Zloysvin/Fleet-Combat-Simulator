using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageComponent : IComponent
{
    public int Damage;

    public DealDamageComponent(int damage)
    {
        Damage = damage;
    }
}
