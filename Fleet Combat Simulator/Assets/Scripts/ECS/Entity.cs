using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    private readonly Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();

    public void AddComponent<T>(T component) where T : class, IComponent
    {
        Type componentType = typeof(T);

        if (!components.TryAdd(componentType, component))
        {
            Debug.Log($"Entity already has a component of type {componentType.Name}.");
        }
    }

    public void RemoveComponent<T>() where T : class, IComponent
    {
        Type componentType = typeof(T);

        if (components.ContainsKey(componentType))
        {
            components.Remove(componentType);
        }
        else
        {
            Debug.Log($"Entity does not have a component of type {componentType.Name}.");
        }
    }

    public T GetComponent<T>() where T : class, IComponent
    {
        Type componentType = typeof(T);

        if (components.TryGetValue(componentType, out var component))
        {
            return component as T;
        }
        else
        {
            Debug.Log($"Entity does not have a component of type {componentType.Name}.");
            return null;
        }
    }

    public bool HasComponent<T>() where T : class, IComponent
    {
        Type componentType = typeof(T);
        return components.ContainsKey(componentType);
    }
}
