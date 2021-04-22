using TMPro;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class UIAttribute : UIPovRowElement
{
    public Attribute attribute { get; private set; }
    [SerializeField] private new TextMeshPro name;
    
    public void Initialize(Attribute attribute, Transform visualizer)
    {
        Initialize(visualizer);
        this.attribute = attribute;
        name.text = attribute.name;
    }
}
