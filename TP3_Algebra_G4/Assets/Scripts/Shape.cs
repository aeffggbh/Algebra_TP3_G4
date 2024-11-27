using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Shape : MonoBehaviour
{
    public Geometry.Polygon poly;
    public Geometry.PolygonProcess polyProcess;

    Mesh mesh;
    public List<Vector3> vertices;

    private void Awake()
    {

        vertices = new List<Vector3>();

        mesh = GetComponentInChildren<MeshFilter>().sharedMesh;

        if (mesh.vertices == null)
            print("mesh is null");

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            vertices.Add(mesh.vertices[i]);
        }
    }

    public void Setup(Geometry.Polygon poly, Geometry.PolygonProcess polyProcess)
    {
        this.poly = poly;
        this.polyProcess = polyProcess;
    }
}
