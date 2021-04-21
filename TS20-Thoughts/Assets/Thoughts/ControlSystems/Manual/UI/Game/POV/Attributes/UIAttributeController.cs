using System;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using TMPro;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class UIAttributeController : MonoBehaviour
{

    [SerializeField] private new TextMeshPro name;
    private Transform lookAtTarget;
    //[SerializeField] private Transform statsPanel;
    //[SerializeField] private GameObject UIStatPrefab;
    
    public void Initialize(Attribute attribute, Transform visualizer)
    {
        name.text = attribute.name;
        this.lookAtTarget = visualizer;

        //foreach (RelatedStat attributeRelatedStat in attribute.relatedStats)
        //{
        //    UIStatController statController = Instantiate(UIStatPrefab, statsPanel).GetComponent<UIStatController>();
        //    statController.Initialize(attributeRelatedStat.stat);
        //}
    }

    private void Update()
    {
        transform.LookAt(lookAtTarget);
    }
}
