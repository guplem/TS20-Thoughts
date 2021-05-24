using System;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Game.Attributes
{

    [Serializable]
    public class OwnedAttribute
    {
        public MapElement ownerMapElement { get; private set; }
        [SerializeField] public Thoughts.Game.Attributes.Attribute attribute;
        [SerializeField] private int _value;
        public int value { get => _value; private set { _value = value; } }
        [SerializeField] private bool _takeCare;
        public bool takeCare { get => _takeCare; private set { _takeCare = value; } }

        public void UpdateOwner(MapElement newOwner)
        {
            this.ownerMapElement = newOwner;
        }

        public void UpdateValue(int deltaValue)
        {
            this.value += deltaValue;
        }

        public OwnedAttribute(Attributes.Attribute attribute, int value, MapElement ownerMapElement, bool takeCare = false)
        {
            this.attribute = attribute;
            this.value = value;
            this.takeCare = takeCare;
            this.ownerMapElement = ownerMapElement;
        }

        public override string ToString()
        {
            return $"{ownerMapElement}' is owner of an attribute '{attribute}' that has a value of {value}.  TakeCare = {takeCare}.";
        }

        public bool NeedsCare()
        {
            return value <= 0 && takeCare;
        }

    }
}