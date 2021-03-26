using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public MapAction GetAvailableActionToCoverNeed(Need need, MapElement mapElement)
    {
        foreach (Item item in items)
        {
            MapAction action = item.GetActionToCoverNeed(need, mapElement);
            if (action != null)
            {
                return action;
            }
        }
        
        return null;
    }
    
    public void ExecuteTimeElapseActions(MapElement mapElement)
    {
        if (items != null && items.Count > 0)
            Debug.Log($"# Inspecting '{mapElement}' for 'ElapseTimeAction' to execute.");

        foreach (Item item in items)
        {
            Debug.Log($"    - Checking if '{item}' has an 'ElapseTimeAction'.");
            List<IMapAction> itemActions = item.actions;
            foreach (IMapAction itemIAction in itemActions)
            {
                MapAction itemAction = (MapAction)itemIAction;

                if (itemAction.GetType() == typeof(ElapseTimeAction))
                {
                    Debug.Log($"        · Executing action '{itemAction}' of '{item}'.");
                    itemAction.Execute(mapElement, null); // To nobody with no next action in mid in the future (nxt action)
                }
            }
        }
    }
    
    public void Apply(ConsequenceNeed consequenceNeed)
    {
        foreach (Item item in items)
            item.Apply(consequenceNeed);
    }
    
    public Need GetRelatedNeedToTakeCare()
    {
        List<Need> relatedNeedsToTakeCare = new List<Need>();
        foreach (Item item in items)
        {
            foreach (RelatedNeed itemRelatedNeed in item.relatedNeeds)
            {
                if (itemRelatedNeed.needsCare)
                    relatedNeedsToTakeCare.Add(itemRelatedNeed.need);
            }
        }
        relatedNeedsToTakeCare.Sort();
        return relatedNeedsToTakeCare.Count >= 1 ? relatedNeedsToTakeCare.ElementAt(0) : null;
    }
    
    public bool CanSatisfyNeed(RequiredNeed need)
    {
        foreach (Item item in items)
        {
            foreach (RelatedNeed itemRelatedNeed in item.relatedNeeds)
            {
                if (itemRelatedNeed.Satisfies(need))
                    return true;
            }
        }
        return false;
    }
    
}