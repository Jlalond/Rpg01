using System;
using UnityEngine;

namespace TerrainChunkObjects
{
    [Serializable]
    public class TerrainChunkGameObject : MonoBehaviour
    {
        public TerrainChunkGameObject Above;
        public TerrainChunkGameObject Below;
        public TerrainChunkGameObject Left;
        public TerrainChunkGameObject Right;
        public float[,] HeightMap;
    }
}
