using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TerrainRepository
    {
        public static Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

        public static IEnumerable<TerrainChunk> GetChunksWithinDistance(Vector2 origin)
        {
            var nearbyChunks = new List<TerrainChunk>();
            TerrainChunk left;
            if (terrainChunkDictionary.TryGetValue(new Vector2(origin.x + 96, origin.y), out left))
            {
                nearbyChunks.Add(left);
            }

            TerrainChunk right;
            if(terrainChunkDictionary.TryGetValue(new Vector2(origin.x - 96, origin.y), out right))
            {
                nearbyChunks.Add(right);
            }

            TerrainChunk above;
            if(terrainChunkDictionary.TryGetValue(new Vector2(origin.x, origin.y + 96), out above))
            {
                nearbyChunks.Add(above);
            }

            TerrainChunk below;
            if (terrainChunkDictionary.TryGetValue(new Vector2(origin.x, origin.y - 96), out below))
            {
                nearbyChunks.Add(below);
            }

            return nearbyChunks;
        }
    }
}
