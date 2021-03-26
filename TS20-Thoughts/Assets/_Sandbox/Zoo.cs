using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEngine;

public class Zoo : MonoBehaviour
{
    [SerializeField] private Animal favAnimal;
    
    [SerializeField] public List<ConsequenceNeed> consequenceNeeds = new List<ConsequenceNeed>();
    [SerializeField] public List<RequiredNeed> requiredNeeds = new List<RequiredNeed>();
    
}
