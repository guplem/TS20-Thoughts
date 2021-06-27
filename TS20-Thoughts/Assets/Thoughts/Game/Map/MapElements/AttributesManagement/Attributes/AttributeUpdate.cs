using System;
using Thoughts.Game.GameMap;
using UnityEngine.Serialization;

namespace Thoughts.Game.Attributes
{

    [Serializable]
    public class AttributeUpdate
    {
        public Thoughts.Game.Attributes.Attribute attribute;
        public int value = 1;
        [FormerlySerializedAs("affectedMapEvent")]
        [FormerlySerializedAs("affected")]
        public AffectedMapElement affectedMapElement = AffectedMapElement.eventOwner;

        public override string ToString()
        {
            return $"{attribute} (val={value})";
        }

    }
}