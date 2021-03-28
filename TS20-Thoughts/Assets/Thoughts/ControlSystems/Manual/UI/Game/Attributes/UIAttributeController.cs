using Thoughts;
using TMPro;
using UnityEngine;

public class UIAttributeController : MonoBehaviour
{

    [SerializeField] private new TextMeshProUGUI name;
    
    public void Initialize(Attribute attribute)
    {
        name.text = attribute.name;
    }
}
