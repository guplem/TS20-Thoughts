using System.Collections;
using System.Collections.Generic;
using Thoughts;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public class TerrainUI : CreationStepUI // Like UserNameUI
{
    public void GenerateTerrain()
    {
        AppManager.gameManager.map.GenerateNew(); //Todo: Generate terrain, not all map
    }
}
