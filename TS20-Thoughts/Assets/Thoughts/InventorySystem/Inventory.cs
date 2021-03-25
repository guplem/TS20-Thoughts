using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using Thoughts.MapElements;
using Thoughts.Needs;
using UnityEngine;

[Serializable]
public class Inventory
{
   
    [SerializeField] private List<Item> items = new List<Item>();

    public MobAction GetAvailableActionToCoverNeed(Need need, MapElement mapElement,out Vector3 positionToPerformAction)
    {
        foreach (Item item in items)
        {
            MobAction action = item.GetActionToCoverNeed(need, mapElement,out positionToPerformAction);
            if (action != null)
            {
                //positionToPerformAction = action.performPosition;
                return action;
            }
        }
        
        positionToPerformAction = Vector3.zero;
        return null;
    }
}
