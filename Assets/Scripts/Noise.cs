using UnityEngine;
using UnityEngine.Serialization;

public static class Noise {

    public enum NormalizeMode {Local, Global};

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre) {
        var noiseMap = new float[mapWidth,mapHeight];

        var prng = new System.Random (settings.Seed);
        var octaveOffsets = new Vector2[settings.Octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (var i = 0; i < settings.Octaves; i++) {
            var offsetX = prng.Next (-100000, 100000) + settings.Offset.x + sampleCentre.x;
            var offsetY = prng.Next (-100000, 100000) - settings.Offset.y - sampleCentre.y;
            octaveOffsets [i] = new Vector2 (offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.Persistance;
        }

        var maxLocalNoiseHeight = float.MinValue;
        var minLocalNoiseHeight = float.MaxValue;

        var halfWidth = mapWidth / 2f;
        var halfHeight = mapHeight / 2f;


        for (var y = 0; y < mapHeight; y++) {
            for (var x = 0; x < mapWidth; x++) {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < settings.Octaves; i++) {
                    var sampleX = (x-halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
                    var sampleY = (y-halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;

                    var perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.Persistance;
                    frequency *= settings.Lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                } 
                if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap [x, y] = noiseHeight;

                if (settings.NormalizeMode == NormalizeMode.Global) {
                    var normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return NormalizeHeightmap(noiseMap);
    }

    private static float[,] NormalizeHeightmap(float[,] heightMaps)
    {
        var sum = 0f;
        for (var x = 0; x < heightMaps.GetLength(0); x++)
        {
            for (var y = 0; y < heightMaps.GetLength(1); y++)
            {
                sum += heightMaps[x, y];
            }
        }

        sum = sum / (heightMaps.GetLength(0) * heightMaps.GetLength(1));

        for (var x = 0; x < heightMaps.GetLength(0); x++)
        {
            for (var y = 0; y < heightMaps.GetLength(1); y++)
            {
                var heightAtCoords = heightMaps[x, y];
                if (heightAtCoords > sum)
                {
                    var diff = heightAtCoords - sum;
                    heightMaps[x, y] = heightAtCoords - (diff / 2);
                }
                else
                {
                    var diff = sum - heightAtCoords;
                    heightMaps[x, y] = heightAtCoords + (diff / 2);
                }
            }
        }

        return heightMaps;
    }

}



[System.Serializable]
public class NoiseSettings {
    [FormerlySerializedAs("normalizeMode")] public Noise.NormalizeMode NormalizeMode;

    [FormerlySerializedAs("scale")] public float Scale = 50;

    [FormerlySerializedAs("octaves")] public int Octaves = 6;
    [FormerlySerializedAs("persistance")] [Range(0,1)]
    public float Persistance =.6f;
    [FormerlySerializedAs("lacunarity")] public float Lacunarity = 2;

    [FormerlySerializedAs("seed")] public int Seed;
    [FormerlySerializedAs("offset")] public Vector2 Offset;

    public void ValidateValues() {
        Scale = Mathf.Max (Scale, 0.01f);
        Octaves = Mathf.Max (Octaves, 1);
        Lacunarity = Mathf.Max (Lacunarity, 1);
        Persistance = Mathf.Clamp01 (Persistance);
    }
}