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
    [SerializeField] private int _value;
    public int value { get => _value; private set { _value = value; } }
    [SerializeField] private bool _takeCare;
    public bool takeCare { get => _takeCare; private set { _takeCare = value; } }
    
    public void UpdateOwner(MapElement newOwner)
    {
        this.ownerMapElement = newOwner;
    }
    
    public void UpdateValue(int deltaValue)
    {
        this.value += deltaValue;
    }

    public OwnedAttribute(Attribute attribute, int value, MapElement ownerMapElement, bool takeCare = false)
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

    public bool NeedsCare()
    {
        return value <= 0 && takeCare;
    }

}
