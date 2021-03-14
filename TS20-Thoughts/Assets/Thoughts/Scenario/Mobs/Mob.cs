using System;
using Thoughts.Needs;
using UnityEngine;

namespace Thoughts.Mobs
{
    public abstract class Mob : MonoBehaviour
    {
        [SerializeField] protected NeedsHierarchy needsHierarchy;

        private void Awake()
        {
            // Clone the NeedsHierarchy so each mob has a different one instead of all sharing the same  
            needsHierarchy = Instantiate(needsHierarchy);
        }

        private void Update()
        {
            foreach (Need need in needsHierarchy.needs)
            {
                
            }
        }
        
        
        
        [ContextMenu("Decrease value first need")]
        public void DecreaseValueFirstNeed()
        {
            needsHierarchy.needs[0].value = 50;    
        }
        
        [ContextMenu("Debug needs hierarchy")]
        public void DebugHierarchyNeeds()
        {
            DebugEssentials.LogEnumerable(needsHierarchy.needs, ", ", $"{this.GetType().Name} Needs Hierarchy:  ", this.gameObject);    
        } 
            
            
    }
}
