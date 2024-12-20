using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//4

[ExecuteAlways]
public class AABB : MonoBehaviour
{
    #region vars
    Vector3[] vertices;
    Vector3 worldVertex;
    public Vector3 minPoint;
    public Vector3 maxPoint;
    public Color color;
    public Color defaultColor;

    #endregion

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
