using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public static class TerrainRepository
{
    public static Dictionary<Vector2, TerrainChunk> TerrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private static float _meshSize = -1;
    private static float _meshScale;
    private static readonly float DistanceBetweenOrigins = 720;

    public static void AddChunk(TerrainChunk terrainChunk)
    {
        Debug.Log("Adding Terrain chunk with x: " + terrainChunk.Coord.x + " and y: " + terrainChunk.Coord.y);
        TerrainChunkDictionary.Add(new Vector2(terrainChunk.Coord.x * DistanceBetweenOrigins, terrainChunk.Coord.y * DistanceBetweenOrigins), terrainChunk);
    }

    public static NeighboringChunks GetChunksWithinDistance(Vector2 origin)
    {
        var normalizedX = origin.x;
        var normalizedY = origin.y;
        var nearbyChunks = new TerrainChunk[4];
        TerrainChunk left;
        if (TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX - DistanceBetweenOrigins, normalizedY), out left))
        {
            Debug.Log("Found left");
            nearbyChunks[0] = left;
        }


        TerrainChunk right;
        if(TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX + DistanceBetweenOrigins, normalizedY), out right))
        {
            nearbyChunks[1] = right;
        }

        TerrainChunk above;
        if(TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX, normalizedY + DistanceBetweenOrigins), out above))
        {
            nearbyChunks[2] = above;
        }

        TerrainChunk below;
        if (TerrainChunkDictionary.TryGetValue(new Vector2(normalizedX, normalizedY - DistanceBetweenOrigins), out below))
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