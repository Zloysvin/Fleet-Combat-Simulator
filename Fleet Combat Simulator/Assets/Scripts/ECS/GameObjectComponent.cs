using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectComponent : IComponent
{
    public GameObject gameObject;

    public GameObjectComponent(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}
