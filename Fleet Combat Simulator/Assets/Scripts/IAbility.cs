using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public int OptimalRange { get; set; }
    public int Cost { get; set; }
    public Vector3 Target { get; set; }
    public void Execute(PositionComponent target, Entity Origin);
    public (Vector3, int) GetScore(List<Entity> targets, Vector3 Origin);

    public int GetRange()
    {
        return OptimalRange;
    }
}