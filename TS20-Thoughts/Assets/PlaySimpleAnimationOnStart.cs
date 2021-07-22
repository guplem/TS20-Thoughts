using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleAnimationsManager))]
public class PlaySimpleAnimationOnStart : MonoBehaviour
{
    private SimpleAnimationsManager simpleAnimationsManager;

    private void Awake()
    {
        simpleAnimationsManager = gameObject.GetComponentRequired<SimpleAnimationsManager>();
    }

    void Start()
    {
        simpleAnimationsManager.Play(0);
        Debug.LogWarning("Playing");
    }

}
