using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.GameMap;
using UnityEngine;
using Object = System.Object;

namespace Thoughts.Needs
{
    [CreateAssetMenu(fileName = "Stat", menuName = "Thoughts/Stat", order = 2)]
    public class Stat: ScriptableObject, IEquatable<Stat>, IComparer<Stat>
    {

        [SerializeField] public int priority = 50; //Todo: switch to an enumerator with the levels named (?)

        public override string ToString()
        {
            //return $"{this.name}: L={priority}";
            return this.name;
        }
        
        public virtual List<MapEventFromAttributeAtMapElement> GetEventsToSatisfyThisStat(MapElement needyMapElement, List<MapEventFromAttributeAtMapElement> mapEventsFromMapElements = null, int iteration = 0)
        {
            // Check if infinite loop
            if (iteration >= 50)
            {
                Debug.LogWarning($"Iteration {iteration} reached to get an action path to take care of the need '{this.name}'.");
                mapEventsFromMapElements.DebugLog( ",","  \\_Events found so far: ");
                return null;
            }
            
            
            // Get the event to cover the stat and were to perform it (MapElement)
            MapElement mapElementWithEventToCoverNeed = null;
            Attribute attributeWithEventToCoverNeed = null;
            MapEvent eventToCoverNeedyStat = AppManager.gameManager.map.FindEventToCoverStat(this, needyMapElement, out mapElementWithEventToCoverNeed, out attributeWithEventToCoverNeed); // TODO: Use the latest event's map element to find the closest next event to the previous one
            
            // An event to cover the needed stat has been found
            if (eventToCoverNeedyStat != null)
            {
                // Create returned list
                if (mapEventsFromMapElements == null)
                    mapEventsFromMapElements = new List<MapEventFromAttributeAtMapElement>();
                
                // Add the found event to the returned list
                mapEventsFromMapElements.Add(new MapEventFromAttributeAtMapElement(eventToCoverNeedyStat, attributeWithEventToCoverNeed, mapElementWithEventToCoverNeed));
                
                // Check if the event can be executed, if not, search how to solve the needed stat
                List<Stat> needsToPerformAction = eventToCoverNeedyStat.GetRequiredNeedsNotSatisfiedBy(needyMapElement, mapElementWithEventToCoverNeed);
                if (needsToPerformAction != null && needsToPerformAction.Count > 0)
                    //foreach (Need needToPerformAction in needsToPerformAction) //ToDo: multiple enqueued lists - More than one stat might need to be covered
                    return needsToPerformAction.ElementAt(0).GetEventsToSatisfyThisStat(needyMapElement, mapEventsFromMapElements, iteration + 1);
            }
            
            return mapEventsFromMapElements;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Stat) obj);
        }
        public bool Equals(Stat other)
        {
            return other != null && other.name.Equals(this.name);
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        public static bool operator ==(Stat left, Stat right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Stat left, Stat right)
        {
            return !Equals(left, right);
        }
        public int Compare(Stat x, Stat y)
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