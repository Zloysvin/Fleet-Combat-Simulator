using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionBoostComponent : IComponent
{
    public float evasion = 10f;

    public EvasionBoostComponent(float evasion)
    {
        this.evasion = evasion;
    }
}
