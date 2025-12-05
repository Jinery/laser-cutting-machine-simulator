using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CuttableMaterial : MonoBehaviour
{
    [SerializeField] private Renderer _materialRenderer;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private float _cutRadius = 0.1f;
    [SerializeField] private bool _physicsAfterCut = true;

    private Material _materialInstance;
    private Texture2D _cutTexture;
    private bool[,] _cutMask;
    private bool _isInitialized = false;

    private const int TEXTURE_SIZE = 256;
    
    private HashSet<int> _cutVericles = new HashSet<int>();
    private Mesh _originalMesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_materialRenderer != null && _meshFilter != null)
        {
            _materialInstance = _materialRenderer.material;
            _originalMesh = _meshFilter.mesh;

            _meshFilter.mesh = Instantiate(_originalMesh);

            _cutTexture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE);
            _cutTexture.wrapMode = TextureWrapMode.Clamp;
            Color[] clearPixels = new Color[TEXTURE_SIZE * TEXTURE_SIZE];
            for (int index = 0; index < clearPixels.Length; index++)
            {
                clearPixels[index] = Color.clear;
            }

            _cutTexture.SetPixels(clearPixels);
            _cutTexture.Apply();

            _materialInstance.SetTexture("_CutMap", _cutTexture);

            _cutMask = new bool[TEXTURE_SIZE, TEXTURE_SIZE];
        }
        _isInitialized = true;
    }

    public void CutAtPosition(Vector3 gridPosition)
    {
        if (!_isInitialized) return;

        Debug.Log($"Cutting at: {gridPosition}");

        Vector2 uv = WorldToUV(gridPosition);
        if (UpdateCutMasK(uv))
        {
            UpdateCutTexture();
            UpdateMeshGeometry();
        }
    }

    private Vector2 WorldToUV(Vector3 worldPosition)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);
        float u = (localPos.x + 0.5f);
        float v = (localPos.y + 0.5f);

        return new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));
    }


    private bool UpdateCutMasK(Vector2 uv)
    {
        int centerX = Mathf.RoundToInt(uv.x * TEXTURE_SIZE);
        int centerY = Mathf.RoundToInt(uv.y * TEXTURE_SIZE);
        int radius = Mathf.RoundToInt(_cutRadius * TEXTURE_SIZE);

        bool madeCut = false;

        for(int x = centerX - radius; x <= centerX + radius; x++)
        {
            for(int y = centerY - radius; y <= centerY + radius; y++)
            {
                if(x >= 0 && x < TEXTURE_SIZE && y >= 0 && y < TEXTURE_SIZE)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    if(distance <= radius && !_cutMask[x, y])
                    {
                        _cutMask[x, y] = true;
                        madeCut = true;
                    }
                }
            }
        }
        return madeCut;
    }

    private void UpdateCutTexture()
    {
        Color[] pixels = _cutTexture.GetPixels();

        for(int x = 0;  x < TEXTURE_SIZE; x++)
        {
            for(int y = 0; y < TEXTURE_SIZE; y++)
            {
                int index = y * TEXTURE_SIZE + x;
                if (_cutMask[x, y])
                {
                    pixels[index] = Color.red;
                }
            }
        }

        _cutTexture.SetPixels(pixels);
        _cutTexture.Apply();
    }

    private void UpdateMeshGeometry()
    {
        Mesh mesh = _meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();
        List<int> newTriangles = new List<int>();

        Dictionary<int, int> vertexMap = new Dictionary<int, int>();

        for (int index = 0; index < vertices.Length; index++)
        {
            Vector2 uv = WorldToUV(transform.TransformPoint(vertices[index]));
            int texX = Mathf.RoundToInt(uv.x * TEXTURE_SIZE);
            int texY = Mathf.RoundToInt(uv.y * TEXTURE_SIZE);

            bool isCut = texX >= 0 && texX < TEXTURE_SIZE && texY >= 0 && texY < TEXTURE_SIZE && _cutMask[texX, texY];
            if(!isCut)
            {
                vertexMap[index] = newVertices.Count;
                newVertices.Add(vertices[index]);
                newNormals.Add(normals[index]);
                newUVs.Add(uvs[index]);
            }
        }

        for (int index = 0; index < triangles.Length; index += 3)
        {
            int v1 = triangles[index];
            int v2 = triangles[index + 1];
            int v3 = triangles[index + 2];

            if(vertexMap.ContainsKey(v1) && vertexMap.ContainsKey(v2) && vertexMap.ContainsKey(v3))
            {
                newTriangles.Add(vertexMap[v1]);
                newTriangles.Add(vertexMap[v2]);
                newTriangles.Add(vertexMap[v3]);
            }
        }

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.normals = newNormals.ToArray();
        mesh.uv = newUVs.ToArray();
        mesh.triangles = newTriangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        if (_physicsAfterCut && newVertices.Count > 0)
        {
            AddPhysicsComponents();
        }
    }

    private void AddPhysicsComponents()
    {
        if (GetComponent<MeshCollider>() == null)
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = _meshFilter.mesh;
            collider.convex = true;
        }

        if (_physicsAfterCut && GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.1f;

            XRGrabInteractable grab = gameObject.AddComponent<XRGrabInteractable>();
            grab.interactionManager = FindFirstObjectByType<XRInteractionManager>();
            grab.smoothPosition = true;
            grab.smoothRotation = true;
            grab.useDynamicAttach = true;
            grab.interactionLayers = InteractionLayerMask.GetMask("Cuttable");
        }
    }
}
