using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PathInformationComponent : IComponent
{
    public List<List<PathNode>> paths;

    public List<PathNode> selectedPath;

    public PathInformationComponent(List<List<PathNode>> paths)
    {
        this.paths = paths;
    }
}
