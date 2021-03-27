using System.Collections;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using UnityEngine;

public class MapEventFromAttributeAtMapElement
{
    public MapEvent mapEvent;
    public Attribute attribute;
    public MapElement mapElement;

    public MapEventFromAttributeAtMapElement(MapEvent mapEvent, Attribute attribute, MapElement mapElement)
    {
        this.mapElement = mapElement;
        this.mapEvent = mapEvent;
        this.attribute = attribute;
    }

    public override string ToString()
    {
        return $"{mapEvent.GetName()} from {mapElement.name}/{attribute.name}";
    }
}
