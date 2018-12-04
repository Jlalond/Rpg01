using Biomes;
using UnityEngine;

public class TerrainChunk {
	
    const float ColliderGenerationDistanceThreshold = 5;
    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    public Vector2 Coord;

    readonly GameObject _meshObject;
    readonly Vector2 _sampleCentre;
    Bounds _bounds;

    readonly MeshRenderer _meshRenderer;
    readonly MeshFilter _meshFilter;
    readonly MeshCollider _meshCollider;

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
        this.Biome = BiomeGenerator.GenerateRelativeBiome(this);

        _sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        var position = coord * meshSettings.meshWorldSize;
        _bounds = new Bounds(position,Vector2.one * meshSettings.meshWorldSize );


        _meshObject = new GameObject("Terrain Chunk");
        _meshObject.transform.localScale = new Vector3(10.2f, 10.2f, 10.2f);
        
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();
        _meshRenderer.material = material;

        _meshObject.transform.position = new Vector3(position.x,0,position.y);
        _meshObject.transform.parent = parent;
        SetVisible(false);
        _maxViewDst = 400f;
    }

    public void Load() {
        OnHeightMapReceived(HeightMapGenerator.GenerateHeightMap (_meshSettings.numVertsPerLine, _meshSettings.numVertsPerLine, _heightMapSettings, _sampleCentre));
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
