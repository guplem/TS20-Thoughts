using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using UnityEngine;

public class WaterSourcesGenerator : CreationStepGenerator
{
    protected override void _DeleteStep()
    {
        
    }
    protected override void _GenerateStep(bool clearPrevious)
    {
        TerrainType[,] terrainTypes = new TerrainType[mapGenerator.mapConfiguration.mapRadius * 2 +1, mapGenerator.mapConfiguration.mapRadius * 2 +1];

            for (int x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);
                    if (terrainTypes[arrayCoords.x,arrayCoords.y] != TerrainType.none)
                        continue;
                    if (!IsLocationUnderWater(new Vector2(x, y)))
                    {
                        terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.land;
                    }
                    else
                    {
                        if (arrayCoords.x == 0 || arrayCoords.y == 0 || arrayCoords.x == terrainTypes.GetLength(0) - 1 || arrayCoords.y == terrainTypes.GetLength(1) - 1)
                        {
                            terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.sea;
                        }
                    }
                }                
            }

            terrainTypes = PropagateSea(terrainTypes);
            
            // Only interior water sopts should be left
            for (int x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);
                    if (terrainTypes[arrayCoords.x,arrayCoords.y] != TerrainType.none)
                        continue;
                    terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.interior;
                }                
            }
            
            //Replace interior with shorelineInterior
            int avoidedEdge = 1;
            for (int x = -mapGenerator.mapConfiguration.mapRadius+avoidedEdge; x <= mapGenerator.mapConfiguration.mapRadius-avoidedEdge; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius+avoidedEdge; y <= mapGenerator.mapConfiguration.mapRadius-avoidedEdge; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);
                    if (terrainTypes[arrayCoords.x,arrayCoords.y] != TerrainType.interior)
                        continue;
                    
                    try
                    {
                        TerrainType bottom = terrainTypes[arrayCoords.x - 1, arrayCoords.y];
                        TerrainType top = terrainTypes[arrayCoords.x + 1, arrayCoords.y ];
                        TerrainType left = terrainTypes[arrayCoords.x , arrayCoords.y - 1];
                        TerrainType right = terrainTypes[arrayCoords.x , arrayCoords.y + 1];

                        bool isAnyNeighbourInterior = bottom == TerrainType.interior || top == TerrainType.interior || left == TerrainType.interior || right == TerrainType.interior ;
                        bool isAnyNeighbourLand     = bottom == TerrainType.land     || top == TerrainType.land     || left == TerrainType.land     || right == TerrainType.land     ;
                        //bool isOnlyOneNeighbourLand = ((topLeftTerrainType == TerrainType.land ^ topRightTerrainType == TerrainType.land) ^ (bottomLeftTerrainType == TerrainType.land ^ bottomRightTerrainType == TerrainType.land));
                        
                        if (isAnyNeighbourLand && isAnyNeighbourInterior)
                            terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.interiorShoreline;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Array coords: {arrayCoords}. Array Length: [{terrainTypes.GetLength(0)},{terrainTypes.GetLength(1)}]. World coords: ({x},{y}).\nERROR: {e.Message}");
                    }

                }                
            }

            // Do whatever with each type (spawn water sources, ...)
            for (int x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);

                    float rayDuration = 1f;
                    float rayLength = 0.3f;
                    switch (terrainTypes[arrayCoords.x,arrayCoords.y])
                    {

                        case TerrainType.none:
                            Debug.DrawRay(new Vector3(x,mapGenerator.mapConfiguration.seaHeightAbsolute,y),Vector3.up*rayLength, Color.black, rayDuration );
                            break;
                        case TerrainType.sea:
                            Debug.DrawRay(new Vector3(x,mapGenerator.mapConfiguration.seaHeightAbsolute,y),Vector3.up*rayLength, Color.white, rayDuration );
                            break;
                        case TerrainType.interior:
                            Debug.DrawRay(new Vector3(x,mapGenerator.mapConfiguration.seaHeightAbsolute,y),Vector3.up*rayLength, Color.yellow, rayDuration );
                            break;
                        case TerrainType.interiorShoreline:
                            Debug.DrawRay(new Vector3(x,mapGenerator.mapConfiguration.seaHeightAbsolute,y),Vector3.up*rayLength, Color.magenta, rayDuration );
                            MapElement spawned = mapGenerator.SpawnMapElement(waterSourcePrefab, new Vector3(x, mapGenerator.mapConfiguration.seaHeightAbsolute, y), Quaternion.identity, waterSourceParent);
                            //Debug.Log("SPAWN", spawned.gameObject);
                            break;
                        case TerrainType.land:
                            Debug.DrawRay(new Vector3(x,GetHeightAt(new Vector2(x,y)),y),Vector3.up*rayLength, Color.green, rayDuration );
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
    }
}
