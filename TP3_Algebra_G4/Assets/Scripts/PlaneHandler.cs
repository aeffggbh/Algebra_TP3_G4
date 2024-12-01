using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class PlaneHandler : MonoBehaviour
{
    Mesh mesh;

    #region .
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
    #endregion

    public List<MyPlane> myPlanes = new();

    private void Awake()
    {
        CalculatePlanes();
    }

    private void Update()
    {
        RecalculatePlanes();
    }

    public void CalculatePlanes()
    {
        if (Application.isPlaying)
            mesh = GetComponentInChildren<MeshFilter>().mesh;
        else
            mesh = GetComponentInChildren<MeshFilter>().sharedMesh;

        Vector3[] vertices = mesh.vertices;
        //guarda los indices en orden para iterar los triangulos correctamente
        int[] triangles = mesh.triangles;

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

    public void RecalculatePlanes()
    {
        myPlanes.Clear();
        CalculatePlanes();
    }

    /// <summary>
    /// Check collisions with Ray Casting
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
                if (IsPointReallyInPlane(plane, myPoint))
                {
                    //se suma una interseccion
                    counter++;
                }

            }
        }

        return counter % 2 == 1;
    }

    /// <summary>
    /// Line-Plane intersections
    /// https://es.wikipedia.org/wiki/Intersecci%C3%B3n_(geometr%C3%ADa)#Una_recta_y_un_plano
    /// si agarras un punto y tiras una linea y colisiona con los planos de una manera impar, significa que esta dentro del objeto.
    /// si colisiona de manera par, esta por fuera del objeto.
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="ray"></param>
    /// <param name="point"></param>
    /// <returns></returns>

    bool IsPointInPlane(MyPlane plane, MyRay ray, out Vector3 point)
    {
        //Inicializacion del punto donde van a colisionar en 0,0,0 para empezar
        point = Vector3.zero;

        //obtengo el coseno entre el plano y el destino si es menor a cero esta ubicado
        //en el lado opuesto de la normal del plano
        //de donde origina el ray? eso lo verifica con el destino del ray
        float distance = MyDotProduct(plane.normal, ray.destination);
        if (Mathf.Abs(distance) > Vector3.kEpsilon)
        {
            // distance between the ray origin (the point) and the plane (entre 0 y la distancia y lo divido por la distancia para normalizarlo)
            // Ejemplo: El coseno me dio 35 y la distancia es 40 entonces hago 35/40 y me da un numero entre 0 y 1
            // Si el margen da mayor a cero significa que hay un punto donde colisionó el ray
            //t
            float fractionOfRay = MyDotProduct((plane.normal * plane.distance - ray.origin), plane.normal) / distance;
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
    private bool IsPointReallyInPlane(MyPlane plane, Vector3 point)
    {
        float x1 = plane.verA.x; float y1 = plane.verA.y;
        float x2 = plane.verB.x; float y2 = plane.verB.y;
        float x3 = plane.verC.x; float y3 = plane.verC.y;

        // Area del triangulo
        //multiplica las distancias entre las esquinas
        float firstTriangleArea = Mathf.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1));

        // Areas de los 3 triangulos hechos con el punto y las esquinas
        float area1 = Mathf.Abs((x1 - point.x) * (y2 - point.y) - (x2 - point.x) * (y1 - point.y));

        float area2 = Mathf.Abs((x2 - point.x) * (y3 - point.y) - (x3 - point.x) * (y2 - point.y));

        float area3 = Mathf.Abs((x3 - point.x) * (y1 - point.y) - (x1 - point.x) * (y3 - point.y));


        // Si la suma del area de los 3 triangulos es igual a la del original estamos adentro
        //Se chequea el epsilon porque por ejemplo 180 - 180 no podria dar 0 por imprecision de los floats (si uno fuera 179.099 no daria precisamente 0)
        return Math.Abs(area1 + area2 + area3 - firstTriangleArea) < Vector3.kEpsilon;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;


        foreach (MyPlane plane in myPlanes)
        {
            Gizmos.DrawRay(transform.TransformPoint(mesh.bounds.center), plane.normal);
        }

        Gizmos.color = Color.red;

    }

    public static Vector3 MyCrossProduct(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
            );
    }

    public static float MyDotProduct(Vector3 a, Vector3 b)
    {
        return a.x * b.x +
               a.y * b.y +
               a.z * b.z;
    }
}


