using UnityEngine;

namespace Extensions
{
    public static class HeightMapExtensions
    {
        public static float[,] NormalizeLeftSide(this float[,] heightMap, float[] leftSide)
        {
            Debug.Log("Normalizing LeftSide");
            if (leftSide == null || leftSide.Length == 0)
            {
                return heightMap;
            }

            Debug.Log("Top left of array trying to be normalized to: " + leftSide[0]);
            Debug.Log("Current heightmap TopLeft: " + heightMap[0,0]);
            for (int i = 0; i < leftSide.Length; i++)
            {
                heightMap[0, i] = leftSide[i];
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
                heightMap[heightMap.GetLength(0) -1, i] = rightSide[i];
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
                heightMap[i, 0] = topSide[i];
            }

            return heightMap;
        }

        public static float[,] NormalizeBottomSide(this float[,] heightMap, float[] bottomSide)
        {
            if (bottomSide == null || bottomSide.Length == 0)
            {
                return heightMap;
            }
            for (var i = 0; i < bottomSide.Length; i++)
            {
                heightMap[i, 0] = bottomSide[i];
            }

            return heightMap;
        }
    }
}
