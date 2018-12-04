using System.Linq;
using UnityEngine;

public static class MeshGenerator {


	public static Mesh GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
	{
	    var width = heightMap.GetLength(0);
	    var height = heightMap.GetLength(1);

	    var meshData = new MeshData(width);

	    var topLeftX = (width - 1) / -2f;
	    var topLeftZ = (height - 1) / 2f;

	    var vertexIndex = 0;
	    for (var x = 0; x < heightMap.GetLength(0); x++)
	    {
	        for (var y = 0; y < heightMap.GetLength(0); y++)
	        {
	            meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x,y], y - topLeftZ);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
	            if (x < width - 1 && y < height - 1)
	            {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
	            }
	            vertexIndex++;
	        }
	    }

	    //var flatShadedVertices = new Vector3[triangles.Count];
	    //var flatShadedUvs = new Vector2[triangles.Count];

	    //for (var i = 0; i < triangles.Count; i++)
	    //{
     //       Debug.Log("Trying to access vertex: " + triangles[i]);
	    //    flatShadedVertices[i] = verticies[triangles[i]];
	    //    flatShadedUvs[i] = uvs[triangles[i]];
	    //    triangles[i] = i;
	    //}

        var mesh = new Mesh();
	    var debug = meshData.Triangles.Any(c => c >= (72 * 72));
	    mesh.vertices = meshData.Vertices;
	    mesh.uv = meshData.Uvs;
	    mesh.triangles = meshData.Triangles;
        mesh.RecalculateNormals();
	    

	    return mesh;
	}

    private struct MeshData
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;
        public readonly Vector2[] Uvs;

        private int _triangleIndex;

        public MeshData(int meshWidth)
        {
            Vertices = new Vector3[meshWidth * meshWidth];
            Uvs = new Vector2[Vertices.Length];
            Triangles = new int[((meshWidth -1)*(meshWidth -1))*6];
            _triangleIndex = 0;
        }

        public void AddTriangle(int a, int b, int c)
        {
            Triangles[_triangleIndex] = a;
            Triangles[_triangleIndex + 1] = b;
            Triangles[_triangleIndex + 2] = c;
            _triangleIndex += 3;
        }
    }
}
