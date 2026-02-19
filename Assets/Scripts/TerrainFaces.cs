using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class TerrainFaces
{
    
    ShapeGenerations shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;


    Transform planetTransform;
    public List<Matrix4x4> propMatrices = new List<Matrix4x4>();

    public TerrainFaces(ShapeGenerations shapeGenerator, Mesh mesh, int resolution, Vector3 localUp,Transform planetTransform)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.planetTransform = planetTransform;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        Vector2[] uvs = mesh.uv;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitCube.normalized);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uvs;
    }

    public void UpdateUVs(ColourGenerator colourGenerator)
    {
        Vector2[] uvs = new Vector2[resolution * resolution];
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitSphere = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                uvs[i] = new Vector2(colourGenerator.BiomePercentFromPoint(pointOnUnitSphere), 0);
            }
        }
        mesh.uv = uvs;
    }
    public void GenerateProps()
    {
        propMatrices.Clear();

        // Recorremos los vértices de esta cara
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 localVertexPos = mesh.vertices[i];

            // Probabilidad aleatoria de que haya un árbol en este vértice (ej. 2% de probabilidad)
            if (Random.value < 0.02f) 
            {
                // 1. Posición en el mundo
                Vector3 worldPos = planetTransform.TransformPoint(localVertexPos);
                
                // 2. Rotación apuntando hacia afuera
                Vector3 upDirection = localVertexPos.normalized;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, upDirection) * planetTransform.rotation;
                
                // 3. Empaquetamos en matriz y guardamos (Escala 1,1,1)
                Matrix4x4 matrix = Matrix4x4.TRS(worldPos, rotation, Vector3.one);
                propMatrices.Add(matrix);
            }
        }
    }

}
