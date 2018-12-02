using Data;
using UnityEngine;

public static class HeightMapGenerator {

    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
        var values = Noise.GenerateNoiseMap (width, height, settings.NoiseSettings, sampleCentre);

        var heightCurveThreadsafe = new AnimationCurve (settings.HeightCurve.keys);

        var minValue = float.MaxValue;
        var maxValue = float.MinValue;

        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                values [i, j] *= heightCurveThreadsafe.Evaluate (values [i, j]) * settings.HeightMultiplier;

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
        TopEdge = GetTopEdge(values);
        BottomEdge = GetBottomEdge(values);
        LeftEdge = GetLeftEdge(values);
        RightEdge = GetRightEdge(values);
    }

    private static float[] GetBottomEdge(float[,] values)
    {
        var bottomEdge = new float[values.GetLength(0)];
        for (var i = 0; i < bottomEdge.Length; i++)
        {
            bottomEdge[i] = values[i, values.GetLength(1) - 1];
        }

        return bottomEdge;
    }

    private static float[] GetLeftEdge(float[,] values)
    {
        var leftEdge = new float[values.GetLength(1)];
        for (var i = 0; i < leftEdge.Length; i++)
        {
            leftEdge[i] = values[0, i];
        }

        return leftEdge;
    }

    private static float[] GetRightEdge(float[,] values)
    {
        var rightEdge = new float[values.GetLength(1)];
        for (var i = 0; i < rightEdge.Length; i++)
        {
            rightEdge[i] = values[values.GetLength(1) - 1, i];
        }

        return rightEdge;
    }

    private static float[] GetTopEdge(float[,] values)
    {
        var topEdge = new float[values.GetLength(0)];
        for(var i = 0; i < topEdge.Length; i++)
        {
            topEdge[i] = values[i, 0];
        }

        return topEdge;
    }
}