using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Biomes;
using Extensions;
using UnityEditor.VersionControl;
using UnityEngine;
using Object = UnityEngine.Object;

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

    readonly LODInfo[] _detailLevels;
    readonly LODMesh[] _lodMeshes;
    readonly int _colliderLodIndex;

    public HeightMap HeightMap;
    bool _heightMapReceived;
    int _previousLodIndex = -1;
    bool _hasSetCollider;
    readonly float _maxViewDst;

    public Biome Biome;

    readonly HeightMapSettings _heightMapSettings;
    readonly MeshSettings _meshSettings;
    readonly Transform _viewer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLodIndex, Transform parent, Transform viewer, Material material) {
        this.Coord = coord;
        this._detailLevels = detailLevels;
        this._colliderLodIndex = colliderLodIndex;
        this._heightMapSettings = heightMapSettings;
        this._meshSettings = meshSettings;
        this._viewer = viewer;
        this.Biome = BiomeGenerator.GenerateRelativeBiome(this);

        _sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        var position = coord * meshSettings.meshWorldSize;
        _bounds = new Bounds(position,Vector2.one * meshSettings.meshWorldSize );


        _meshObject = new GameObject("Terrain Chunk");
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();
        _meshRenderer.material = material;

        _meshObject.transform.position = new Vector3(position.x,0,position.y);
        _meshObject.transform.parent = parent;
        SetVisible(false);

        _lodMeshes = new LODMesh[detailLevels.Length];
        for (var i = 0; i < detailLevels.Length; i++) {
            _lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            _lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLodIndex) {
                _lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        _maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;

    }

    public void Load() {
        OnHeightMapReceived(HeightMapGenerator.GenerateHeightMap (_meshSettings.numVertsPerLine, _meshSettings.numVertsPerLine, _heightMapSettings, _sampleCentre));
    }


    private void OnHeightMapReceived(HeightMap heightMapObject) {
        HeightMap = heightMapObject;
        _heightMapReceived = true;
        UpdateTerrainChunk ();
    }

    Vector2 ViewerPosition {
        get {
            return new Vector2 (_viewer.position.x, _viewer.position.z);
        }
    }


    public void UpdateTerrainChunk() {
        if (_heightMapReceived) {
            var viewerDstFromNearestEdge = Mathf.Sqrt (_bounds.SqrDistance (ViewerPosition));

            var wasVisible = IsVisible ();
            var visible = viewerDstFromNearestEdge <= _maxViewDst;

            if (visible) {
                var lodIndex = 0;

                for (var i = 0; i < _detailLevels.Length - 1; i++) {
                    if (viewerDstFromNearestEdge > _detailLevels [i].visibleDstThreshold) {
                        lodIndex = i + 1;
                    } else {
                        break;
                    }
                }

                if (lodIndex != _previousLodIndex) {
                    var lodMesh = _lodMeshes [lodIndex];
                    if (lodMesh.hasMesh) {
                        _previousLodIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.mesh;
                    } else if (!lodMesh.hasRequestedMesh) {
                        lodMesh.RequestMesh (HeightMap, _meshSettings);
                    }
                }


            }

            if (wasVisible != visible) {
				
                SetVisible (visible);
                if (OnVisibilityChanged != null) {
                    OnVisibilityChanged (this, visible);
                }
            }
        }
    }

    public void UpdateCollisionMesh() {
        if (!_hasSetCollider) {
            var sqrDstFromViewerToEdge = _bounds.SqrDistance (ViewerPosition);

            if (sqrDstFromViewerToEdge < _detailLevels [_colliderLodIndex].sqrVisibleDstThreshold) {
                if (!_lodMeshes [_colliderLodIndex].hasRequestedMesh) {
                    _lodMeshes [_colliderLodIndex].RequestMesh (HeightMap, _meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < ColliderGenerationDistanceThreshold * ColliderGenerationDistanceThreshold) {
                if (_lodMeshes [_colliderLodIndex].hasMesh) {
                    _meshCollider.sharedMesh = _lodMeshes [_colliderLodIndex].mesh;
                    _hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible) {
        _meshObject.SetActive (visible);
    }

    public bool IsVisible() {
        return _meshObject.activeSelf;
    }

}

class LODMesh {

    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    readonly int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod) {
        this.lod = lod;
    }

    void OnMeshDataReceived(object meshDataObject) {
        mesh = ((MeshData)meshDataObject).CreateMesh ();
        hasMesh = true;

        updateCallback ();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData (() => MeshGenerator.GenerateTerrainMesh (heightMap.Values, meshSettings, lod), OnMeshDataReceived);
    }

}