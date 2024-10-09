using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInformationComponent : IComponent
{
    public float MoveSpeed = 5f;
    public int MaxMoveDistance = 5;

    public Vector3 Target;

    public MovementInformationComponent(float moveSpeed, int maxMoveDistance)
    {
        MoveSpeed = moveSpeed;
        MaxMoveDistance = maxMoveDistance;
    }

    public MovementInformationComponent(int maxMoveDistance)
    {
        MaxMoveDistance = maxMoveDistance;
    }

    public MovementInformationComponent()
    {
    }
}
