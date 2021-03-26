using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Needs
{
    [CreateAssetMenu(fileName = "Item", menuName = "Thoughts/Need", order = 2)]
    public class Need: ScriptableObject, IComparable<Need>
    {
        
        [SerializeField] public int level = 0; //Todo: switch to an enumerator with the levels named

        public int CompareTo(Need other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            return other.level.CompareTo(level);
        }

        public override string ToString()
        {
            return $"{this.name}: L={level}";
        }
        
        public virtual List<MapAction> GetActionsSatisfy(MapElement needyMapElement, List<MapAction> actions = null, int iteration = 0)
        {
            if (iteration >= 100)
            {
                Debug.LogWarning($"Iteration {iteration}");
                return null;
            }


            if (actions == null)
                actions = new List<MapAction>();

            Vector3 positionToPerformAction;
            MapAction actionToCoverNeed = AppManager.gameManager.map.FindActionToCoverNeed(this, out positionToPerformAction);

            if (actionToCoverNeed == null)
                return null;

            actions.Add(actionToCoverNeed);

            if (!actionToCoverNeed.NeedsToExecuteAreCovered(needyMapElement))
                return GetActionsSatisfy(needyMapElement, actions, iteration + 1);

            return actions;
        }
        
    }
    
}