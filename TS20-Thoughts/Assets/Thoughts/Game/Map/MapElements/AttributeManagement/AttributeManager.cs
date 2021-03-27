using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;
using Attribute = Thoughts.Attribute;
using Object = UnityEngine.Object;

[Serializable]
public class AttributeManager
{
    [SerializeField] private List<Attribute> attributes = new List<Attribute>();

    public void Initialize()
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            attributes[i] = Object.Instantiate(attributes[i]);
        }
    }
    
    public MapAction GetAvailableActionToCoverNeed(Stat stat, MapElement mapElement)
    {
        foreach (Attribute attribute in attributes)
        {
            MapAction action = attribute.GetActionToCoverNeed(stat, mapElement);
            if (action != null)
            {
                return action;
            }
        }
        
        return null;
    }
    
    public void ExecuteTimeElapseActions(MapElement mapElement)
    {
        if (attributes != null && attributes.Count > 0)
            Debug.Log($"# Inspecting '{mapElement}' for 'ElapseTimeAction' to execute.");

        if (attributes != null)
            foreach (Attribute attribute in attributes)
            {
                Debug.Log($"    - Checking if '{attribute}' has an 'ElapseTimeAction'.");
                List<IMapAction> itemActions = attribute.actions;
                foreach (IMapAction itemIAction in itemActions)
                {
                    MapAction itemAction = (MapAction) itemIAction;

                    if (itemAction.GetType() == typeof(ElapseTimeAction))
                    {
                        Debug.Log($"        · Executing action '{itemAction}' of '{attribute}'.");
                        itemAction.Execute(mapElement, null); // To nobody with no next action in mid in the future (nxt action)
                    }
                }
            }
    }
    
    public void Apply(ConsequenceStat consequenceStat)
    {
        foreach (Attribute attribute in attributes)
            attribute.Apply(consequenceStat);
    }
    
    public Stat GetRelatedStateToTakeCare()
    {
        List<Stat> relatedNeedsToTakeCare = new List<Stat>();
        foreach (Attribute item in attributes)
        {
            foreach (RelatedStat itemRelatedNeed in item.relatedStats)
            {
                if (itemRelatedNeed.needsCare)
                    relatedNeedsToTakeCare.Add(itemRelatedNeed.stat);
            }
        }
        relatedNeedsToTakeCare.Sort();
        return relatedNeedsToTakeCare.Count >= 1 ? relatedNeedsToTakeCare.ElementAt(0) : null;
    }
    
    public bool CanSatisfyStat(RequiredStat stat)
    {
        foreach (Attribute attribute in attributes)
        {
            foreach (RelatedStat attributeRelatedNeed in attribute.relatedStats)
            {
                if (attributeRelatedNeed.Satisfies(stat))
                    return true;
            }
        }
        return false;
    }
    
}