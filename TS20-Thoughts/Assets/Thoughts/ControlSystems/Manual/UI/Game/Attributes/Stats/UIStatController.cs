using Thoughts;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using TMPro;
using UnityEngine;

public class UIStatController : MonoBehaviour
{
    [SerializeField] private new TextMeshProUGUI title;
    
    public void Initialize(Stat stat)
    {
        title.text = stat.name;
    }
}
