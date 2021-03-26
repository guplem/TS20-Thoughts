using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.GameMap;
using UnityEngine;
using Object = System.Object;

namespace Thoughts.Needs
{
    [CreateAssetMenu(fileName = "Item", menuName = "Thoughts/Need", order = 2)]
    public class Need: ScriptableObject, IEquatable<Need>, IComparer<Need>
    {

        [SerializeField] public int priority = 50; //Todo: switch to an enumerator with the levels named (?)

        public override string ToString()
        {
            return $"{this.name}: L={priority}";
        }
        
        public virtual List<MapActionFromMapElement> GetActionsSatisfy(MapElement needyMapElement, List<MapActionFromMapElement> actions = null, int iteration = 0)
        {
            if (iteration >= 100)
            {
                Debug.LogWarning($"Iteration {iteration} reached to get an action path to take care of the need '{this.name}'.");
                actions.DebugLog( ",","  \\_Found actions so far: ");
                return null;
            }
            
            if (actions == null)
                actions = new List<MapActionFromMapElement>();

            //Vector3 positionToPerformAction;
            MapElement mapElementWithActionToCoverNeed = null;
            MapAction actionToCoverNeed = AppManager.gameManager.map.FindActionToCoverNeed(this, needyMapElement, out mapElementWithActionToCoverNeed);
            // TODO: Use the latest action's map element to find the closest nect action to the previous one
            //MapAction actionToCoverNeed = AppManager.gameManager.map.FindActionToCoverNeed(this, actions.Count == 0 ? needyMapElement : actions.ElementAt(actions.Count() - 1).mapElement);

            if (actionToCoverNeed != null)
                actions.Add(new MapActionFromMapElement(actionToCoverNeed, mapElementWithActionToCoverNeed));

            if (actionToCoverNeed != null)
            {
                List<Need> needsToPerformAction = actionToCoverNeed.GetNotSatisfiedRequiredNeedsBy(needyMapElement);
                if (needsToPerformAction != null && needsToPerformAction.Count > 0)
                    //foreach (Need needToPerformAction in needsToPerformAction) //To-Do: multiple enqueued lists
                    return needsToPerformAction.ElementAt(0).GetActionsSatisfy(needyMapElement, actions, iteration + 1);
            }
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
        public int Compare(Need x, Need y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (ReferenceEquals(null, y))
                return 1;
            if (ReferenceEquals(null, x))
                return -1;
            return x.priority.CompareTo(y.priority);
        }
    }
    
}