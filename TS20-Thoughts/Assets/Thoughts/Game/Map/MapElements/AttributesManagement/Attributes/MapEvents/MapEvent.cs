using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    [Serializable]
    public class MapEvent
    {
        [SerializeField] public string name = "";
        [SerializeField] public float maxDistance = 5f;
        [SerializeField] public bool executeWithTimeElapse = false;
        [SerializeField] public List<AttributeUpdate> consequences = new List<AttributeUpdate>();
        [SerializeField] public List<AttributeUpdate> requirements = new List<AttributeUpdate>();
        
        public Attribute ownerAttribute { get; private set; }
        public void UpdateOwner(Attribute newOwner)
        {
            ownerAttribute = newOwner;
        }

        public void Execute(MapElement executer, MapElement target)
        {
            Debug.Log($"        · MapElement '{executer}' is executing '{name}' of '{ownerAttribute}' with target '{target}'.");

            foreach (AttributeUpdate attributeUpdate in consequences)
            {
                switch (attributeUpdate.affected)
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        MapElement ownerMapElement = ownerAttribute.ownerAttributeManager.ownerMapElement;
                        ownerMapElement.UpdateAttribute(attributeUpdate.attribute, attributeUpdate.value);
                    break;
                    case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                        executer.UpdateAttribute(attributeUpdate.attribute, attributeUpdate.value);
                    break;
                    case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                        target.UpdateAttribute(attributeUpdate.attribute, attributeUpdate.value);
                    break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public override string ToString()
        {
            return this.GetType().Name + (name.IsNullEmptyOrWhiteSpace() ? "" : $" ({name})") ;
        }

    }
}