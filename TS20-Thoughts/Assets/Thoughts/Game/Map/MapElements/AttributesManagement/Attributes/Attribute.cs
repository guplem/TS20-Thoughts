using System;
using System.Collections.Generic;
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
        
        public new string name => base.name;
        
        public List<ExecutionPlan> GetExecutionPlanToCoverThisAttribute(MapElement attributeOwnergi)
        {
            // Look for all MapEvents that, as consequence of the event, they make the attribute value increase.
                //Be aware that the event consequence can be in 'mode' target, owner or executer 
            throw new NotImplementedException();
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
            return Equals(left, right);
        }
        
        public static bool operator !=(Attribute left, Attribute right)
        {
            return !Equals(left, right);
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

