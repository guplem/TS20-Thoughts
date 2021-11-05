using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    public class VegetationGenerator : CreationStepGenerator
    {

        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int vegetationSeed => _randomNumberToAlterMainSeed + mapManager.mapGenerator.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 5151335; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
        
        private void GenerateVegetation(bool clearPrevious)
        {
            if (clearPrevious)
                Delete();

            for (int v = 0; v < mapManager.mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn.Length; v++)
            {
                mapManager.mapGenerator.SpawnMapElementsWithPerlinNoiseDistribution(
                    mapManager.mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].mapElementPrefab, 
                    vegetationSeed, 
                    mapManager.mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].spawningHeightRange, 
                    mapManager.mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].probability, 
                    mapManager.mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].density, 
                    this.transform,
                    mapManager.mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].noiseSettings,
                    false
                );
            }
            
            InvokeOnFinishStepGeneration();
        }
        
        
        protected override void _DeleteStep()
        {
            mapManager.mapGenerator.DestroyAllMapElementsChildOf(this.transform);
        }
        
        protected override void _GenerateStep(bool clearPrevious)
        {
            GenerateVegetation(clearPrevious);
        }
    }
}
