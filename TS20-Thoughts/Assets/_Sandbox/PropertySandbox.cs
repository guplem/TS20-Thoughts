using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thoughts.Game.Map.MapEvents;
using UnityEngine;

namespace _Sandbox
{
    /// <summary>
    /// A quality, characteristic or object ascribed to a MapElement.
    /// </summary>
    [CreateAssetMenu(fileName = "PropertySandbox", menuName = "Thoughts/Sandbox/PropertySandbox", order = 1)]
    public class PropertySandbox : SerializedScriptableObject, IEquatable<PropertySandbox>
    {
        /// <summary>
        /// What should be done when the value of this propertySandbox is 0 in a MapElement
        /// </summary>
        public BehaviourWhenEmpty behaviourWhenEmpty { get { return _behaviourWhenEmpty; } }
        
        /// <summary>
        /// What should be done when the value of this propertySandbox is 0 in a MapElement
        /// </summary>
        [Tooltip("What should be done when the value of this propertySandbox is 0 in a MapElement")]
        [SerializeField] private BehaviourWhenEmpty _behaviourWhenEmpty = BehaviourWhenEmpty.Remove;
        
        /// <summary>
        /// The level of priority of the propertySandbox.
        /// <para>More than 0 if it is a need. 0 otherwise.</para>
        /// </summary>
        public NeedPriority needPriority { get { return _needPriority; } }
        
        /// <summary>
        /// The level of priority of the propertySandbox.
        /// <para>More than 0 if it is a need. 0 otherwise.</para>
        /// </summary>
        [Tooltip("The level of priority, if it is a need.")]
        [ShowIf("_behaviourWhenEmpty", BehaviourWhenEmpty.TakeCare)]
        [SerializeField] private NeedPriority _needPriority = NeedPriority.None;
        
        /// <summary>
        /// The MapEvents made available by the PropertySandbox.
        /// </summary>
        [Tooltip("The MapEvents made available by the PropertySandbox.")]
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
        /// The levels of priority an propertySandbox can have.
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
            /// <para>Examples: Personal security, employment, resources, heath, propertySandbox, ...</para>
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
        /// Indicates if this PropertySandbox is more prioritary than another given PropertySandbox.
        /// </summary>
        /// <param name="propertySandbox">The PropertySandbox against which to check the priority.</param>
        /// <returns>True, if this PropertySandbox is more prioritary than the given PropertySandbox. False, otherwise.</returns>
        public bool IsMorePrioritaryThan(PropertySandbox propertySandbox)
        {
            if (propertySandbox == null)
                return true;
            
            if (!this.IsNeed())
                return false;

            if (!propertySandbox.IsNeed())
                return true;

            return this.needPriority < propertySandbox.needPriority;
        }

        /// <summary>
        /// Indicates if this PropertySandbox is considered a Need.
        /// </summary>
        /// <returns>True if the level of priority is > 0 (meaning that it is a need). False, otherwise.</returns>
        public bool IsNeed()
        {
            return this.needPriority != NeedPriority.None;
        }
        
    #region EqualityComparer
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// <para>The equality is considered checking only the name of the Properties.</para>
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
            return Equals((PropertySandbox) obj);
        }
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// <para>The equality is considered checking only the name of the Properties.</para>
        /// </summary>
        /// <param name="other">The object to check against</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(PropertySandbox other)
        {
            return other != null && other.name.Equals(this.name);
        }
        
        /// <summary>
        /// Override to the equal operator so two Properties are considered the same if their names are the same.
        /// <para>This is because the Equals method is used, and it uses the GetHasCode method to compare equality while it uses the name to obtain it. </para>
        /// </summary>
        /// <returns>True if the left object's name is equal to the right object's name; otherwise, false.</returns>
        public static bool operator ==(PropertySandbox left, PropertySandbox right)
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
        /// Override to the mot-equal operator so two Properties are considered different the same if their names are different.
        /// <para>This is because the Equals method is used, and it uses the GetHasCode method to compare equality while it uses the name to obtain it. </para>
        /// </summary>
        /// <returns>True if the left object's name is different to the right object's name; otherwise, false.</returns>
        public static bool operator !=(PropertySandbox left, PropertySandbox right)
        {
            return !(left == right);
        }
    #endregion
        
    }

    /// <summary>
    /// What should be done when the value of this propertySandbox is 0 in a MapElement
    /// </summary>
    public enum BehaviourWhenEmpty
    {
        /// <summary>
        /// Remove the PropertySandboxOwnership from the MapElement 
        /// </summary>
        Remove = 0,
        /// <summary>
        /// Try to increase the value of the PropertySandboxOwnership from the MapElement
        /// </summary>
        TakeCare = 1,
        /// <summary>
        /// Do nothing, leave it with a value of 0
        /// </summary>
        DoNothing = 2
    }
    
}