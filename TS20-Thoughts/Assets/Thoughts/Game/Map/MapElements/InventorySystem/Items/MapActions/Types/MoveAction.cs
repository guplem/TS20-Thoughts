using System.Collections;
using Thoughts.Game.GameMap;
using UnityEngine;

public class MoveAction : MapAction
{
    public override string GetActionName() { return actionName; }
    [SerializeField] public string actionName = "Move"; // To make it visible and editable from the inspector;

    public MoveAction() : this(Vector3.zero) { } 

    [SerializeField] private Vector3 destination;
    
    public MoveAction(Vector3 destination)
    {
        this.destination = destination;
    }

    public override void Execute(MapElement executer)
    {
        executer.navMeshAgent.SetDestination(destination);
        executer.navMeshAgent.isStopped = false;
    }

}
