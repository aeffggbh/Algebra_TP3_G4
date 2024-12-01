using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class CrossProduct : MonoBehaviour
{
    Mesh mesh;
    struct MyRay
    {
        //start
        public Vector3 origin;
        //end
        public Vector3 destination;

        public MyRay(Vector3 origin, Vector3 destination)
        {
            this.origin = origin;
            this.destination = destination;
        }
    }

    private Vector3 center;
    public List<(Vector3 start, Vector3 end)> normalLines = new List<(Vector3, Vector3)>();
    public List<MyPlane> myPlanes = new();

    private void Awake()
    {
        CalculateNormals();
    }

    private void Update()
    {
        RecalculateNormals();
    }

    public void CalculateNormals()
    {

        if (Application.isPlaying)
            mesh = GetComponentInChildren<MeshFilter>().mesh;
        else
            mesh = GetComponentInChildren<MeshFilter>().sharedMesh;

        Vector3[] vertices = mesh.vertices;
        //guarda los indices en orden para iterar los triangulos correctamente
        int[] triangles = mesh.triangles;

        // Obtener el centro del modelo en coordenadas globales
        center = GetComponentInChildren<MeshRenderer>().bounds.center;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            //Un objeto 3d puede utilizar el mismo vertice para varios triangulos
            //por eso consigo los indices, ellos son el orden en el que estan los vertices de cada triangulo

            //Los indices me dicen los vertices que necesito para formar un triangulo (por eso los recorro de a 3)
            int index1 = mesh.triangles[i];
            int index2 = mesh.triangles[i + 1];
            int index3 = mesh.triangles[i + 2];

            Vector3 vertex1 = transform.TransformPoint(vertices[index1]);
            Vector3 vertex2 = transform.TransformPoint(vertices[index2]);
            Vector3 vertex3 = transform.TransformPoint(vertices[index3]);

            // Calcular el centro de la cara
            Vector3 faceCenter = (vertex1 + vertex2 + vertex3) / 3;

            MyPlane thisPlane = new(vertex1, vertex2, vertex3);

            myPlanes.Add(thisPlane);

            //OPERATION TO DRAW THE NORMALS!!!

            //// Calcular la normal del triángulo usando el producto cruzado
            //Vector3 normal = myCrossProduct(vertex2 - vertex1, vertex3 - vertex1).normalized;

            //// Asegurar que la normal apunta hacia el centro del modelo
            //Vector3 directionCenterToFace = center - faceCenter;
            //if (myDotProduct(normal, directionCenterToFace) < 0)
            //{
            //    normal = -normal;
            //}

            //// Añadir la normal invertida para que apunte al centro
            //normalLines.Add((faceCenter, faceCenter + normal * directionCenterToFace.magnitude));
        }
    }

    public void RecalculateNormals()
    {
        normalLines.Clear();
        myPlanes.Clear();
        CalculateNormals();
    }

    /// <summary>
    /// Check collisions with Ray Casting
    /// Line-Plane intersections
    /// https://es.wikipedia.org/wiki/Intersecci%C3%B3n_(geometr%C3%ADa)#Una_recta_y_un_plano
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsPointInsideModel(Vector3 point)
    {
        //Create a ray con el punto que quiero chequear hacia una posicion cualquiera
        MyRay ray = new MyRay(point, Vector3.forward * 2);

        int counter = 0;

        foreach (var plane in myPlanes)
        {
            if (IsPointInPlane(plane, ray, out Vector3 myPoint))
            {
                //Esta dentro del triangulo de la mesh?
                if (IsValidPlane(plane, myPoint))
                {
                    //se suma una interseccion
                    counter++;
                }

            }
        }

        return counter % 2 == 1;
    }

    //si agarras un punto y tiras una linea y colisiona con los planos de una manera impar, significa que esta dentro del objeto.
    //si colisiona de manera par, esta por fuera del objeto.

    bool IsPointInPlane(MyPlane plane, MyRay ray, out Vector3 point)
    {
        //Inicializacion del punto donde van a colisionar en 0,0,0 para empezar
        point = Vector3.zero;

        //obtengo el coseno entre el plano y el destino  (pasa lo mismo que con el plano y los 90 grados) si es menor esta inclinado
        //al lado opuesto a la normal del plano
        float distance = Vector3.Dot(plane.normal, ray.destination);
        if (Mathf.Abs(distance) > Vector3.kEpsilon)
        {
            // distance between the ray origin (point) and the plane (entre 0 y la distancia y lo divido por la distancia para normalizarlo)
            // Ejemplo: El coseno me dio 35 y la distancia es 40 entonces hago 35/40 y me da un numero entre 0 y 1
            // Si el margen da mayor a cero significa que hay un punto donde colisionó el ray
            float fractionOfRay = Vector3.Dot((plane.normal * plane.distance - ray.origin), plane.normal) / distance;
            if (fractionOfRay >= Vector3.kEpsilon)
            {
                //punto medio entre el origen y el destino.
                // if the fraction was 0.5 I'd get the half
                // INTERPOLACION ENTRE EL INICIO Y EL FINAL (lerp)
                point = ray.origin + ray.destination * fractionOfRay;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Pitagoras
    /// No solo chequea segun el plano sino la ubicacion del modelo en sí (donde estan los triangulos que forman los planos)
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool IsValidPlane(MyPlane plane, Vector3 point)
    {
        float x1 = plane.verA.x; float y1 = plane.verA.y;
        float x2 = plane.verB.x; float y2 = plane.verB.y;
        float x3 = plane.verC.x; float y3 = plane.verC.y;

        // Area del triangulo
        float firstTriangleArea = Mathf.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1));

        // Areas de los 3 triangulos hechos con el punto y las esquinas
        float area1 = Mathf.Abs((x1 - point.x) * (y2 - point.y) - (x2 - point.x) * (y1 - point.y));
        float area2 = Mathf.Abs((x2 - point.x) * (y3 - point.y) - (x3 - point.x) * (y2 - point.y));
        float area3 = Mathf.Abs((x3 - point.x) * (y1 - point.y) - (x1 - point.x) * (y3 - point.y));


        // Si la suma del area de los 3 triangulos es igual a la del original estamos adentro
        //Se chequea el epsilon porque por ejemplo 180 - 180 no podria dar 0 por imprecision de los floats (si fueran 179.099 no daria precisamente 0)
        return Math.Abs(area1 + area2 + area3 - firstTriangleArea) < Vector3.kEpsilon;

        //chequear con los 3 puntos de plane si el punto pertence al vertice
        // si pertenece true, y sumo counter
        //si no false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Dibujar normal desde el centro de cada cara apuntando al centro del modelo
        //foreach (var line in normalLines)
        //{
        //    Gizmos.DrawLine(line.start, line.end);
        //}

        foreach (MyPlane plane in myPlanes)
        {
            Gizmos.DrawRay(transform.TransformPoint(mesh.bounds.center), plane.normal);
        }

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(center, 0.001f);
    }

    public void DrawPlane(Vector3 position, Vector3 normal, Color color)
    {
        //Vector3 v3;
        //if (normal.normalized != Vector3.forward)
        //    v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        //else
        //    v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; 

        //Vector3 corner0 = position + v3;
        //Vector3 corner2 = position - v3;
        //Quaternion q = Quaternion.AngleAxis(90.0f, normal);
        //v3 = q * v3;
        //Vector3 corner1 = position + v3;
        //Vector3 corner3 = position - v3;
        //Debug.DrawLine(corner0, corner2, color);
        //Debug.DrawLine(corner1, corner3, color);
        //Debug.DrawLine(corner0, corner1, color);
        //Debug.DrawLine(corner1, corner2, color);
        //Debug.DrawLine(corner2, corner3, color);
        //Debug.DrawLine(corner3, corner0, color);
        //Debug.DrawRay(position, normal, Color.magenta);
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
        return a.x * b.x +
               a.y * b.y +
               a.z * b.z;
    }
}


