using System.Collections;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using TMPro;
using UnityEngine;

public class UIRelatedStat : UIPovRowElement
{
    public RelatedStat relatedStat { get; private set; }
    [SerializeField] private new TextMeshPro name;
    
    public void Initialize(RelatedStat relatedStat, Transform visualizer)
    {
        Initialize(visualizer);
        this.relatedStat = relatedStat;
        name.text = GetText();
    }
    
    private string GetText()
    {
        string ret = relatedStat.stat.name;
        ret += '\n';
        ret += '\n';
        ret += relatedStat.satisfactionAmount;
        ret += '\n';
        ret += '\n';
        ret += $"Min: {relatedStat.minimumDemandedSatisfactionAmount}\nNeeds Care: {relatedStat.needsCare}";
        return ret;
    }

    protected override void UpdateVisuals()
    {
        base.UpdateVisuals();
        name.text = GetText();
    }
}
