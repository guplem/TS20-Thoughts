using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Thoughts.MapElements;
using Thoughts.Needs;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class Mob : MapElement
    {
        
        private Need currentObjectiveNeed
        {
            get => _currentObjectiveNeed;
            set
            {
                _currentObjectiveNeed = value;
                //Debug.Log($"Current working need is '{currentWorkingNeed}'");
                currentActionPath = currentObjectiveNeed.GetActionsSatisfy(this);
                if (currentActionPath == null)
                    Debug.LogWarning($"An action path to take care of the need '{currentObjectiveNeed}' was not found.");
                else
                {
                    currentActionPath.DebugLog(", ", $"Found action path to cover need '{currentObjectiveNeed}': ", gameObject);
                    DoNextAction();    
                }
            }
        }
        [CanBeNull]
        private Need _currentObjectiveNeed;
        private List<MobAction> currentActionPath = new List<MobAction>();
        public NavMeshAgent navMeshAgent { get; private set; }

        private void Awake()
        {

            navMeshAgent = GetComponent<NavMeshAgent>();
            
            //StartNeedsSatisfactionLoss();
        }
        /*private void StartNeedsSatisfactionLoss()
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
        */
        
        private bool DoNextAction() // false if distance is too big
        {
            //Todo: check if is in range to do the action. if not, get closer
            // new MoveAction(elementToCoverNeed.gameObject.transform.position)
            Debug.LogWarning("Possible check needed for the range to be able to do the action. if not, get closer");

            MobAction action = currentActionPath.ElementAt(0);

            Debug.Log($"Executing action {action}");
            action.Execute(this);
            
            currentActionPath.RemoveAt(0);
            return true;
        }
        private double DistanceToNextAction()
        {
            throw new System.NotImplementedException();
        }


    }
}
