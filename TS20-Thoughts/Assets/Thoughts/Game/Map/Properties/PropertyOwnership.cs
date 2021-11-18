using System;
using Thoughts.Game.Map.MapElements;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map.Properties
{
    /// <summary>
    /// A relation between an Property and a MapElement that owns it and a value of that property.
    /// The owner can be null if it is not owned by anybody but it is only treated as a property-value relationship.
    /// </summary>
    [Serializable]
    public class PropertyOwnership
    {
        /// <summary>
        /// The MapElement that owns the property.
        /// </summary>
        public MapElement owner { get; private set; }
        
        /// <summary>
        /// The Property that is owned.
        /// </summary>
        [SerializeField] public Property property;
        
        /// <summary>
        /// The current value this relationship (The value of the Property when by the established MapElement).
        /// </summary>
        [SerializeField] private float _value;
        public float value { get => _value; private set { _value = value; } }
        
        /// <summary>
        /// Indicates whether or not the owner should try to keep the value of this relationship with higher than 0.
        /// </summary>
        public bool takeCare => property.behaviourWhenEmpty == BehaviourWhenEmpty.TakeCare;

        /// <summary>
        /// Sets up a new owner for this relationship.
        /// </summary>
        /// <param name="newOwner">The new owner.</param>
        public void UpdateOwner(MapElement newOwner)
        {
            this.owner = newOwner;
        }

        /// <summary>
        /// Updates the value of this relationship.
        /// </summary>
        /// <param name="deltaValue">The difference that is wanted to apply to the current value of the relationship. Can be positive and negative.</param>
        public void UpdateValue(float deltaValue)
        {
            this.value += deltaValue;
        }

        /// <summary>
        /// The class constructor. Creates a relationship between a MapElement and an Property that is owned by the first.
        /// </summary>
        /// <param name="property">The property that is going to be owned thanks to this relationship.</param>
        /// <param name="value">The initial value of this relationship.</param>
        /// <param name="owner">The MapEvent that is going to be set as the owner in this relation.</param>
        public PropertyOwnership(Property property, float value, MapElement owner)
        {
            this.property = property;
            this.value = value;
            this.owner = owner;
            
            if (value < 0)
                Debug.LogWarning($"A new PropertyOwnership has been created with a negative value: {this.ToString()}");
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"PO: '{property}' owned by '{owner}' - value={value}, TakeCare={takeCare}";
        }

        /// <summary>
        /// Indicates whether the property has a value of 0 or less and it has been indicated that it is intended to have the value always higher than 0 (by the takeCare property)
        /// </summary>
        /// <returns>True, if the value is less or equal to 0 and the property takeCare is set to true. Otherwise, false.</returns>
        public bool NeedsCare()
        {
            return value <= 0 && takeCare;
        }

    }
}