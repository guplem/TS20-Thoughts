using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(NeedSatisfaction))]
public class NeedSatisfactionInspector : Editor
{
    //private Type[] implementations;
    //private int selectedImplementationIndex;
    private NeedSatisfaction needSatisfaction;
    
    public override void OnInspectorGUI()
    {

        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        //specify type
        needSatisfaction = (NeedSatisfaction) target;
        if (needSatisfaction == null) { return; }

        //find all implementations of ISimpleAnimation using System.Reflection.Module
        //if (implementations == null)
        //    implementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<ISimpleAnimation>();

        GUILayout.Button("Dummy");
    }
}
