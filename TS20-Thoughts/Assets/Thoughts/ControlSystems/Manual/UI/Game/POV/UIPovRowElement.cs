using TMPro;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public abstract class UIPovRowElement : MonoBehaviour
{
    private Transform lookAtTarget;

    public virtual void Initialize(Transform visualizer)
    {
        this.lookAtTarget = visualizer;
    }

    private void Update()
    {
        transform.LookAt(lookAtTarget);
        UpdateVisuals();
    }

    protected abstract void UpdateVisuals();
}
