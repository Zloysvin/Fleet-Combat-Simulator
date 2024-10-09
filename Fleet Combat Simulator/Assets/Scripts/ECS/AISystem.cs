using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AISystem
{
    public static void Update(Entity Origin)
    {
        if (!GameManager.IsPlayerTurn && Origin.HasComponent<SelectedMarker>() && !Origin.HasComponent<InMotionMarker>() &&
            Origin.HasComponent<AIComponent>())
        {
            var abilities = Origin.GetComponent<AbilityComponent>().abilities;
            var ai = Origin.GetComponent<AIComponent>();

            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities[i].Cost != 2)
                {
                    var result = CalculateAbilityScores(abilities[i], Origin);
                    if (result.Item2 > ai.MaxAbilityScore)
                    {
                        ai.MaxAbilityScore = result.Item2;
                        ai.MoveCoords = result.Item1;
                        ai.Target = GridSystem.GetXY(result.Item3);
                        ai.BestAbility = abilities[i];
                    }
                }
                else
                {
                    var result = abilities[i].GetScore(
                        GameManager.entities.Where(e => e.HasComponent<PlayerTurnMarker>()).ToList(),
                        GridSystem.GetWorldPosition(Origin.GetComponent<PositionComponent>().X,
                            Origin.GetComponent<PositionComponent>().Y) +
                        new Vector3(0.5f, 0.5f));
                    if (result.Item2 > ai.MaxAbilityScore)
                    {
                        ai.MaxAbilityScore = result.Item2;
                        ai.MoveCoords = new List<PathNode>();
                        ai.Target = GridSystem.GetXY(result.Item1);
                        ai.BestAbility = abilities[i];
                    }
                }
            }

            if (ai.BestAbility.Cost == 2)
            {
                Origin.AddComponent(new AbilityReadyToUseMarker());
                ai.MaxAbilityScore = 0;
            }
        }
    }

    private static (List<PathNode>, int, Vector3) CalculateAbilityScores(IAbility Ability, Entity Origin)
    {
        var AllowedPaths = Origin.GetComponent<PathInformationComponent>().paths;

        List<(List<PathNode>, int, Vector3)> scores = new List<(List<PathNode>, int, Vector3)>();

        foreach (var allowedPath in AllowedPaths)
        {
            var abilityResult = Ability.GetScore(
                GameManager.entities.Where(e => e.HasComponent<PlayerTurnMarker>()).ToList(),
                GridSystem.GetWorldPosition(allowedPath[^1].x, allowedPath[^1].y) +
                new Vector3(0.5f, 0.5f));


            scores.Add((allowedPath, abilityResult.Item2, abilityResult.Item1));
            //Debug.Log(scores[^1].Item2);
            //GameManager.Instance.DebugColorGrid(allowedPath.end.x, allowedPath.end.y, (float)(scores[^1].Item2 / 1000f));
            //GameManager.Grid.SetCustomDebugText(allowedPath.end.x, allowedPath.end.y, scores[^1].Item2.ToString());
        }

        var selected = scores.OrderByDescending(t => t.Item2).FirstOrDefault();
        return selected;
    }
}
