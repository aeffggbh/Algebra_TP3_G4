using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class Collisions : MonoBehaviour
{
    public List<GameObject> objects;

    public CubicalGrid grid;

    //-
    public GameObject objectToFind;

    private void Awake()
    {
        GameObject tempObj = GameObject.Find("CubicGrid");
        grid = tempObj.GetComponent<CubicalGrid>();
    }

    void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (CheckGridAABBCollision(objects[i]))
                for (int j = 0; j < objects.Count; j++)
                {
                    if (CheckGridAABBCollision(objects[j]))
                    {
                        if (i != j)
                        {
                            AABB aABB1 = objects[i].GetComponent<AABB>();
                            AABB aABB2 = objects[j].GetComponent<AABB>();

                            if (CheckCollisionAABB(aABB1, aABB2))
                            {
                                aABB1.SetColor(Color.red);
                                aABB2.SetColor(Color.red);

                                if (CheckGridPointCollision(objects[i], objects[j]))
                                {
                                    aABB1.SetColor(Color.magenta);
                                    aABB2.SetColor(Color.magenta);
                                }
                            }
                        }
                    }
                }

        }
    }

    [ContextMenu("ResetColors")]
    private void ResetColors()
    {
        foreach (GameObject obj in objects)
        {
            AABB aABB = obj.GetComponent<AABB>();
            aABB.SetColor(aABB.defaultColor);
        }
    }

    private bool CheckCollisionAABB(AABB aABB1, AABB aABB2)
    {
        return aABB1.maxPoint.x > aABB2.minPoint.x &&
               aABB1.minPoint.x < aABB2.maxPoint.x &&
               aABB1.maxPoint.y > aABB2.minPoint.y &&
               aABB1.minPoint.y < aABB2.maxPoint.y &&
               aABB1.maxPoint.z > aABB2.minPoint.z &&
               aABB1.minPoint.z < aABB2.maxPoint.z;
    }

    bool CheckGridPointCollision(GameObject obj1, GameObject obj2)
    {
        foreach (Vector3 point in grid.grid)
        {
            // Verificar si el punto está dentro de ambos AABB de los modelos
            if (IsPointInModel(point, obj1.GetComponent<PlaneHandler>()) && IsPointInModel(point, obj2.GetComponent<PlaneHandler>()))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPointInModel(Vector3 point, PlaneHandler product)
    {
        return product.IsPointInsideModel(point);
    }

    bool CheckGridAABBCollision(GameObject aABB)
    {
        foreach (Vector3 point in grid.grid)
        {
            // Verificar si el punto está dentro de ambos AABB de los modelos
            if (IsPointInsideAABB(point, aABB))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPointInsideAABB(Vector3 point, GameObject obj)
    {
        Vector3 min = obj.GetComponent<AABB>().minPoint;
        Vector3 max = obj.GetComponent<AABB>().maxPoint;

        return point.x >= min.x && point.x <= max.x &&
               point.y >= min.y && point.y <= max.y &&
               point.z >= min.z && point.z <= max.z;
    }
}
