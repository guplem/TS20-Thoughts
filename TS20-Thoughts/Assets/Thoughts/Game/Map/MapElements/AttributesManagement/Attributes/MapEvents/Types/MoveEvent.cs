using System.Collections;
using Thoughts;
using Thoughts.Game.GameMap;
using UnityEngine;

public class MoveEvent : MapEvent
{
    public override void Execute(MapElement executer, MapElement owner, Attribute attributeOwnerOfEvent, MapEventInAttributeAtMapElement nextEnqueuedEventInExecuter)
    {
        if (nextEnqueuedEventInExecuter == null)
        {
            Debug.LogWarning($"Trying to move {executer} to the location of the next action (which does not exist).");
            return;
        }
        
        Vector3 movePosition = nextEnqueuedEventInExecuter.mapElement.transform.position;
        //Debug.Log($"Executing MoveAction at {executer} to go to {movePosition}");
        executer.navMeshAgent.SetDestination(movePosition);
        executer.navMeshAgent.isStopped = false;
    }
}
