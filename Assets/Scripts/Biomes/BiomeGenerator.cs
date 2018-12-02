using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Biomes
{
    public static class BiomeGenerator
    {
        public static Biome GenerateRelativeBiome(TerrainChunk origin)
        {
            var nearbyChunks = TerrainRepository.GetChunksWithinDistance(origin.Coord).Flatten().ToList();
            if(!nearbyChunks.Any())
            {
                return new Biome();
            }

            var biomes = nearbyChunks.Select(c => c.Biome);
            var biomeDictionary = new Dictionary<Biome, int>();
            foreach(var biome in biomes)
            {
                if (!biomeDictionary.ContainsKey(biome))
                {
                    biomeDictionary.Add(biome, 0);
                }
                biomeDictionary[biome]++;
            }

            var numOfBiomes = biomeDictionary.Keys.Count;
            var weightedBiomes = new List<KeyValuePair<float, Biome>>();
            foreach(var pair in biomeDictionary)
            {
                weightedBiomes.Add(new KeyValuePair<float, Biome>((float)pair.Value / numOfBiomes, pair.Key));
            }

            var max = weightedBiomes.Max(c => c.Key);
            var randomNumberBetweenZeroAndMax = Random.Range(0.0f, max);


            return weightedBiomes.First(c => c.Key >= randomNumberBetweenZeroAndMax).Value;
        }
    }
}
