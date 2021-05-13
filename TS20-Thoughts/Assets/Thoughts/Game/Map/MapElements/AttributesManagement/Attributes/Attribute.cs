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
        public NeedPriority needPriority => _needPriority;
        [SerializeField] private NeedPriority _needPriority = NeedPriority.None;
        
        [SerializeField] public List<MapEvent> mapEvents;

        public override string ToString()
        {
            return name;
        }
        
        /*
            5. Self-actualization (SelfActualization - Priority = 5)
            	- Becoming the most that one can be
            	- Creative activities
            	
            4. Esteem (Esteem - Priority = 4)
            	- Respect
            	- Self-esteem
            	- Status
            	- Recognition/Prestige
            	- Strength
            	- Freedom
            	- Accomplishment
            	
            3. Love and Belonging (Love - Priority = 3)
            	- Friendship
            	- Intimacy
            	- Family
            	- Sense of connection
            	
            2. Safety (Safety - Priority = 2)
            	- Personal security
            	- Employment
            	- Resources
            	- Heath
            	- Property
            	
            1. Physiological (Physiological - Priority = 1)
            	- Water
            	- Food
            	- Shelter
            	- Sleep/Rest
            	- Clothing
            	- Reproduction
            	- Warmth
            	
            0. Not a need (None - Priority = 0)
         */
        
        public enum NeedPriority
        {
            SelfActualization = 5,    // MINIMUM priority
            Esteem = 4,
            Love = 3,
            Safety = 2,
            Physiological = 1,        // MAXIMUM priority
            None = 0 // No priority
        }
        
        public bool IsMorePrioritaryThan(Attribute attribute)
        {
            if (this.needPriority == NeedPriority.None)
                return false;

            if (attribute.needPriority == NeedPriority.None)
                return true;

            return this.needPriority < attribute.needPriority;
        }
        
    #region EqualityComparer
        
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
        
    }



}

