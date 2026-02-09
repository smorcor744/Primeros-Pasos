using Unity.VectorGraphics;
using UnityEngine;

public class Planeta : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    ShapeGenerations shapeGenerator;


    [SerializeField, HideInInspector]
    MeshFilter[] meshFilter;
    TerrainFaces[] terrainFaces;

    private void OnValidate()
    {
        GeneratePlanet();
    }
    void Initialize()
    {
        shapeGenerator = new ShapeGenerations(shapeSettings);
        if (meshFilter == null || meshFilter.Length == 0)
        {
            meshFilter = new MeshFilter[6];
        }

        terrainFaces = new TerrainFaces[6];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        for (int i = 0; i < 6; i++)
        {
            if (meshFilter[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilter[i] = meshObj.AddComponent<MeshFilter>();
                meshFilter[i].mesh = new Mesh();
            }
            terrainFaces[i] = new TerrainFaces(shapeGenerator, meshFilter[i].mesh, resolution, directions[i]);
        }
    }
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }

    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFaces face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColors()
    {
        foreach (MeshFilter m in meshFilter)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }
}

