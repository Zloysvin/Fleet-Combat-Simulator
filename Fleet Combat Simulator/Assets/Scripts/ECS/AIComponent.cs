using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Unity.Mathematics;
using UnityEngine;

public class AIComponent : IComponent
{
    public int MaxAbilityScore;
    public List<PathNode> MoveCoords;
    public PositionComponent Target;
    public IAbility BestAbility;
}
