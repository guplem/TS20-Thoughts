using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class UIPovManager : MonoBehaviour
{
    [SerializeField]
    public UIAttributesManager uiAttributesManager;

    public void ShowUIFor(Mob mob)
    {
        this.transform.parent = mob.povCameraPrentTransform;
        this.transform.localPosition = Vector3.zero;
        uiAttributesManager.ShowUIFor(mob);
    }
}
