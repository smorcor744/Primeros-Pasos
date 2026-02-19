using Unity.VectorGraphics;
using UnityEngine;
using System.Collections.Generic;


public class Planeta : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public enum FaceRenderMask { All, Top, Bottom, Front, Back, Left, Right };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    ShapeGenerations shapeGenerator = new ShapeGenerations(); 
    ColourGenerator colourGenerator = new ColourGenerator();


    [SerializeField, HideInInspector]
    MeshFilter[] meshFilter;
    TerrainFaces[] terrainFaces;

    [Header("Props Instancing (GPU)")]
    public Mesh treeMesh;
    public Material treeMaterial;
    
    private List<Matrix4x4> allTreeMatrices = new List<Matrix4x4>();


    private void OnValidate()
    {
        GeneratePlanet();
    }
    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colorSettings);
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

                meshObj.AddComponent<MeshRenderer>();
                meshFilter[i] = meshObj.AddComponent<MeshFilter>();
                meshFilter[i].mesh = new Mesh();
            }
            meshFilter[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            terrainFaces[i] = new TerrainFaces(shapeGenerator, meshFilter[i].mesh, resolution, directions[i], this.transform);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilter[i].gameObject.SetActive(renderFace);
        }
    }

    void Update()
    {
        if (allTreeMatrices.Count > 0 && treeMesh != null && treeMaterial != null)
        {
            // DrawMeshInstanced solo puede dibujar de 1023 en 1023. 
            // Este bucle divide tu lista gigante en trozos de 1023.
            int batchSize = 1023;
            for (int i = 0; i < allTreeMatrices.Count; i += batchSize)
            {
                int count = Mathf.Min(batchSize, allTreeMatrices.Count - i);
                List<Matrix4x4> batch = allTreeMatrices.GetRange(i, count);
                
                Graphics.DrawMeshInstanced(treeMesh, 0, treeMaterial, batch);
            }
        }
    }
    
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
        GenerateProps();
    }

    void GenerateProps()
    {
        allTreeMatrices.Clear();
        for (int i = 0; i < 6; i++)
        {
            if (meshFilter[i].gameObject.activeSelf)
            {
                terrainFaces[i].GenerateProps();
                // Juntamos las matrices de esta cara en la lista global del planeta
                allTreeMatrices.AddRange(terrainFaces[i].propMatrices);
            }
        }
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
        for (int i = 0; i < 6; i++)
        {
            if (meshFilter[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }
    }

    void GenerateColors()
    {
        colourGenerator.UpdateColours();
        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
        for (int i = 0; i < 6; i++)
        {
            if (meshFilter[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }

    }
}

