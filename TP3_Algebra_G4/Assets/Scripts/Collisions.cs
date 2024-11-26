using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class Collisions : MonoBehaviour
{
    public List<GameObject> objects;

    public GameObject objectToFind;

    public CubicalGrid gridAux;

    Vector3 min1;
    Vector3 min2;
    Vector3 max1;
    Vector3 max2;

    private void Start()
    {
        GameObject tempObj = GameObject.Find("CubicGrid");
        gridAux = tempObj.GetComponent<CubicalGrid>();

        objects = new List<GameObject>();

        objectToFind = GameObject.Find("dodecahedron");

        objects.Add(objectToFind);

        objectToFind = GameObject.Find("tetrahedron");

        objects.Add(objectToFind);

        objectToFind = GameObject.Find("icosahedron");

        objects.Add(objectToFind);

        objectToFind = GameObject.Find("cube");

        objects.Add(objectToFind);

        objectToFind = GameObject.Find("decahedron");

        objects.Add(objectToFind);

        objectToFind = GameObject.Find("octahedron");

        objects.Add(objectToFind);

        min1 = Vector3.zero;
        min2 = Vector3.zero;
        max1 = Vector3.zero;
        max2 = Vector3.zero;

        //Setup Geometry

        for (int i = 0; i < objects.Count; i++)
        {
            Geometry.Polygon poly = new(objects[i].GetComponent<Shape>().vertices);
            Geometry.PolygonProcess polyProcess = new(poly);

            objects[i].GetComponent<Shape>().Setup(poly, polyProcess);
        }
    }

    void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            for (int j = 0; j < objects.Count; j++)
            {
                if (i != j)
                {
                    min1 = objects[i].GetComponent<AABB>().minPoint;
                    max1 = objects[i].GetComponent<AABB>().maxPoint;

                    min2 = objects[j].GetComponent<AABB>().minPoint;
                    max2 = objects[j].GetComponent<AABB>().maxPoint;


                    if (CheckCollisionAABB())
                    {
                        objects[i].GetComponent<AABB>().SetColor(Color.red);
                        objects[j].GetComponent<AABB>().SetColor(Color.red);

                        // Si hay AABB, chequear colisi�n por grilla
                        if (CheckGridPointCollision(objects[i], objects[j]))
                        {
                            objects[i].GetComponent<AABB>().SetColor(Color.magenta);
                            objects[j].GetComponent<AABB>().SetColor(Color.magenta);
                        }
                    }
                }
            }

        }
    }

    private bool CheckCollisionAABB()
    {
        return max1.x > min2.x &&
               min1.x < max2.x &&
               max1.y > min2.y &&
               min1.y < max2.y &&
               max1.z > min2.z &&
               min1.z < max2.z;
    }

    bool CheckGridPointCollision(GameObject obj1, GameObject obj2)
    {
        foreach (Vector3 point in gridAux.grid)
        {
            // Verificar si el punto est� dentro de ambos AABB de los modelos
            if (IsPointInModel(point, obj1.GetComponent<Shape>().polyProcess) && IsPointInModel(point, obj2.GetComponent<Shape>().polyProcess))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPointInModel(Vector3 point, Geometry.PolygonProcess polyProcess)
    {
        return polyProcess.IsPointInPolygon(point);
    }
}
