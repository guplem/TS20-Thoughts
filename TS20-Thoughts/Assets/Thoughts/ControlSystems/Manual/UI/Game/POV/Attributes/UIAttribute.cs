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
        UpdateVisuals();
    }
    
    
    private string GetText()
    {
        string ret =attribute.name;
        ret += '\n';

        ret += $"{attribute.value}/{attribute.minValue}";
        
        return ret;
    }
    protected override void UpdateVisuals()
    {
        name.text = GetText();
    }
}
