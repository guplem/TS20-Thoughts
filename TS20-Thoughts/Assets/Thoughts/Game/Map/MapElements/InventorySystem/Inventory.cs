using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using Thoughts.Needs;
using UnityEngine;

[Serializable]
public class Inventory
{
   
    [SerializeField] private List<Item> items = new List<Item>();

    public MapAction GetAvailableActionToCoverNeed(Need need, MapElement mapElement,out Vector3 positionToPerformAction)
    {
        foreach (Item item in items)
        {
            MapAction action = item.GetActionToCoverNeed(need, mapElement,out positionToPerformAction);
            if (action != null)
            {
                //positionToPerformAction = action.performPosition;
                return action;
            }
        }
        
        positionToPerformAction = Vector3.zero;
        return null;
    }
    
    /*public void CheckDemandedNeeds(Thoughts.Game.GameMap.MapElement mapElement)
    {
        foreach (Item item in items)
        {
            List<IMapAction> itemActions = item.actions;
            foreach (IMapAction itemIAction in itemActions)
            {
                MapAction itemAction = (MapAction)itemIAction;
                if (itemAction.GetType() == typeof(ElapseTimeAction))
                    itemAction.Execute(mapElement);
            }
            //Action: ElapseTime
        }
    }*/
    
    public List<Need> GetNeeds(Type needType)
    {
        List<Need> needs = new List<Need>();
        foreach (Item item in items)
        {
            foreach (Need need in item.attachedNeeds)
            {
                if (need.GetType() == needType)
                    needs.Add(need);
            }
        }
        return needs;
    }
    
    public void ExecuteTimeElapse(MapElement mapElement)
    {
        foreach (Item item in items)
        {
            List<IMapAction> itemActions = item.actions;
            foreach (IMapAction itemIAction in itemActions)
            {
                MapAction itemAction = (MapAction)itemIAction;
                if (itemAction.GetType() == typeof(ElapseTimeAction))
                    itemAction.Execute(mapElement);
            }
        }
    }
}