using System.Collections;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
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
        
        if (mapEvent.requirements.Count > 0)
            ret += "Requirements:\n";
        foreach (AttributeUpdate requirement in mapEvent.requirements)
            ret += $" - {requirement.attribute}: {requirement.value}\n";
        
        if (mapEvent.consequences.Count > 0)
            ret += "Consequences:\n";
        foreach (AttributeUpdate consequence in mapEvent.consequences)
            ret += $" - {consequence.attribute} ({consequence.value})\n";
        
        return ret;
    }
}
