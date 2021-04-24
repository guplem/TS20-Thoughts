using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    [Serializable]
    public class MapEvent
    {
        [SerializeField] public string name = "";
        [SerializeField] public float maxDistance = 5f; // Ignored if it is <0 or if 'executeWithTimeElapse' is true
        [SerializeField] public bool executeWithTimeElapse = false;
        [SerializeField] public bool executerMustOwnAttribute = false;
        [SerializeField] public List<AttributeUpdate> consequences = new List<AttributeUpdate>();
        [SerializeField] public List<AttributeUpdate> requirements = new List<AttributeUpdate>();

        public void Execute(MapElement executer, MapElement target, MapElement owner)
        {
            // Debug.Log($"        · MapElement '{executer}' is executing '{name}' of '{ownerAttribute}' with target '{target}'.");

            foreach (AttributeUpdate attributeUpdate in consequences)
            {
                switch (attributeUpdate.affected)
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        owner.UpdateAttribute(attributeUpdate.attribute, attributeUpdate.value);
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
            return  (name.IsNullEmptyOrWhiteSpace() ? (this.GetType().Name + " (no name)") : $"{name}") ;
        }
        
        public bool IsDistanceMet(MapElement eventOwner, MapElement executer)
        {
            if (maxDistance < 0)
                return true;
        
            Vector3 eventOwnerPosition = eventOwner.transform.position;
            Vector3 executerPosition = executer.transform.position;
            return Vector3.Distance(eventOwnerPosition, executerPosition) <= maxDistance;
        }
        
        public bool AreRequirementsMet(MapElement eventOwner, MapElement executer, MapElement target)
        {
            bool result;
            foreach (AttributeUpdate requirement in requirements)
            {
                switch (requirement.affected)
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        result = eventOwner.attributeManager.Meets(requirement);
                        if (!result) return false;
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                        result = executer.attributeManager.Meets(requirement);
                        if (!result) return false;
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                        result = target.attributeManager.Meets(requirement);
                        if (!result) return false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return true;
        }

    }
}