using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ConsumeAction : MobAction
{
    public override string GetActionName() { return actionName; }
    [SerializeField] public string actionName = "Consume"; // To make it visible and editable from the inspector;
    
    public ConsumeAction() { ;}

    public override void Execute(Mob mob)
    {
        throw new System.NotImplementedException();
    }
    
}
