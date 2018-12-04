using Extensions;
using UnityEngine;

public static class HeightMapGenerator {

    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
        var values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre, settings.heightMultiplier);


        var minValue = float.MaxValue;
        var maxValue = float.MinValue;

        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (values [i, j] > maxValue) {
                    maxValue = values [i, j];
                }
                if (values [i, j] < minValue) {
                    minValue = values [i, j];
                }
            }
        }

        return new HeightMap (values, minValue, maxValue);
    }
}

public struct HeightMap {
    public readonly float[,] Values;
    public readonly float MinValue;
    public readonly float MaxValue;
    public readonly float[] TopEdge;
    public readonly float[] BottomEdge;
    public readonly float[] LeftEdge;
    public readonly float[] RightEdge;

    public HeightMap (float[,] values, float minValue, float maxValue)
    {
        Values = values;
        MinValue = minValue;
        MaxValue = maxValue;
        TopEdge = values.GetTopEdge();
        BottomEdge = values.GetBottomEdge();
        LeftEdge = values.GetLeftEdge();
        RightEdge = values.GetRightEdge();
    }
}