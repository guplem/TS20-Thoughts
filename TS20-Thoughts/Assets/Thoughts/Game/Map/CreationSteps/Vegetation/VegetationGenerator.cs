﻿using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    public class VegetationGenerator : CreationStepGenerator
    {
        /// <summary>
        /// Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.
        /// </summary>
        [Tooltip("Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.")]
        [SerializeField] private MapGenerator mapGenerator;

        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int vegetationSeed => _randomNumberToAlterMainSeed + mapGenerator.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 5151335; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
        
        
        private void GenerateVegetation(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteVegetation();

            for (int v = 0; v < mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn.Length; v++)
            {
                mapGenerator.SpawnMapElementsWithPerlinNoiseDistribution(
                    mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].mapElementPrefab, 
                    vegetationSeed, 
                    mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].spawningHeightRange, 
                    mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].probability, 
                    mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].density, 
                    this.transform,
                    mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].noiseSettings,
                    false
                );
            }
            
            InvokeOnFinishStepGeneration();
        }
        public void DeleteVegetation()
        {
            if (Application.isPlaying)
                this.transform.DestroyAllChildren(); 
            else
                this.transform.DestroyImmediateAllChildren();
            
            
            Debug.LogWarning("NotImplementedException();"); // And remove them from the MapElement's list in the mapManager
        }
        
        protected override void GenerateStep(bool clearPrevious, bool generateNextStepOnFinish)
        {
            Debug.Log($"Generating in {this.name}.generateNextStepOnFinish = {generateNextStepOnFinish}", this);
            //base.GenerateStep(clearPrevious, generateNextStepOnFinish);
            GenerateVegetation(clearPrevious);
        }
    }
}
