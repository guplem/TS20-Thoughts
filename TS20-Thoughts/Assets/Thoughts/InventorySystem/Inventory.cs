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
            Debug.LogError("Expected error"); // TODO: It does not contain any bc they are not the same object, type should eb checked insteas
            if (item.coveredNeeds.Contains(need))
                return true;
        }
        return false;
    }
}
