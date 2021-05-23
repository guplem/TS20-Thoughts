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
        [Tooltip("If false, in case the requirements are not met and the event can not be executed, this event is going to be ignored (so no map element is going to 'try' to fix the requirements so it can be executed.")]
        [SerializeField] public bool tryToCoverRequirementsIfNotMet = true;
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
            return  (name.IsNullEmptyOrWhiteSpace() ? (this.GetType().Name + " (no name)") : name) ;
        }
        
        public bool IsDistanceMet(MapElement target, MapElement eventOwner, MapElement executer)
        {
            if (maxDistance < 0)
                return true;
        
            Vector3 eventOwnerPosition = eventOwner.transform.position;
            
            Vector3 executerPosition = executer.transform.position;
            float distanceOwnerExecuter = Vector3.Distance(eventOwnerPosition, executerPosition);
            

            Vector3 targetPosition = target.transform.position;
            float distanceTargetExecuter = Vector3.Distance(eventOwnerPosition, targetPosition);

            float currentMaxDistance = Mathf.Max(distanceTargetExecuter, distanceOwnerExecuter);
            //Debug.Log($"CURRENT MAX DISTANCE = {currentMaxDistance}");
            return currentMaxDistance <= maxDistance;
        }
        
        public List<OwnedAttribute> GetRequirementsNotMet(MapElement eventOwner, MapElement executer, MapElement target, int executionTimes, out List<int> remainingValueToCoverRequirementsNotMet)
        {
            List<OwnedAttribute> requirementsNotMet = new List<OwnedAttribute>();
            remainingValueToCoverRequirementsNotMet = new List<int>();
            
            foreach (AttributeUpdate requirement in requirements)
            {
                //OwnedAttribute attributeThatMostCloselyMeetsTheRequirement;
                int remainingValueToCoverRequirementNotMet;
                bool meets = true;

                switch (requirement.affected)
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        meets = eventOwner.attributeManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            requirementsNotMet.Add(eventOwner.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                            remainingValueToCoverRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                        meets = executer.attributeManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            requirementsNotMet.Add(executer.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                            remainingValueToCoverRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                        meets = target.attributeManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            requirementsNotMet.Add(target.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                            remainingValueToCoverRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //if (!meets)
                //{
                //    //remainingValueToCoverRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                //    //requirementsNotMet.Add(requirement);
                //    //if (attributeThatMostCloselyMeetsTheRequirement == null)
                //    //    Debug.LogWarning("Adding a attributeThatMostCloselyMeetsTheRequirement that is null"); // Only ok if there is no 
                //}

                //if (requirementsNotMet.Count != remainingValueToCoverRequirementsNotMet.Count)
                //{
                //    Debug.LogWarning("The two lists (requirementsNotMet and remainingValueToCoverInRequirementsNotMet) should be kept syncronized and they have different sizes.");
                //    requirementsNotMet.DebugLogWarning();
                //    remainingValueToCoverRequirementsNotMet.DebugLogWarning();
                //}
                
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