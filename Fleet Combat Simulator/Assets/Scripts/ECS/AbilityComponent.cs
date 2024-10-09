using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityComponent : IComponent
{
    public List<IAbility> abilities;

    public AbilityComponent(List<IAbility> abilities)
    {
        this.abilities = abilities;
    }
}
