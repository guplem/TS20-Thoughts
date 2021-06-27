using System;
using Thoughts.Game.GameMap;

namespace Thoughts.Game.Attributes
{

    [Serializable]
    public class Consequence
    {
        public Thoughts.Game.Attributes.Attribute attribute;
        public int value = 1;
        public AffectedMapElement affectedMapElement = AffectedMapElement.eventOwner;

        public override string ToString()
        {
            return $"{attribute} (val={value})";
        }

    }
}
