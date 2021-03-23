using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Needs;
using UnityEngine;

public enum IngredientUnit { Spoon, Cup, Bowl, Piece }

[Serializable]
public class NeedSatisfaction : UnityEngine.Object
{
    [SerializeField] private TypeSerializable needType;
    [SerializeField] private int satisfactionAmount;
    
    public string name;
    public int amount = 1;
    public IngredientUnit unit;
    
    
    public NeedSatisfaction(Need need)
    {
        this.needType = new TypeSerializable(need.GetType());
    }
    
    public bool Solves(Need need)
    {
        return (needType.Name == need.GetType().Name);
    }
}
