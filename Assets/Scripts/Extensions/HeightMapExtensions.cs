using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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

            //var averageDiff = 0f;
            //var count = 0;
            //for (int x = 0; x < 10; x++)
            //{
            //    for (int y = 0; y < heightMap.GetLength(1); y++)
            //    {
            //        count++;
            //        averageDiff += leftSide[x] - heightMap[x, y];
            //    }
            //}

            //averageDiff = averageDiff / count;

            //for (int x = 0; x < heightMap.GetLength(0) / 4; x++)
            //{
            //    for (int y = 0; y < heightMap.GetLength(1); y++)
            //    {
            //        var value = heightMap[x, y];
            //        if (value > averageDiff)
            //        {
            //            heightMap[x, y] -= averageDiff / 1 + Math.Abs(1 + x); // divided by the offset from the left most edge, 1 being the left most edge 
            //        }
            //        else
            //        {
            //            heightMap[x, y] += averageDiff / 1 + Math.Abs(1 + x);
            //        }
            //    }
            //}

            return heightMap;
        }


        public static float[,] NormalizeRightSide(this float[,] heightMap, float[] rightSide)
        {
            if (rightSide == null || rightSide.Length == 0)
            {
                return heightMap;
            }
            var start = heightMap.GetLength(1) / 4;
            var percentagePerIndex = 1f / start;
            var iterationAmount = 1f;

            for (int x = start; x > -1; x--)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    var diffFromStart = x - start;
                    if (diffFromStart == 0)
                    {
                        var lerpAmount = Mathf.Lerp(rightSide[y], heightMap[x, y], 1);
                        heightMap[x, y] = RandomizeLerpAmount(lerpAmount);
                    }
                    else
                    {
                        var lerpAmount = Mathf.Lerp(rightSide[y], heightMap[x, y], iterationAmount);
                        heightMap[x, y] = RandomizeLerpAmount(lerpAmount);
                    }
                }

                iterationAmount -= percentagePerIndex;
            }

            return heightMap;
        }

        public static float[,] NormalizeTopSide(this float[,] heightMap, float[] topSide)
        {
            if (topSide == null || topSide.Length == 0)
            {
                return heightMap;
            }

            var start = heightMap.GetLength(1) / 4;
            var percentagePerIndex = 1f / start;
            var iterationAmount = 1f;

            for (int x = start; x > -1; x--)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    var diffFromStart = x - start;
                    if (diffFromStart == 0)
                    {
                        var lerpAmount = Mathf.Lerp(topSide[y], heightMap[x,y],  1);
                        heightMap[x, y] = RandomizeLerpAmount(lerpAmount);
                    }
                    else
                    {
                        var lerpAmount = Mathf.Lerp(topSide[y], heightMap[x,y], iterationAmount);
                        heightMap[x, y] = RandomizeLerpAmount(lerpAmount);
                    }
                }

                iterationAmount -= percentagePerIndex;
            }

            return heightMap;
        }

        public static float[,] NormalizeBottomSide(this float[,] heightMap, float[] bottomSide)
        {
            if (bottomSide == null || bottomSide.Length == 0)
            {
                return heightMap;
            }

            Debug.Break();

            var distanceFromStartToBottom = Mathf.Abs(heightMap.GetLength(1) - (heightMap.GetLength(1) - heightMap.GetLength(1) / 4));
            var start = heightMap.GetLength(1) - heightMap.GetLength(1) / 4;
            var percentagePerIndex = 1f / distanceFromStartToBottom;
            var iterationAmount = 0f;
            for (var x = start; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    var diffFromStart = x - start;
                    if (diffFromStart == 0)
                    {
                        var lerpAmount = Mathf.Lerp(heightMap[start, y], bottomSide[y], 0);
                        heightMap[x, y] = RandomizeLerpAmount(lerpAmount);
                    }
                    else
                    {
                        var lerpAmount = Mathf.Lerp(heightMap[start, y], bottomSide[y], iterationAmount);
                        heightMap[x, y] = RandomizeLerpAmount(lerpAmount);
                    }
                }

                iterationAmount += percentagePerIndex;
            }

            return heightMap;
        }

        public static float[,] BumpifyMatrix(this float[,] heightMap)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    if (x != 0 && y != 0 && x != heightMap.GetLength(0) - 1 && y != heightMap.GetLength(1) - 1)
                    {
                        var percent = Random.Range(1, 2);
                        var diceRoll = Random.Range(0, 100);
                        if (diceRoll > 80)
                        {
                            var diff = (heightMap[x, y] * (1 + (percent / 100f)));
                            heightMap[x, y] = diff;
                        }
                        else if (diceRoll > 60)
                        {
                            var diff = (heightMap[x, y] * (1 - percent / 100f)); ;
                            heightMap[x, y] = diff;
                        }
                    }
                }
            }

            return heightMap;
        }

        private static float RandomizeLerpAmount(float lerpAmount)
        {
            var percent = Random.Range(1, 2);
            var diceRoll = Random.Range(0, 100);
            if (diceRoll > 80)
            {
                return lerpAmount * (1 + (percent / 100f));
            }
            else if(diceRoll < 20)
            {
                return lerpAmount * (1 - percent / 100f);
            }

            return lerpAmount;
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
