namespace Assets.Scripts.Biomes
{
    public class Biome
    {
        public float Floor { get; set; }
        public string Name { get; set; }
        public TextureData Texture { get; set; }
        public int ScalingFactor { get; set; }
        public float FlatteningRate { get; set; }

        public float[,] NormalizeHeightmap(float[,] heightMaps)
        {
            var sum = 0f;
            for (int x = 0; x < heightMaps.GetLength(0); x++)
            {
                for (int y = 0; y < heightMaps.GetLength(1); y++)
                {
                    sum += heightMaps[x, y];
                }
            }

            sum = sum / (heightMaps.GetLength(0) * heightMaps.GetLength(1));

            for (int x = 0; x < heightMaps.GetLength(0); x++)
            {
                for (int y = 0; y < heightMaps.GetLength(1); y++)
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
}
