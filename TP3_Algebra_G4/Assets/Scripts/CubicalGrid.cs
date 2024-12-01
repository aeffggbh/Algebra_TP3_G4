using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Animations;
using UnityEngine;

[ExecuteAlways]
public class CubicalGrid : MonoBehaviour
{
    float x;
    float y;
    float z;

    float radius;

    Vector3 location;

    int rows;
    int height;
    int max;
    int rowsStart;

    float moveY;

    int horPoints;
    int verPoints;

    float space;
    int currentSphere;

    public Vector3[] grid;

    private void Awake()
    {
        horPoints = 20;
        verPoints = 20;

        rows = (horPoints * verPoints) + verPoints;

        height = verPoints;

        max = rows * height;

        grid = new Vector3[max];

        space = 0.5f;

        moveY = 0.0f;

        rowsStart = 0;

        for (int i = 0; i < height; i++)
        {

            location = new(0, moveY, 0);
            //first series of horizontal points
            for (int j = rowsStart; j < rowsStart + horPoints; j++)
            {
                grid[j] = location;

                location.x += space;

            }

            location = new(0, moveY, 0);
            //first series of vertical points
            for (int j = rowsStart + horPoints; j < rowsStart + horPoints + verPoints; j++)
            {
                location.z += space;
                grid[j] = location;
            }

            location = new(0, moveY, 0);
            location.x += space;
            location.z += space;

            currentSphere = 0;
            //the rest of the points
            for (int j = rowsStart + horPoints + verPoints; j < rowsStart + rows; j++)
            {
                if (currentSphere < horPoints)
                    currentSphere++;
                else
                {
                    currentSphere = 0;
                    location.z += space;
                    location.x = 0;
                }

                location.x += space;

                grid[j] = location;
            }

            moveY += space;

            rowsStart += rows;
        }

        radius = 0.03f;
    }

    public void BuildCube()
    {

    }

    public void DrawGrid()
    {
        Gizmos.color = Color.gray;

        for (int j = 0; j < max; j++)
        {
            Gizmos.DrawSphere(grid[j], radius);
        }

    }

    private void OnDrawGizmos()
    {
        DrawGrid();

    }

}