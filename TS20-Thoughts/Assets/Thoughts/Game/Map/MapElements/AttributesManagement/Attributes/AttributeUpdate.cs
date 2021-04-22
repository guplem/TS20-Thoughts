using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

[Serializable]
public class AttributeUpdate
{
    public Attribute attribute;
    public int value = 1;
    public AttributeUpdateAffected affected = AttributeUpdateAffected.eventOwner;

    public enum AttributeUpdateAffected
    {
        eventOwner,
        eventExecuter,
        eventTarget
    }

    public void Apply(MapElement owner, MapElement executer, MapElement target)
    {
        throw new System.NotImplementedException();
    }
}
