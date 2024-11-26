using System.Collections.Generic;
using UnityEngine;

public class CrossProduct : MonoBehaviour
{
    private Vector3 center;
    public List<(Vector3 start, Vector3 end)> normalLines = new List<(Vector3, Vector3)>();

    void Start()
    {
        Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        //guarda los indices en orden para iterar los triangulos correctamente
        int[] triangles = mesh.triangles;

        // Obtener el centro del modelo en coordenadas globales
        center = GetComponentInChildren<MeshRenderer>().bounds.center;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            Vector3 vertex1 = transform.TransformPoint(vertices[index1]);
            Vector3 vertex2 = transform.TransformPoint(vertices[index2]);
            Vector3 vertex3 = transform.TransformPoint(vertices[index3]);

            //REPASAR A PARTIR DE ACÁ!

            // Calcular el centro de la cara (en espacio global)
            Vector3 faceCenter = (vertex1 + vertex2 + vertex3) / 3;

            // Calcular la normal del triángulo usando el producto cruzado
            Vector3 normal = myCrossProduct(vertex2 - vertex1, vertex3 - vertex1).normalized;

            // Asegurar que la normal apunta hacia el centro del modelo
            Vector3 directionCenterToFace = center - faceCenter;
            if (myDotProduct(normal, directionCenterToFace) < 0)
            {
                normal = -normal;
            }

            // Añadir la normal invertida para que apunte al centro
            normalLines.Add((faceCenter, faceCenter + normal * directionCenterToFace.magnitude));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Dibujar normal desde el centro de cada cara apuntando al centro del modelo
        foreach (var line in normalLines)
        {
            Gizmos.DrawLine(line.start, line.end);
        }
    }

    // Producto Cruzado
    private Vector3 myCrossProduct(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.y * b.z - a.z * b.y, 
            a.z * b.x - a.x * b.z, 
            a.x * b.y - a.y * b.x  
        );
    }

    // Producto punto
    private float myDotProduct(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }
}
