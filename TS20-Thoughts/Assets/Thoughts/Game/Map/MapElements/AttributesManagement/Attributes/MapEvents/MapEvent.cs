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
        [SerializeField] public bool executerMustOwnAttribute = false; //TODO. Check if needed. It is used in "Drop"
        [SerializeField] public List<AttributeUpdate> consequences = new List<AttributeUpdate>();
        [SerializeField] public List<AttributeUpdate> requirements = new List<AttributeUpdate>();
        [Tooltip("If false, in case the requirements are not met nd the event can not be executed, this event is going to be ignored (so no map element is going to 'try' to fix the requirements so it can be executed.")]
        [SerializeField] public bool tryToCoverRequirementsIfNotMet = true;


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
            return  (name.IsNullEmptyOrWhiteSpace() ? (this.GetType().Name + " (no name)") : name) ;
        }
        
        public bool IsDistanceMet(MapElement eventOwner, MapElement executer)
        {
            if (maxDistance < 0)
                return true;
        
            Vector3 eventOwnerPosition = eventOwner.transform.position;
            Vector3 executerPosition = executer.transform.position;
            return Vector3.Distance(eventOwnerPosition, executerPosition) <= maxDistance;
        }
        
        public List<OwnedAttribute> GetRequirementsNotMet(MapElement eventOwner, MapElement executer, MapElement target)
        {
            List<OwnedAttribute> requirementsNotMet = new List<OwnedAttribute>();
            
            foreach (AttributeUpdate requirement in requirements)
            {
                switch (requirement.affected)
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        if(!eventOwner.attributeManager.Meets(requirement))
                            requirementsNotMet.Add(eventOwner.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                        if(!executer.attributeManager.Meets(requirement))
                            requirementsNotMet.Add(executer.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                        if(!target.attributeManager.Meets(requirement))
                            requirementsNotMet.Add(target.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return requirementsNotMet;
        }

        public bool ConsequencesCover(OwnedAttribute ownedAttribute, MapElement target, MapElement executer, MapElement owner)
        {
            bool consequenceCoversOwnerOfAttribute = false;
            // Debug.Log($"$$$$$ Checking if consequences of '{name}' cover '{ownedAttribute.attribute}'.\n");
            foreach (AttributeUpdate consequence in consequences)
            {
                //Debug.Log($"    $$$$$ Current consequence's attribute = '{consequence.attribute}'.\n");
                if (consequence.attribute == ownedAttribute.attribute && consequence.value > 0)
                {
                    switch (consequence.affected)
                    {
                        case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                            consequenceCoversOwnerOfAttribute = ownedAttribute.ownerMapElement == owner;
                            break;
                        case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                            consequenceCoversOwnerOfAttribute = ownedAttribute.ownerMapElement == executer;
                            break;
                        case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                            consequenceCoversOwnerOfAttribute = ownedAttribute.ownerMapElement == target;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (consequenceCoversOwnerOfAttribute)
                    {
                        if (consequence.affected == AttributeUpdate.AttributeUpdateAffected.eventTarget)
                        {
                            // The 'target' must not be the 'executer' neither the 'owner'
                            if (target == executer || target == owner)
                                return false;
                            return true;
                        }
                        else
                        {
                            return true;
                        }
                    }

                }
            }
            
            return false;
        }
    }
}