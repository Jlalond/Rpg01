using System;
using UnityEngine;

namespace Extensions
{
    public static class HeightMapExtensions
    {
        public static float[,] NormalizeLeftSide(this float[,] heightMap, float[] leftSide)
        {
            if (leftSide == null || leftSide.Length == 0)
            {
                return heightMap;
            }

            for (int i = 0; i < leftSide.Length; i++)
            {
                heightMap[i, 0] = leftSide[i];
            }

            var averageDiff = 0f;
            var count = 0;
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    count++;
                    averageDiff += leftSide[x] - heightMap[x, y];
                }
            }

            averageDiff = averageDiff / count;

            for (int x = 0; x < heightMap.GetLength(0) / 4; x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    var value = heightMap[x, y];
                    if (value > averageDiff)
                    {
                        heightMap[x, y] -= averageDiff / 1 + Math.Abs(1 + x); // divided by the offset from the left most edge, 1 being the left most edge 
                    }
                    else
                    {
                        heightMap[x, y] += averageDiff / 1 + Math.Abs(1 + x);
                    }
                }
            }

            return heightMap;
        }


        public static float[,] NormalizeRightSide(this float[,] heightMap, float[] rightSide)
        {
            if (rightSide == null || rightSide.Length == 0)
            {
                return heightMap;
            }
            for (int i = 0; i < rightSide.Length; i++)
            {
                heightMap[i, heightMap.GetLength(0) - 1] = rightSide[i];
            }

            var averageDiff = 0f;
            var count = 0;
            for (int x = heightMap.GetLength(0) - heightMap.GetLength(0) / 4; x < rightSide.Length; x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    count++;
                    averageDiff += rightSide[x] - heightMap[x, y];
                }
            }

            return heightMap;
        }

        public static float[,] NormalizeTopSide(this float[,] heightMap, float[] topSide)
        {
            if (topSide == null || topSide.Length == 0)
            {
                return heightMap;
            }
            for (int i = 0; i < topSide.Length; i++)
            {
                heightMap[0, i] = topSide[i];
            }

            var averageDiff = 0f;
            var count = 0;
            for (int x = 0; x < heightMap.GetLength(1) / 4; x++)
            {
                for (int y = 0; y < heightMap.GetLength(0); y++)
                {
                    var lerpAmount = Mathf.Lerp(topSide[y], heightMap[x,y], (float)1 / (x + 1));
                    heightMap[x, y] = lerpAmount;
                    averageDiff += topSide[x] - heightMap[x, y];
                }
            }

            return heightMap;
        }

        public static float[,] NormalizeBottomSide(this float[,] heightMap, float[] bottomSide)
        {
            if (bottomSide == null || bottomSide.Length == 0)
            {
                return heightMap;
            }

            for (int x = heightMap.GetLength(1) - heightMap.GetLength(1) / 4; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    var lerpAmount = Mathf.Lerp(bottomSide[y], heightMap[x,y], (float)x / heightMap.GetLength(1));
                    heightMap[x, y] = lerpAmount;
                }
            }

            for (var i = 0; i < bottomSide.Length; i++)
            {
                heightMap[heightMap.GetLength(1) - 1, i] = bottomSide[i];
            }

            return heightMap;
        }

        public static float[] GetBottomEdge(this float[,] values)
        {
            var bottomEdge = new float[values.GetLength(0)];
            for (var i = 0; i < bottomEdge.Length; i++)
            {
                bottomEdge[i] = values[values.GetLength(1) - 1, i];
            }

            return bottomEdge;
        }

        public static float[] GetLeftEdge(this float[,] values)
        {
            var leftEdge = new float[values.GetLength(1)];
            for (var i = 0; i < leftEdge.Length; i++)
            {
                leftEdge[i] = values[i, 0];
            }

            return leftEdge;
        }

        public static float[] GetRightEdge(this float[,] values)
        {
            var rightEdge = new float[values.GetLength(1)];
            for (var i = 0; i < rightEdge.Length; i++)
            {
                rightEdge[i] = values[i, values.GetLength(0) - 1];
            }

            return rightEdge;
        }

        public static float[] GetTopEdge(this float[,] values)
        {
            var topEdge = new float[values.GetLength(0)];
            for (var i = 0; i < topEdge.Length; i++)
            {
                topEdge[i] = values[0, i];
            }

            return topEdge;
        }
    }
}
