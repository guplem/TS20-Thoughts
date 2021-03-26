using System;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Inventory
{
    [SerializeField] private List<Item> items = new List<Item>();

    public void Initialize()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = Object.Instantiate(items[i]);
        }
    }
    
    public MapAction GetAvailableActionToCoverNeed(Need need, MapElement mapElement,out Vector3 positionToPerformAction)
    {
        foreach (Item item in items)
        {
            MapAction action = item.GetActionToCoverNeed(need, mapElement,out positionToPerformAction);
            if (action != null)
            {
                return action;
            }
        }
        
        positionToPerformAction = Vector3.zero;
        return null;
    }
    
    public void ExecuteTimeElapseActions(MapElement mapElement)
    {
        foreach (Item item in items)
        {
            Debug.Log($"Inspecting '{mapElement}/{item}' for 'ElapseTimeAction' to execute.");
            List<IMapAction> itemActions = item.actions;
            foreach (IMapAction itemIAction in itemActions)
            {
                MapAction itemAction = (MapAction)itemIAction;
                Debug.Log($"  - Inspecting action '{itemAction}'");
                if (itemAction.GetType() == typeof(ElapseTimeAction))
                    itemAction.Execute(mapElement);
            }
        }
    }
    
    public void Apply(ConsequenceNeed consequenceNeed)
    {
        foreach (Item item in items)
            item.Apply(consequenceNeed);
    }


    public void CheckNeedToFulfillRelatedNeeds()
    {
        throw new NotImplementedException();
    }
}