using System;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Game.Attributes
{
    /// <summary>
    /// A relation between an Attribute and a MapElement that owns it.
    /// </summary>
    [Serializable]
    public class AttributeOwnership
    {
        /// <summary>
        /// The MapElement that owns the attribute.
        /// </summary>
        public MapElement owner { get; private set; }
        
        /// <summary>
        /// The Attribute that is owned.
        /// </summary>
        [SerializeField] public Attribute attribute;
        
        /// <summary>
        /// The current value this relationship (The value of the Attribute when by the established MapElement).
        /// </summary>
        [SerializeField] private int _value;
        public int value { get => _value; private set { _value = value; } }
        
        /// <summary>
        /// Indicates whether or not the owner should try to keep the value of this relationship with higher than 0.
        /// </summary>
        [SerializeField] private bool _takeCare;
        public bool takeCare { get => _takeCare; private set { _takeCare = value; } }

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
        public void UpdateValue(int deltaValue)
        {
            this.value += deltaValue;
        }

        /// <summary>
        /// The class constructor. Creates a relationship between a MapElement and an Attribute that is owned by the first.
        /// </summary>
        /// <param name="attribute">The attribute that is going to be owned thanks to this relationship.</param>
        /// <param name="value">The initial value of this relationship.</param>
        /// <param name="owner">The MapEvent that is going to be set as the owner in this relation.</param>
        /// <param name="takeCare">Should the owner try to keep the value of this relationship higher than 0?</param>
        public AttributeOwnership(Attributes.Attribute attribute, int value, MapElement owner, bool takeCare = false)
        {
            this.attribute = attribute;
            this.value = value;
            this.owner = owner;
            this.takeCare = takeCare;
            
            if (value < 0)
                Debug.LogWarning($"A new AttributeOwnership has been created with a negative value: {this.ToString()}");
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{owner}' is owner of an attribute '{attribute}' that has a value of {value}.  TakeCare = {takeCare}.";
        }

        /// <summary>
        /// Indicates whether the attribute has a value of 0 or less and it has been indicated that it is intended to have the value always higher than 0 (by the takeCare property)
        /// </summary>
        /// <returns>True, if the value is less or equal to 0 and the property takeCare is set to true. Otherwise, false.</returns>
        public bool NeedsCare()
        {
            return value <= 0 && takeCare;
        }

    }
}