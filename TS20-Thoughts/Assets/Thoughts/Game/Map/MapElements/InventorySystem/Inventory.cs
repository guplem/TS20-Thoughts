using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
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
    
    public List<DemandedNeed> GetDemandedNeedsOf(Type needType)
    {
        List<DemandedNeed> selectedDemandedNeeds = new List<DemandedNeed>();
        foreach (Item item in items)
        {
            foreach (DemandedNeed itemDemandedNeed in item.demandedNeeds)
            {
                if (itemDemandedNeed.needType != null && itemDemandedNeed.needType.GetType() == needType)
                    selectedDemandedNeeds.Add(itemDemandedNeed);
            }
        }
        return selectedDemandedNeeds;
    }
    
    public void ExecuteTimeElapseActions(MapElement mapElement)
    {
        foreach (Item item in items)
        {
            Debug.Log($"Investigating {mapElement}");
            List<IMapAction> itemActions = item.actions;
            foreach (IMapAction itemIAction in itemActions)
            {
                MapAction itemAction = (MapAction)itemIAction;
                Debug.Log($"  - action {itemAction}");
                if (itemAction.GetType() == typeof(ElapseTimeAction))
                    itemAction.Execute(mapElement);
            }
        }
    }
}