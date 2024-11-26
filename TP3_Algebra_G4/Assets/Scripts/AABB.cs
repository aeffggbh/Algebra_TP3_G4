using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AABB : MonoBehaviour
{ 
    [SerializeField] Vector3[] vertices_test;

    Vector3[] vertices;
    Vector3 worldVertex;
    Mesh mesh;
    public Vector3 minPoint;
    public Vector3 maxPoint;
    public Color color;
    public Color defaultColor;

    private void Awake()
    {
        // Obtener el mesh del objeto
        Mesh mesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;
        // Transformar el primer vertice a coordenadas globales
        worldVertex = transform.TransformPoint(vertices[0]);

        defaultColor = Color.cyan;
        color = defaultColor;
    }

    void Update()
    {
        CalculateAABB();
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    void CalculateAABB()
    {
        vertices_test = vertices;

        // Inicializar minPoint y maxPoint con el primer vertice transformado
        minPoint = worldVertex;
        maxPoint = worldVertex;

        // Recorrer todos los vertices y actualizar min y max
        foreach (Vector3 vertex in vertices)
        {
            // Transformar cada vertice a coordenadas globales
            worldVertex = transform.TransformPoint(vertex);

            // Actualizar el minPoint y maxPoint
            minPoint = Vector3.Min(minPoint, worldVertex);
            maxPoint = Vector3.Max(maxPoint, worldVertex);
        }
    }

    // Dibujar usando Gizmos
    void OnDrawGizmos()
    {
        // Calcular la AABB si no esta calculada
        if (minPoint == Vector3.zero && maxPoint == Vector3.zero)
        {
            CalculateAABB();
        }
        Gizmos.color = color;

        // Dibujar los bordes de la AABB usando las esquinas de la box
        Gizmos.DrawLine(new Vector3(minPoint.x, minPoint.y, minPoint.z), new Vector3(minPoint.x, maxPoint.y, minPoint.z));
        Gizmos.DrawLine(new Vector3(minPoint.x, minPoint.y, minPoint.z), new Vector3(maxPoint.x, minPoint.y, minPoint.z));
        Gizmos.DrawLine(new Vector3(minPoint.x, minPoint.y, minPoint.z), new Vector3(minPoint.x, minPoint.y, maxPoint.z));

        Gizmos.DrawLine(new Vector3(maxPoint.x, maxPoint.y, maxPoint.z), new Vector3(minPoint.x, maxPoint.y, maxPoint.z));
        Gizmos.DrawLine(new Vector3(maxPoint.x, maxPoint.y, maxPoint.z), new Vector3(maxPoint.x, minPoint.y, maxPoint.z));
        Gizmos.DrawLine(new Vector3(maxPoint.x, maxPoint.y, maxPoint.z), new Vector3(maxPoint.x, maxPoint.y, minPoint.z));

        Gizmos.DrawLine(new Vector3(maxPoint.x, minPoint.y, minPoint.z), new Vector3(maxPoint.x, maxPoint.y, minPoint.z));
        Gizmos.DrawLine(new Vector3(minPoint.x, maxPoint.y, minPoint.z), new Vector3(minPoint.x, maxPoint.y, maxPoint.z));
        Gizmos.DrawLine(new Vector3(minPoint.x, minPoint.y, maxPoint.z), new Vector3(maxPoint.x, minPoint.y, maxPoint.z));

        Gizmos.DrawLine(new Vector3(minPoint.x, maxPoint.y, maxPoint.z), new Vector3(minPoint.x, minPoint.y, maxPoint.z));
        Gizmos.DrawLine(new Vector3(maxPoint.x, minPoint.y, minPoint.z), new Vector3(maxPoint.x, minPoint.y, maxPoint.z));
        Gizmos.DrawLine(new Vector3(maxPoint.x, maxPoint.y, minPoint.z), new Vector3(minPoint.x, maxPoint.y, minPoint.z));
    }
}
