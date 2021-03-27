using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class MapActionFromMapElement
{
    public MapElement mapElement;
    public MapAction mapAction;

    public MapActionFromMapElement(MapAction mapAction, MapElement mapElement)
    {
        this.mapElement = mapElement;
        this.mapAction = mapAction;
    }

    public override string ToString()
    {
        return $"{mapAction.GetName()} from {mapElement.name}";
    }
}
