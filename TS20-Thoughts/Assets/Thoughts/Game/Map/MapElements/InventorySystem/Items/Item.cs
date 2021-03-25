using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Item", menuName = "Thoughts/Item", order = 1)]
    public class Item : ScriptableObject
    {
        public new string name;

        [SerializeReference] public List<IMapAction> actions;
        public MapAction GetAction(int index)
        {
            if (actions.Count > index)
                return (MapAction) actions[index];
            
            Debug.LogWarning($"Trying to get the action with index '{index}' of the Item '{this.name} but the size of the array is {actions.Count}.");
            return null;
        }
        
        public MapAction GetActionToCoverNeed(Need need, MapElement mapElement, out Vector3 positionToPerformAction)
        {
            foreach (IMapAction iMobAction in actions)
            {
                MapAction mapAction = (MapAction) iMobAction;
                //MobAction action = (MobAction) Activator.CreateInstance(actionType.GetType());
                if (mapAction.SatisfiesNeed(need))
                {
                    positionToPerformAction = mapElement.transform.position;
                    return mapAction;
                }

            }

            positionToPerformAction = Vector3.zero;
            return null;
        }
    }
    
}

