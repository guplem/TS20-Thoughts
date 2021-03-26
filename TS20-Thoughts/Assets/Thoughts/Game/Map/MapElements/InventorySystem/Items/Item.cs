using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Item", menuName = "Thoughts/Item", order = 1)]
    public class Item : ScriptableObject
    {

        [SerializeField] public List<RelatedNeed> relatedNeeds = new List<RelatedNeed>();
        [SerializeReference] public List<IMapAction> actions;
        
        public MapAction GetAction(int index)
        {
            if (actions.Count > index)
                return (MapAction) actions[index];
            
            Debug.LogWarning($"Trying to get the action with index '{index}' of the Item '{this.name} but the size of the array is {actions.Count}.");
            return null;
        }
        
        public MapAction GetActionToCoverNeed(Need need, MapElement mapElement)
        {
            foreach (IMapAction iMobAction in actions)
            {
                MapAction mapAction = (MapAction) iMobAction;
                //MobAction action = (MobAction) Activator.CreateInstance(actionType.GetType());
                if (mapAction.SatisfiesNeed(need))
                {
                    return mapAction;
                }
            }

            return null;
        }
        public void Apply(ConsequenceNeed consequenceNeed)
        {
            foreach (RelatedNeed relatedNeed in relatedNeeds)
            {
                relatedNeed.Apply(consequenceNeed);
            }
        }
    }
    
}

