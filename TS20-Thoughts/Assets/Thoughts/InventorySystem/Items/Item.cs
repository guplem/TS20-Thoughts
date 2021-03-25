using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.GameMap;
using Thoughts.MapElements;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Item", menuName = "Thoughts/Item", order = 1)]
    public class Item : ScriptableObject
    {
        public new string name;

        [SerializeReference] public List<IMobAction> actions;
        public MobAction GetAction(int index)
        {
            if (actions.Count > index)
                return (MobAction) actions[index];
            
            Debug.LogWarning($"Trying to get the action with index '{index}' of the Item '{this.name} but the size of the array is {actions.Count}.");
            return null;
        }
        
        public MobAction GetActionToCoverNeed(Need need, MapElement mapElement, out Vector3 positionToPerformAction)
        {
            foreach (IMobAction iMobAction in actions)
            {
                MobAction mobAction = (MobAction) iMobAction;
                //MobAction action = (MobAction) Activator.CreateInstance(actionType.GetType());
                if (mobAction.SatisfiesNeed(need))
                {
                    positionToPerformAction = mapElement.transform.position;
                    return mobAction;
                }

            }

            positionToPerformAction = Vector3.zero;
            return null;
        }
    }
    
}

