using Biomes;
using TerrainChunkObjects;
using UnityEngine;

public class TerrainChunk {
	
    const float ColliderGenerationDistanceThreshold = 5;
    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    public Vector2 Coord;

    readonly GameObject _meshObject;
    readonly Vector2 _unnormalizedCenter;
    Bounds _bounds;

    readonly MeshRenderer _meshRenderer;
    readonly MeshFilter _meshFilter;
    readonly MeshCollider _meshCollider;
    private readonly TerrainChunkGameObject _terrainChunkGameObject;

    private Mesh _chunkMesh;
    readonly int _colliderLodIndex;

    public HeightMap HeightMap;
    bool _hasSetCollider;
    readonly float _maxViewDst;

    public Biome Biome;

    readonly HeightMapSettings _heightMapSettings;
    readonly MeshSettings _meshSettings;
    readonly Transform _viewer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, int colliderLodIndex, Transform parent, Transform viewer, Material material) {
        this.Coord = coord;
        this._colliderLodIndex = colliderLodIndex;
        this._heightMapSettings = heightMapSettings;
        this._meshSettings = meshSettings;
        this._viewer = viewer;
        this.Biome = BiomeGenerator.GenerateRelativeBiome(_unnormalizedCenter);

        _unnormalizedCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        var position = coord * meshSettings.meshWorldSize;
        _bounds = new Bounds(position,Vector2.one * meshSettings.meshWorldSize );


        _meshObject = new GameObject("Terrain Chunk");
        _meshObject.transform.localScale = new Vector3(10.2f, 10.2f, 10.2f);
        
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();
        _terrainChunkGameObject = _meshObject.AddComponent<TerrainChunkGameObject>();
        SetNeighboringChunks();
        _meshRenderer.material = material;

        _meshObject.transform.position = new Vector3(position.x,0,position.y);
        _meshObject.transform.parent = parent;
        SetVisible(false);
        _maxViewDst = 400f;
    }

    private void SetNeighboringChunks()
    {
        var neighbors = TerrainRepository.GetChunksWithinDistance(_unnormalizedCenter);
        if (neighbors.Left != null)
        {
            _terrainChunkGameObject.Left = neighbors.Left._terrainChunkGameObject;
            neighbors.Left._terrainChunkGameObject.Right = _terrainChunkGameObject;
        }

        if (neighbors.Right != null)
        {
            _terrainChunkGameObject.Right = neighbors.Right._terrainChunkGameObject;
            neighbors.Right._terrainChunkGameObject.Left = _terrainChunkGameObject;
        }

        if (neighbors.Above != null)
        {
            _terrainChunkGameObject.Above = neighbors.Above._terrainChunkGameObject;
            neighbors.Above._terrainChunkGameObject.Below = _terrainChunkGameObject;
        }

        if (neighbors.Below != null)
        {
            _terrainChunkGameObject.Below = neighbors.Below._terrainChunkGameObject;
            neighbors.Below._terrainChunkGameObject.Above = _terrainChunkGameObject;
        }
    }

    public void Load()
    {
        var heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.numVertsPerLine,
                                                             _meshSettings.numVertsPerLine, _heightMapSettings,
                                                             _unnormalizedCenter);
        _terrainChunkGameObject.HeightMap = heightMap.Values;
        OnHeightMapReceived(heightMap);
    }


    private void OnHeightMapReceived(HeightMap heightMapObject) {
        HeightMap = heightMapObject;
        _chunkMesh = MeshGenerator.GenerateTerrainMesh(HeightMap.Values, _meshSettings, 3);
        _meshFilter.mesh = _chunkMesh;
        SetVisible(true);
    }

    public void SetVisible(bool visible) {
        _meshObject.SetActive (visible);
    }

    public bool IsVisible() {
        return _meshObject.activeSelf;
    }

}
