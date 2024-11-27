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

    public CubicalGrid grid;

    Vector3 min1;
    Vector3 min2;
    Vector3 max1;
    Vector3 max2;

    private void Awake()
    {
        GameObject tempObj = GameObject.Find("CubicGrid");
        grid = tempObj.GetComponent<CubicalGrid>();

        min1 = Vector3.zero;
        min2 = Vector3.zero;
        max1 = Vector3.zero;
        max2 = Vector3.zero;
    }

    void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            for (int j = 0; j < objects.Count; j++)
            {
                if (i != j)
                {
                    AABB aABB1 = objects[i].GetComponent<AABB>();
                    AABB aABB2 = objects[j].GetComponent<AABB>();

                    min1 = aABB1.minPoint;
                    max1 = aABB1.maxPoint;

                    min2 = aABB2.minPoint;
                    max2 = aABB2.maxPoint;


                    if (CheckCollisionAABB())
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

    [ContextMenu("ResetColors")]
    private void ResetColors()
    {
        foreach (GameObject obj in objects)
        {
            AABB aABB = obj.GetComponent<AABB>();
            aABB.SetColor(aABB.defaultColor);
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
        foreach (Vector3 point in grid.grid)
        {
            // Verificar si el punto está dentro de ambos AABB de los modelos
            if (IsPointInModel(point, obj1.GetComponent<CrossProduct>()) && IsPointInModel(point, obj2.GetComponent<CrossProduct>()))
            {
                Debug.Log(point);
                return true;
            }
        }
        return false;
    }

    bool IsPointInModel(Vector3 point, CrossProduct product)
    {
        return product.IsPointInsideModel(point);
    }
}
