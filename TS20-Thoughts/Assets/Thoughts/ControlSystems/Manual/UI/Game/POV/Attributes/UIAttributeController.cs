using Thoughts;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using TMPro;
using UnityEngine;

public class UIAttributeController : MonoBehaviour
{

    [SerializeField] private new TextMeshPro name;
    //[SerializeField] private Transform statsPanel;
    //[SerializeField] private GameObject UIStatPrefab;
    
    public void Initialize(Attribute attribute)
    {
        name.text = attribute.name;

        //foreach (RelatedStat attributeRelatedStat in attribute.relatedStats)
        //{
        //    UIStatController statController = Instantiate(UIStatPrefab, statsPanel).GetComponent<UIStatController>();
        //    statController.Initialize(attributeRelatedStat.stat);
        //}
    }
}
