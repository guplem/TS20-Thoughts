using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ConsumeAction : MapAction
{
    public override string GetActionName() { return actionName; }
    [SerializeField] public string actionName = "Consume"; // To make it visible and editable from the inspector;
    
    public ConsumeAction() { ;}

    public override void Execute(MapElement executer)
    {
        throw new System.NotImplementedException();
    }
    
}
