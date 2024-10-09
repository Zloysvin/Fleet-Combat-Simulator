using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EvasionAbility : IAbility
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public int OptimalRange { get; set; }
    public int Cost { get; set; }
    public Vector3 Target { get; set; }

    public EvasionAbility(string name, int damage, int optimalRange, int cost)
    {
        Name = name;
        Damage = damage;
        OptimalRange = optimalRange;
        Cost = cost;
    }

    public void Execute(PositionComponent target, Entity Origin)
    {
        Origin.AddComponent(new EvasionBoostComponent(Damage));
        Debug.Log($"{Origin.GetComponent<GameObjectComponent>().gameObject.name} used {Name} ability");
    }

    public (Vector3, int) GetScore(List<Entity> targets, Vector3 Origin)
    {
        return (Vector3.zero, 0);
    }
}
