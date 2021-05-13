using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

[Serializable]
public class OwnedAttribute
{
    public MapElement ownerMapElement { get; private set; }
    [SerializeField] public Thoughts.Attribute attribute;
    [SerializeField] public float value;
    [SerializeField] public bool takeCare;
    
    public void UpdateOwner(MapElement newOwner)
    {
        this.ownerMapElement = newOwner;
    }

    public OwnedAttribute(Attribute attribute, float value, MapElement ownerMapElement, bool takeCare = false)
    {
        this.attribute = attribute;
        this.value = value;
        this.takeCare = takeCare;
        this.ownerMapElement = ownerMapElement;
    }

    public override string ToString()
    {
        return $"OwnedAttribute of attribute '{attribute}' owned by '{ownerMapElement}'. Value = {value}. TakeCare = {takeCare}.";
    }

}
