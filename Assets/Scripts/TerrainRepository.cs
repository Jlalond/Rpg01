using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TerrainRepository
{
    public static Dictionary<Vector2, TerrainChunk> TerrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private static float _meshSize = -1;

    public static void SetMeshSize(float num)
    {
        if (_meshSize == -1)
        {
            _meshSize = num;
        }
    }

    public static void AddChunk(TerrainChunk terrainChunk)
    {
        Debug.Log("Adding Terrain chunk with x: " + terrainChunk.Coord.x + " and y: " + terrainChunk.Coord.y);
        TerrainChunkDictionary.Add(terrainChunk.Coord, terrainChunk);
    }

    public static NeighboringChunks GetChunksWithinDistance(Vector2 origin)
    {
        var normalizedX = Mathf.RoundToInt(origin.x / _meshSize);
        var normalizedY = Mathf.RoundToInt(origin.y / _meshSize);
        var nearbyChunks = new TerrainChunk[4];
        TerrainChunk left;
        if (TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX, normalizedY - 1), out left))
        {
            Debug.Log("Found left");
            nearbyChunks[0] = left;
        }

        TerrainChunk right;
        if(TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX, normalizedY + 1), out right))
        {
            nearbyChunks[1] = right;
        }

        TerrainChunk above;
        if(TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX + 1, normalizedY), out above))
        {
            nearbyChunks[2] = above;
        }

        TerrainChunk below;
        if (TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX - 1, normalizedY), out below))
        {
            nearbyChunks[3] = below;
        }

        return new NeighboringChunks(nearbyChunks[0], nearbyChunks[1], nearbyChunks[2], nearbyChunks[3]);
    }

    public struct NeighboringChunks
    {
        public readonly TerrainChunk Left;
        public readonly TerrainChunk Right;
        public readonly TerrainChunk Above;
        public readonly TerrainChunk Below;

        public NeighboringChunks(TerrainChunk left, TerrainChunk right, TerrainChunk above, TerrainChunk below)
        {
            Left = left;
            Right = right;
            Above = above;
            Below = below;
        }

        public IEnumerable<TerrainChunk> Flatten()
        {
            var list = new List<TerrainChunk>();
            if (Left != null)
            {
                list.Add(Left);
            }
            if (Right != null)
            {
                list.Add(Right);
            }
            if (Above != null)
            {
                list.Add(Above);
            }
            if (Below != null)
            {
                list.Add(Below);
            }
            return list;
        }
    }
}