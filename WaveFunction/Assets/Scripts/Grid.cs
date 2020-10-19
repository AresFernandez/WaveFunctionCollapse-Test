using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform gridBottomLeft;

    public int height, width, depth;
    public float cellSize;
    public float gizmoSize;

    public Color gizmoColor;
    public bool drawGizmos;

    Vector3[] cubeVertexs;

    public int CalculateGridSize()
    {
        return height * width * depth;
    }

    public Vector3[] CalculateGridPositions()
    {
        Vector3[] gridPositions = new Vector3[height * width];
        Vector3 initialPosition = gridBottomLeft.position;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                gridPositions[j + i * width] = initialPosition + new Vector3(j * cellSize, i * cellSize, 0);
            }
        }

        return gridPositions;
    }

    public Vector3[] Calculate3DGridPositions()
    {
        Vector3[] gridPositions = new Vector3[height * width * depth];
        Vector3 initialPosition = gridBottomLeft.position;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    gridPositions[i + j * width + k * depth * width] = initialPosition + new Vector3(i * cellSize, k * cellSize, j * cellSize);
                }
            }
        }

        return gridPositions;
    }

    public Vector3 GetRandomPosition()
    {
        return gridBottomLeft.position + new Vector3(Random.Range(0, width) * cellSize, Random.Range(0, height) * cellSize, 0);
    }

    public Vector3 GetRandom3DPosition()
    {
        return gridBottomLeft.position + new Vector3(Random.Range(0, width) * cellSize, Random.Range(0, height) * cellSize, Random.Range(0, depth) * cellSize);
    }

    public void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            cubeVertexs = new Vector3[]{
                new Vector3(-cellSize / 2.0f, -cellSize / 2.0f, -cellSize / 2.0f),
                new Vector3(-cellSize / 2.0f, -cellSize / 2.0f, cellSize / 2.0f),
                new Vector3(cellSize / 2.0f, -cellSize / 2.0f, cellSize / 2.0f),
                new Vector3(cellSize / 2.0f, -cellSize / 2.0f, -cellSize / 2.0f),
                new Vector3(-cellSize / 2.0f, cellSize / 2.0f, -cellSize / 2.0f),
                new Vector3(-cellSize / 2.0f, cellSize / 2.0f, cellSize / 2.0f),
                new Vector3(cellSize / 2.0f, cellSize / 2.0f, cellSize / 2.0f),
                new Vector3(cellSize / 2.0f, cellSize / 2.0f, -cellSize / 2.0f)
            };

            Vector3[] gridPositions = Calculate3DGridPositions();

            gizmoColor.a = 0.5f;
            Gizmos.color = gizmoColor;
            for (int i = 0; i < width * depth; i++)
            {
                DrawCube(gridPositions[i]);
            }

            gizmoColor.a = 1;
            Gizmos.color = gizmoColor;
            DrawOutterCube();
        }
    }

    void DrawCube(Vector3 position)
    {

        Gizmos.DrawLine(position + cubeVertexs[0], ((position + cubeVertexs[1]) - (position + cubeVertexs[0])) * gizmoSize + position + cubeVertexs[0]);
        Gizmos.DrawLine(position + cubeVertexs[0], ((position + cubeVertexs[3]) - (position + cubeVertexs[0])) * gizmoSize + position + cubeVertexs[0]);
        Gizmos.DrawLine(position + cubeVertexs[0], ((position + cubeVertexs[4]) - (position + cubeVertexs[0])) * gizmoSize + position + cubeVertexs[0]);

        Gizmos.DrawLine(position + cubeVertexs[1], ((position + cubeVertexs[0]) - (position + cubeVertexs[1])) * gizmoSize + position + cubeVertexs[1]);
        Gizmos.DrawLine(position + cubeVertexs[1], ((position + cubeVertexs[2]) - (position + cubeVertexs[1])) * gizmoSize + position + cubeVertexs[1]);
        Gizmos.DrawLine(position + cubeVertexs[1], ((position + cubeVertexs[5]) - (position + cubeVertexs[1])) * gizmoSize + position + cubeVertexs[1]);

        Gizmos.DrawLine(position + cubeVertexs[2], ((position + cubeVertexs[1]) - (position + cubeVertexs[2])) * gizmoSize + position + cubeVertexs[2]);
        Gizmos.DrawLine(position + cubeVertexs[2], ((position + cubeVertexs[3]) - (position + cubeVertexs[2])) * gizmoSize + position + cubeVertexs[2]);
        Gizmos.DrawLine(position + cubeVertexs[2], ((position + cubeVertexs[6]) - (position + cubeVertexs[2])) * gizmoSize + position + cubeVertexs[2]);

        Gizmos.DrawLine(position + cubeVertexs[3], ((position + cubeVertexs[0]) - (position + cubeVertexs[3])) * gizmoSize + position + cubeVertexs[3]);
        Gizmos.DrawLine(position + cubeVertexs[3], ((position + cubeVertexs[2]) - (position + cubeVertexs[3])) * gizmoSize + position + cubeVertexs[3]);
        Gizmos.DrawLine(position + cubeVertexs[3], ((position + cubeVertexs[7]) - (position + cubeVertexs[3])) * gizmoSize + position + cubeVertexs[3]);

        //Gizmos.DrawLine(position + cubeVertexs[4], ((position + cubeVertexs[0]) - (position + cubeVertexs[4])) * gizmoSize + position + cubeVertexs[4]);
        //Gizmos.DrawLine(position + cubeVertexs[4], ((position + cubeVertexs[7]) - (position + cubeVertexs[4])) * gizmoSize + position + cubeVertexs[4]);
        //Gizmos.DrawLine(position + cubeVertexs[4], ((position + cubeVertexs[5]) - (position + cubeVertexs[4])) * gizmoSize + position + cubeVertexs[4]);

        //Gizmos.DrawLine(position + cubeVertexs[5], ((position + cubeVertexs[1]) - (position + cubeVertexs[5])) * gizmoSize + position + cubeVertexs[5]);
        //Gizmos.DrawLine(position + cubeVertexs[5], ((position + cubeVertexs[4]) - (position + cubeVertexs[5])) * gizmoSize + position + cubeVertexs[5]);
        //Gizmos.DrawLine(position + cubeVertexs[5], ((position + cubeVertexs[6]) - (position + cubeVertexs[5])) * gizmoSize + position + cubeVertexs[5]);
        
        //Gizmos.DrawLine(position + cubeVertexs[6], ((position + cubeVertexs[7]) - (position + cubeVertexs[6])) * gizmoSize + position + cubeVertexs[6]);
        //Gizmos.DrawLine(position + cubeVertexs[6], ((position + cubeVertexs[2]) - (position + cubeVertexs[6])) * gizmoSize + position + cubeVertexs[6]);
        //Gizmos.DrawLine(position + cubeVertexs[6], ((position + cubeVertexs[5]) - (position + cubeVertexs[6])) * gizmoSize + position + cubeVertexs[6]);
        
        //Gizmos.DrawLine(position + cubeVertexs[7], ((position + cubeVertexs[3]) - (position + cubeVertexs[7])) * gizmoSize + position + cubeVertexs[7]);
        //Gizmos.DrawLine(position + cubeVertexs[7], ((position + cubeVertexs[4]) - (position + cubeVertexs[7])) * gizmoSize + position + cubeVertexs[7]);
        //Gizmos.DrawLine(position + cubeVertexs[7], ((position + cubeVertexs[6]) - (position + cubeVertexs[7])) * gizmoSize + position + cubeVertexs[7]);

    }

    private void DrawOutterCube()
    {
        //Lower Floor
        Gizmos.DrawLine(gridBottomLeft.position + cubeVertexs[0], gridBottomLeft.position + new Vector3((width - 1) * cellSize, 0, 0) + cubeVertexs[3]);
        Gizmos.DrawLine(gridBottomLeft.position + cubeVertexs[0], gridBottomLeft.position + new Vector3(0, 0, (depth - 1) * cellSize) + cubeVertexs[1]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3(0, 0, (depth - 1) * cellSize) + cubeVertexs[1], gridBottomLeft.position + new Vector3((width - 1) * cellSize, 0, (depth - 1) * cellSize) + cubeVertexs[2]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3((width - 1) * cellSize, 0, 0) + cubeVertexs[3], gridBottomLeft.position + new Vector3((width - 1) * cellSize, 0, (depth - 1) * cellSize) + cubeVertexs[2]);

        //Top Floor
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3(0, (height - 1) * cellSize, 0) + cubeVertexs[4], gridBottomLeft.position + new Vector3((width - 1) * cellSize, (height - 1) * cellSize, 0) + cubeVertexs[7]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3(0, (height - 1) * cellSize, 0) + cubeVertexs[4], gridBottomLeft.position + new Vector3(0, (height - 1) * cellSize, (depth - 1) * cellSize) + cubeVertexs[5]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3(0, (height - 1) * cellSize, (depth - 1) * cellSize) + cubeVertexs[5], gridBottomLeft.position + new Vector3((width - 1) * cellSize, (height - 1) * cellSize, (depth - 1) * cellSize) + cubeVertexs[6]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3((width - 1) * cellSize, (height - 1) * cellSize, 0) + cubeVertexs[7], gridBottomLeft.position + new Vector3((width - 1) * cellSize, (height - 1) * cellSize, (depth - 1) * cellSize) + cubeVertexs[6]);

        //Vertical
        Gizmos.DrawLine(gridBottomLeft.position + cubeVertexs[0], gridBottomLeft.position + new Vector3(0, (height - 1) * cellSize, 0) + cubeVertexs[4]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3((width - 1) * cellSize, 0, 0) + cubeVertexs[3], gridBottomLeft.position + new Vector3((width - 1) * cellSize, (height - 1) * cellSize, 0) + cubeVertexs[7]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3(0, 0, (depth - 1) * cellSize) + cubeVertexs[1], gridBottomLeft.position + new Vector3(0, (height - 1) * cellSize, (depth - 1) * cellSize) + cubeVertexs[5]);
        Gizmos.DrawLine(gridBottomLeft.position + new Vector3((width - 1) * cellSize, 0, (depth - 1) * cellSize) + cubeVertexs[2], gridBottomLeft.position + new Vector3((width - 1) * cellSize, (height - 1) * cellSize, (depth - 1) * cellSize) + cubeVertexs[6]);
    }
}
