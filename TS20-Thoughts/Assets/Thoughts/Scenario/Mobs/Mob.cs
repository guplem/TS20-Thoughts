using System.Collections;
using System.Collections.Generic;
using Thoughts.Needs;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Mobs
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class Mob : MonoBehaviour
    {
        
        [SerializeField] protected NeedsHierarchy needsHierarchy;
        private IEnumerator coroutineHolder; // Keeps track of the coroutine
        private Need currentWorkingNeed
        {
            get => _currentWorkingNeed;
            set
            {
                _currentWorkingNeed = value;
                //Debug.Log($"Current working need is '{currentWorkingNeed}'");
                queuedActions = currentWorkingNeed.GetActionsToTakeCare();
                queuedActions.DebugLog(", ", $"Actions to cover '{currentWorkingNeed}' need: ", gameObject);
                DoNextAction();
            }
        }

        private Need _currentWorkingNeed;
        private Queue<MobAction> queuedActions = new Queue<MobAction>();
        public NavMeshAgent navMeshAgent { get; private set; }

        private void Awake()
        {
            // Clone the NeedsHierarchy so each mob has a different one instead of all sharing the same  
            if (needsHierarchy != null)
                needsHierarchy = Instantiate(needsHierarchy);
            else
                Debug.LogError($"Mov {gameObject.name} does not have a NeedsHierarchy set.");
            navMeshAgent = GetComponent<NavMeshAgent>();
            
            StartNeedsConsumption();
        }
        private void StartNeedsConsumption()
        {
            // Ensure that we are not going to lose the track of a previous coroutine 
            // if we lose it, we'll not be able to stop it.
            if (coroutineHolder != null)
                StopCoroutine(coroutineHolder);

            //Assign the coroutine to the holder
            coroutineHolder = ConsumeNeeds();
            //Run the coroutine
            StartCoroutine(coroutineHolder);
        }

        private void StopNeedsConsumption()
        {
            if (coroutineHolder != null)
                StopCoroutine(coroutineHolder);
        }

        private IEnumerator ConsumeNeeds()
        {
            while (true)
            {
                yield return new WaitForSeconds(Need.timeBetweenLoss);
                foreach (Need need in needsHierarchy.needs)
                {
                    need.Consume();
                    if (!need.needsCare)
                        continue;
                    //If the new one has more priority than the current one
                    if (currentWorkingNeed == null || currentWorkingNeed.priority < need.priority || !currentWorkingNeed.needsCare)
                        currentWorkingNeed = need;
                }
            }
        }
        
        
        [ContextMenu("Debug needs hierarchy")]
        public void DebugHierarchyNeeds()
        {
            DebugEssentials.LogEnumerable(needsHierarchy.needs, ", ", $"{this.GetType().Name} Needs Hierarchy:  ", this.gameObject);    
        } 
        
        private void DoNextAction()
        {
            MobAction action = queuedActions.Dequeue();
            Debug.Log($"Executing action {action}");
            action.Execute(this);
        }
            
            
    }
}
