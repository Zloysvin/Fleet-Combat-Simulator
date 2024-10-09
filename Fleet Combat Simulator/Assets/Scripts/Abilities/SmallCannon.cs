using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SmallCannon : IAbility
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public int OptimalRange { get; set; }
    public int Cost { get; set; }
    public Vector3 Target { get; set; }

    public SmallCannon(string name, int damage, int optimalRange, int cost)
    {
        Name = name;
        Damage = damage;
        OptimalRange = optimalRange;
        Cost = cost;
    }

    public void Execute(PositionComponent target, Entity Origin)
    {
        var originPosition = Origin.GetComponent<PositionComponent>();
        var originWorldObj = Origin.GetComponent<GameObjectComponent>().gameObject;
        var targetEntity = GridSystem.GetValue(target.X, target.Y);

        int distanceToTarget =
            GridSystem.CalculateDistance(originPosition.X, originPosition.Y, target.X, target.Y);

        float probability = 0;

        if (distanceToTarget <= 2 * OptimalRange)
        {
            probability = 100f - (float)distanceToTarget / OptimalRange * 50f;
        }
        else
        {
            probability = 0f;
        }

        RaycastHit2D hit = Physics2D.Raycast(originWorldObj.transform.position,
            GridSystem.GetWorldPosition(target.X, target.Y) - originWorldObj.transform.position,
            Vector2.Distance(originWorldObj.transform.position, GridSystem.GetWorldPosition(target.X, target.Y)));

        if (hit.collider != null)
        {
            int x, y;
            GridSystem.GetXY(hit.collider.transform.position, out x, out y);
            int distanceToObstacle =
                GridSystem.CalculateDistance(originPosition.X, originPosition.Y, x, y);
            probability -= 10f + 20f * (float)distanceToObstacle / (2 * OptimalRange);

            if (targetEntity.HasComponent<EvasionBoostComponent>())
                probability -= targetEntity.GetComponent<EvasionBoostComponent>().evasion;

            if (probability < 0f)
                probability = 0f;
        }

        LogSystem.Update(
            $"{(Origin.HasComponent<PlayerTurnMarker>() ? "Ally" : "Enemy")} Used {Name} on {GridSystem.GetValue(target.X, target.Y).GetComponent<ShipInformationComponent>().Type} {GridSystem.GetValue(target.X, target.Y).GetComponent<ShipInformationComponent>().Name}");

        if (Random.Range(0f, 100f) < probability)
        {
            var targetEnt = GridSystem.GetValue(target.X, target.Y);
            targetEnt.AddComponent(new DealDamageComponent(Damage));
            LogSystem.Update(
                $"And hit, dealing {Damage} damage. {targetEnt.GetComponent<HealthComponent>().HP - Damage}/{targetEnt.GetComponent<HealthComponent>().MaxHealth}");
        }
        else
        {
            LogSystem.Update($"And missed!");
        }

        Debug.Log($"Unit {originWorldObj.name} used ability {Name} on {GridSystem.GetValue(target.X, target.Y).GetComponent<GameObjectComponent>().gameObject.name}");
    }

    public (Vector3, int) GetScore(List<Entity> targets, Vector3 Origin)
    {
        List<(Vector3, int)> scores = new List<(Vector3, int)>();

        int defensiveScore = 0;

        foreach (var target in targets)
        {
            // less HP -> higher score
            // if target is within the optimal range -> higher score
            // if target is within the double of optimal range -> score is between 100% and 50%, depending on distance
            // if target is within the triple of optimal range -> score is between 50% and 0%, depending on distance
            // if target is further than the triple of optimal range -> score = 0
            // if target is obstructed by another object -> score = 0

            var targetPosition = target.GetComponent<PositionComponent>();
            var targetObject = target.GetComponent<GameObjectComponent>().gameObject;
            var targetHP = target.GetComponent<HealthComponent>();
            var targetAbilities = target.GetComponent<AbilityComponent>();

            RaycastHit2D hit = Physics2D.Raycast(Origin, targetObject.transform.position - Origin,
                Vector2.Distance(Origin, targetObject.transform.position));

            int offensiveScore = 0;

            int x, y;
            GridSystem.GetXY(Origin, out x, out y);
            int distanceToTarget = GridSystem.CalculateDistance(x, y,
                targetPosition.X, targetPosition.Y);


            if (hit.collider == null)
            {

                int hpScore =
                    Convert.ToInt32((1000f / targets.Count - 100) * (1f - (float)targetHP.HP / (float)targetHP.MaxHealth) + 100);

                int rangeScore = 0;

                if (distanceToTarget <= OptimalRange)
                {
                    float ratio = (float)(OptimalRange - distanceToTarget) / OptimalRange * 50f;
                    rangeScore += (int)ratio + 50;
                }
                else if (distanceToTarget > OptimalRange)
                {
                    if (distanceToTarget > OptimalRange * 2)
                    {
                        rangeScore = 0;
                        hpScore = 0;
                    }
                    else
                    {
                        float ratio = (float)(2 * OptimalRange - distanceToTarget) / OptimalRange * 50f;
                        rangeScore += (int)ratio;
                    }
                }

                offensiveScore = rangeScore + hpScore;
            }

            if (hit.collider != null)
            {
                defensiveScore += 1000 / targets.Count;
            }
            else
            {
                int partialScore = 1000 / targets.Count / 2;
                int reference = targetAbilities.abilities[0].OptimalRange;//(int)(target.Ability.OptimalRange * 1.5f);
                Debug.Log(targetAbilities.abilities[0].GetRange());
                if (distanceToTarget > reference)
                {
                    defensiveScore += partialScore;
                }
                else
                {
                    defensiveScore += (int)(partialScore * distanceToTarget / (targetAbilities.abilities[0].OptimalRange * 1.5f));
                }
            }

            scores.Add(new ValueTuple<Vector3, int>(targetObject.transform.position,
                (int)(defensiveScore * 0.4 + offensiveScore * 0.6)));
        }

        var z = scores.OrderByDescending(t => t.Item2).FirstOrDefault();

        Target = z.Item1;

        return z;
    }
}
