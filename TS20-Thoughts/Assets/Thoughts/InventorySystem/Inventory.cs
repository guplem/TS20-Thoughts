using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Needs;
using UnityEngine;

[Serializable]
public class Inventory
{
   
    [SerializeField] private List<Item> items = new List<Item>();

    public bool ContainsItemToCoverNeed(Need need)
    {
        foreach (Item item in items)
        {
            if ( item.coveredNeeds.Contains( new TypeSerializable(need.GetType()) ) )
                return true;
        }
        return false;
    }
}
