using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

[Serializable]
public class OwnedAttribute
{
    [SerializeField] public Thoughts.Attribute attribute;
    [SerializeField] public float value;
    [SerializeField] public float minValue;
    [SerializeField] public bool takeCare;
    
    public MapElement ownerMapElement { get; private set; }
    public void UpdateOwner(MapElement newOwner)
    {
        this.ownerMapElement = newOwner;
    }

    public OwnedAttribute(Attribute attribute, float value, MapElement ownerMapElement, float minValue = 0, bool takeCare = false)
    {
        this.attribute = attribute;
        this.value = value;
        this.minValue = minValue;
        this.takeCare = takeCare;
        this.ownerMapElement = ownerMapElement;
    }

}
