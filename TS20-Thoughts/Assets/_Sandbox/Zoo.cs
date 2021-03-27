using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

public class Zoo : MonoBehaviour
{
    [SerializeField] private Animal favAnimal;
    
    [SerializeField] public List<ConsequenceStat> consequenceNeeds = new List<ConsequenceStat>();
    [SerializeField] public List<RequiredStat> requiredNeeds = new List<RequiredStat>();
    
}
