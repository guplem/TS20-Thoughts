using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using Object = System.Object;

namespace Thoughts.Needs
{
    [CreateAssetMenu(fileName = "Item", menuName = "Thoughts/Need", order = 2)]
    public class Need: ScriptableObject, IEquatable<Need>
    {

        [SerializeField] public int level = 0; //Todo: switch to an enumerator with the levels named

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
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Need) obj);
        }
        public bool Equals(Need other)
        {
            return other != null && other.name.Equals(this.name);
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        public static bool operator ==(Need left, Need right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Need left, Need right)
        {
            return !Equals(left, right);
        }

    }
    
}