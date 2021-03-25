using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
        private List<MapAction> currentActionPath = new List<MapAction>();
        
        private bool DoNextAction() // false if distance is too big
        {
            //Todo: check if is in range to do the action. if not, get closer
            // new MoveAction(elementToCoverNeed.gameObject.transform.position)
            Debug.LogWarning("Possible check needed for the range to be able to do the action. if not, get closer");

            MapAction action = currentActionPath.ElementAt(0);

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
