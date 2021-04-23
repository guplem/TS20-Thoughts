using System;
using System.Collections.Generic;
using Thoughts.Game;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts
{
    [CreateAssetMenu(fileName = "Attribute", menuName = "Thoughts/Attribute", order = 1)]
    public class Attribute : ScriptableObject, IEquatable<Attribute>
    {
        
    #region Funtionallity

        [SerializeField] public float value;
        [SerializeField] public float minValue;
        [SerializeField] public bool takeCare;
        [SerializeField] public AttributeType type = AttributeType.obj;
        [Space(20)]
        [SerializeField] public List<MapEvent> mapEvents;

        public AttributeManager ownerAttributeManager { get; private set; }
        public void UpdateOwner(AttributeManager newOwner)
        {
            ownerAttributeManager = newOwner;
            foreach (MapEvent mapEvent in mapEvents)
            {
                mapEvent.UpdateOwner(this);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeOwner">Map element that has the attribute</param>
        /// <param name="caregiver">Map element that wants to take care og the attribute</param>
        /// <returns></returns>
        public List<ExecutionPlan> GetExecutionPlanToCoverThisAttribute(MapElement caregiver)
        {
            // Look for all MapEvents that, as consequence of the event, they make the attribute value increase.
            //Be aware that the event consequence can be in 'mode' target, owner or executer 

            List<ExecutionPlan> mapEventsToTakeCare = new List<ExecutionPlan>();
            MapEvent foundMapEvent = null;
            
            //Trying to take care of an own attribute (in the same MapElement)
            if (ownerAttributeManager.ownerMapElement == caregiver)
            {
                // 1. Try to solve it with an own event that affect the owner
                foundMapEvent = caregiver.GetMapEventToTakeCareOf(this, AttributeUpdate.AttributeUpdateAffected.eventOwner);
                if (foundMapEvent == null)
                    foundMapEvent = caregiver.GetMapEventToTakeCareOf(this, AttributeUpdate.AttributeUpdateAffected.eventTarget);
                if (foundMapEvent == null)
                    foundMapEvent = caregiver.GetMapEventToTakeCareOf(this, AttributeUpdate.AttributeUpdateAffected.eventExecuter);
                
                // 2. Try to solve it with an external event that affect the executer or the target
                if (foundMapEvent == null)
                    foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(this, AttributeUpdate.AttributeUpdateAffected.eventExecuter);
                if (foundMapEvent == null)
                    foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(this, AttributeUpdate.AttributeUpdateAffected.eventTarget);
            }
            else // Trying to take care of an attribute in another MapElement
            {
                // 3. Try to solve it with an external event that affect the executer or the target
                foundMapEvent = caregiver.GetMapEventToTakeCareOf(this, AttributeUpdate.AttributeUpdateAffected.eventTarget);
                if (foundMapEvent == null)
                    foundMapEvent = AppManager.gameManager.map.GetExecutionPlanToTakeCareOf(this, AttributeUpdate.AttributeUpdateAffected.eventTarget);
            }

            if (foundMapEvent != null)
                mapEventsToTakeCare.Add(new ExecutionPlan(foundMapEvent, caregiver, ownerAttributeManager.ownerMapElement));
            
            //Todo: check if can be performed, if not, try to solve the incenvinience
            return mapEventsToTakeCare;
        }
        
        /*public void AlterQuantity(int quantity)
        {
            foreach (RelatedStat relatedStat in relatedStats)
            {
                if (relatedStat.stat.name == "Quantity")
                {
                    relatedStat.satisfactionAmount += quantity;
                    Debug.Log($"     The new quantity is {relatedStat.satisfactionAmount}");
                }
            }
        }*/

        /*public MapEvent GetAction(int index)
        {
            if (mapEvents.Count > index)
                return (MapEvent) mapEvents[index];
            
            Debug.LogWarning($"Trying to get the action with index '{index}' of the Item '{this.name} but the size of the array is {mapEvents.Count}.");
            return null;
        }*/
        
        /*public MapEvent GetMapEventToCoverAttribute(Attribute attribute, MapElement mapElement)
        {
            throw new NotImplementedException();
            return null;
        }*/
    
    #endregion
        
    #region Comparasions
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Attribute) obj);
        }
        
        public bool Equals(Attribute other)
        {
            return other != null && other.name.Equals(this.name);
        }
        
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        
        public static bool operator ==(Attribute left, Attribute right)
        {
            if (left is null && right is null)
                return true;
            if ((left is null) && !(right is null))
                return false;
            if (!(left is null) && (right is null))
                return false;
            return left.Equals(right);
        }
        
        public static bool operator !=(Attribute left, Attribute right)
        {
            return !(left == right);
        }
    #endregion

        
        public enum AttributeType
        {
            obj, // 'object' is taken
            feature,
            posession
        }
    }
    
}

