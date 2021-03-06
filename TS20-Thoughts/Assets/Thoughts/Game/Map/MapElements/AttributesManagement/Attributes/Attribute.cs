using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Game.Attributes
{
    /// <summary>
    /// A quality, characteristic or object ascribed to a MapElement.
    /// </summary>
    [CreateAssetMenu(fileName = "Attribute", menuName = "Thoughts/Attribute", order = 1)]
    public class Attribute : ScriptableObject, IEquatable<Attribute>
    {
        /// <summary>
        /// The level of priority of the attribute.
        /// <para>More than 0 if it is a need. 0 otherwise.</para>
        /// </summary>
        public NeedPriority needPriority { get { return _needPriority; } }
        
        /// <summary>
        /// The level of priority of the attribute.
        /// <para>More than 0 if it is a need. 0 otherwise.</para>
        /// </summary>
        [Tooltip("The level of priority, if it is a need.")]
        [SerializeField] private NeedPriority _needPriority = NeedPriority.None;
        
        /// <summary>
        /// The MapEvents made available by the Attribute.
        /// </summary>
        [Tooltip("The MapEvents made available by the Attribute.")]
        [SerializeField] public List<MapEvent> mapEvents;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return name;
        }
        
        /// <summary>
        /// The levels of priority an attribute can have.
        /// </summary>
        public enum NeedPriority
        {
            /// <summary>
            /// Level of priority = 5. MINIMUM level of priority
            /// <para>Examples: Becoming the most that one can be, creative activities, ...</para>
            /// </summary>
            SelfActualization = 5,
            /// <summary>
            /// Level of priority = 4.
            /// <para>Examples: Respect,Self-esteem, status, recognition/prestige, strength, freedom, accomplishment, ...</para>
            /// </summary>
            Esteem = 4,
            /// <summary>
            /// Level of priority = 3.
            /// <para>Examples: Friendship, intimacy, family, sense of connection, ...</para>
            /// </summary>
            Love = 3,
            /// <summary>
            /// Level of priority = 2.
            /// <para>Examples: Personal security, employment, resources, heath, property, ...</para>
            /// </summary>
            Safety = 2,
            /// <summary>
            /// Level of priority = 1. MAXIMUM level of priority
            /// <para>Examples: Hydration, satiety, shelter, sleep/Rest, clothing, reproduction, warmth, ...</para>
            /// </summary>
            Physiological = 1,
            /// <summary>
            /// Level of priority = 0. No priority. If it is not a need, it should have this level of priority
            /// <para>Examples: Objects like water (not hydration), wood, berries (not satiety), ...</para>
            /// </summary>
            None = 0 
        }
        
        /// <summary>
        /// Indicates if this Attribute is more prioritary than another given Attribute.
        /// </summary>
        /// <param name="attribute">The Attribute against which to check the priority.</param>
        /// <returns>True, if this Attribute is more prioritary than the given Attribute. False, otherwise.</returns>
        public bool IsMorePrioritaryThan(Attribute attribute)
        {
            if (attribute == null)
                return true;
            
            if (!this.IsNeed())
                return false;

            if (!attribute.IsNeed())
                return true;

            return this.needPriority < attribute.needPriority;
        }

        /// <summary>
        /// Indicates if this Attribute is considered a Need.
        /// </summary>
        /// <returns>True if the level of priority is > 0 (meaning that it is a need). False, otherwise.</returns>
        public bool IsNeed()
        {
            return this.needPriority != NeedPriority.None;
        }
        
    #region EqualityComparer
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// <para>The equality is considered checking only the name of the Attributes.</para>
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
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
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// <para>The equality is considered checking only the name of the Attributes.</para>
        /// </summary>
        /// <param name="other">The object to check against</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(Attribute other)
        {
            return other != null && other.name.Equals(this.name);
        }
        
        /// <summary>
        /// Returns the hash code for the object (given by its name).
        /// </summary>
        /// <returns>The hash code for the object.</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        
        /// <summary>
        /// Override to the equal operator so two Attributes are considered the same if their names are the same.
        /// <para>This is because the Equals method is used, and it uses the GetHasCode method to compare equality while it uses the name to obtain it. </para>
        /// </summary>
        /// <returns>True if the left object's name is equal to the right object's name; otherwise, false.</returns>
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
        
        /// <summary>
        /// Override to the mot-equal operator so two Attributes are considered different the same if their names are different.
        /// <para>This is because the Equals method is used, and it uses the GetHasCode method to compare equality while it uses the name to obtain it. </para>
        /// </summary>
        /// <returns>True if the left object's name is different to the right object's name; otherwise, false.</returns>
        public static bool operator !=(Attribute left, Attribute right)
        {
            return !(left == right);
        }
    #endregion
        
    }



}

