using System;

namespace Thoughts.Game.Attributes
{
    [Serializable]
    public class AttributeUpdate
    {
        public Thoughts.Game.Attributes.Attribute attribute;
        public int value = 1;
        public AttributeUpdateAffected affected = AttributeUpdateAffected.eventOwner;

        public enum AttributeUpdateAffected
        {
            eventOwner,
            eventExecuter,
            eventTarget
        }

        public override string ToString()
        {
            return $"{attribute} (val={value})";
        }

    }
}