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
    //-
    #region vars
    float x;
    float y;
    float z;

    float radius;

    Vector3 location;

    int floorSize;
    int floorsAmount;
    int max;
    int rowsStart;

    float moveY;

    int horPoints;
    int verPoints;

    float space;
    int currentHorizontalPoint;
    #endregion

    public Vector3[] grid;

    private void Awake()
    {
        //un piso de la grilla
        horPoints = 30;
        verPoints = 30;
        floorSize = horPoints * verPoints;

        //la altura de la grilla (cuántos pisos son)
        floorsAmount = verPoints/2;

        max = floorSize * floorsAmount;

        //-
        grid = new Vector3[max];

        space = 0.5f;

        moveY = 0.0f;
        rowsStart = 0;

        for (int currentFloor = 0; currentFloor < floorsAmount; currentFloor++)
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

            currentHorizontalPoint = 0;
            //the rest of the points
            for (int j = rowsStart + horPoints + verPoints; j < rowsStart + floorSize; j++)
            {
                //cada vez que termina de hacer una fila en horizontal (manejado por currentSphere)
                //procede a hacer la fila vertical (sumandole en z y poniendole la x en 0 nuevamente)

                if (currentHorizontalPoint < horPoints)
                    currentHorizontalPoint++;
                else
                {
                    currentHorizontalPoint = 0;
                    location.z += space;
                    location.x = 0;
                }

                location.x += space;

                grid[j] = location;
            }

            //mueve en y
            moveY += space;

            //posiciona en el arreglo
            rowsStart += floorSize;
        }

        radius = 0.03f;
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