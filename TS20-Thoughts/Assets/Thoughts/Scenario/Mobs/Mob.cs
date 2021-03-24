using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Thoughts.Needs;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Mobs
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class Mob : MonoBehaviour
    {
        
        [SerializeField] protected NeedsHierarchy needsNeedsHierarchy;
        private IEnumerator needsSatisfationLossCoroutineHolder; // Keeps track of the coroutine
        private Need currentObjectiveNeed
        {
            get => _currentObjectiveNeed;
            set
            {
                _currentObjectiveNeed = value;
                //Debug.Log($"Current working need is '{currentWorkingNeed}'");
                currentActionPath = currentObjectiveNeed.GetActionsToTakeCare();
                if (currentActionPath == null)
                    Debug.LogWarning($"An action path to take care of the need '{currentObjectiveNeed}' was not found.");
                currentActionPath.DebugLog(", ", $"Found action path to cover '{currentObjectiveNeed}' need: ", gameObject);
                DoNextAction();
            }
        }
        [CanBeNull]
        private Need _currentObjectiveNeed;
        private List<MobAction> currentActionPath = new List<MobAction>();
        public NavMeshAgent navMeshAgent { get; private set; }

        private void Awake()
        {
            // Clone the NeedsHierarchy so each mob has a different one instead of all sharing the same  
            if (needsNeedsHierarchy != null)
                needsNeedsHierarchy = Instantiate(needsNeedsHierarchy);
            else
                Debug.LogError($"Mov {gameObject.name} does not have a NeedsHierarchy set.");
            
            navMeshAgent = GetComponent<NavMeshAgent>();
            
            StartNeedsSatisfactionLoss();
        }
        private void StartNeedsSatisfactionLoss()
        {
            // Ensure that we are not going to lose the track of a previous coroutine 
            // if we lose it, we'll not be able to stop it.
            if (needsSatisfationLossCoroutineHolder != null)
                StopNeedsSatisfactionLoss();

            //Assign the coroutine to the holder
            needsSatisfationLossCoroutineHolder = LossNeedsSatisfaction();
            
            //Run the coroutine
            StartCoroutine(needsSatisfationLossCoroutineHolder);
        }

        private void StopNeedsSatisfactionLoss()
        {
            if (needsSatisfationLossCoroutineHolder != null)
                StopCoroutine(needsSatisfationLossCoroutineHolder);
            needsSatisfationLossCoroutineHolder = null;
        }

        private IEnumerator LossNeedsSatisfaction()
        {
            while (true)
            {
                yield return new WaitForSeconds(Need.timeBetweenNeedSatisfactionLoss);
                foreach (Need need in needsNeedsHierarchy.needs)
                {
                    need.LossSatisfaction();
                    if (!need.needsCare)
                        continue;
                    //If the new one has more priority than the current one
                    if (currentObjectiveNeed == null || currentObjectiveNeed.level < need.level || !currentObjectiveNeed.needsCare)
                        currentObjectiveNeed = need;
                }
            }
        }
        
        
        [ContextMenu("Debug inherent needs hierarchy")]
        public void DebugInherentNeedsHierarchy()
        {
            DebugEssentials.LogEnumerable(needsNeedsHierarchy.needs, ", ", $"{this.GetType().Name} Needs Hierarchy:  ", this.gameObject);    
        } 
        
        private void DoNextAction()
        {
            //Todo: check if is in range to do the action. if not, get closer
            // new MoveAction(elementToCoverNeed.gameObject.transform.position)
            Debug.LogWarning("Todo: check if is in range to do the action. if not, get closer");
            
            MobAction action = currentActionPath.ElementAt(0);
            Debug.Log($"Executing action {action}");
            action.Execute(this);
            currentActionPath.RemoveAt(0);
        }
            
            
    }
}
