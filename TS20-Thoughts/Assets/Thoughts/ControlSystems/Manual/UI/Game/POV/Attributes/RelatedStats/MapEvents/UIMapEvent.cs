using System.Collections;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using TMPro;
using UnityEngine;

public class UIMapEvent : UIPovRowElement
{
    public MapEvent mapEvent { get; private set; }
    [SerializeField] private new TextMeshPro name;
    
    public void Initialize(MapEvent mapEvent, Transform visualizer)
    {
        Initialize(visualizer);
        this.mapEvent = mapEvent;
        name.text = GetText();
    }
    
    private string GetText()
    {
        string ret = mapEvent.name;
        ret += '\n';
        
        if (mapEvent.requiredStats.Count > 0)
            ret += "Requirements:\n";
        foreach (RequiredStat requirement in mapEvent.requiredStats)
            ret += $" - {requirement.stat}: {requirement.requiredAmount}\n";
        
        if (mapEvent.consequenceStats.Count > 0)
            ret += "Consequences:\n";
        foreach (ConsequenceStat consequence in mapEvent.consequenceStats)
            ret += $" - {consequence.stat} ({consequence.deltaSatisfactionAmount})\n";
        
        return ret;
    }
}
