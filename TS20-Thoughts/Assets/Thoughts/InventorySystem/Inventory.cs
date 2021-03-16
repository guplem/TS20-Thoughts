using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Needs;
using UnityEngine;

[Serializable]
public class Inventory
{
   
    [SerializeField] private List<Item> items = new List<Item>();

    public bool ContainsItemToCoverNeed(Need need, out Item itemToCoverNeed)
    {
        foreach (Item item in items)
        {
            if (item.coveredNeeds.Contains(new TypeSerializable(need.GetType())))
            {
                itemToCoverNeed = item;
                return true;

            }
        }
        
        itemToCoverNeed = null;
        return false;
    }
}
