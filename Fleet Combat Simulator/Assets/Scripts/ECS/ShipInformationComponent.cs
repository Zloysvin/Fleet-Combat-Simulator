using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInformationComponent : IComponent
{
    public string Name;
    public string Type;

    public ShipInformationComponent(string name, string type)
    {
        Name = name;
        Type = type;
    }
}
