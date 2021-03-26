using System.Collections;
using Thoughts.Game.GameMap;
using UnityEngine;

public class MoveAction : MapAction
{


    public override void Execute(MapElement executer, MapActionFromMapElement nextActionFromMapElement)
    {
        if (nextActionFromMapElement == null)
        {
            Debug.LogWarning($"Trying to move {executer} to the location of the next action (which does not exist).");
            return;
        }
        
        Vector3 movePosition = nextActionFromMapElement.mapElement.transform.position;
        Debug.Log($"Executing MoveAction at {executer} to go to {movePosition}");
        executer.navMeshAgent.SetDestination(movePosition);
        executer.navMeshAgent.isStopped = false;
    }
}
