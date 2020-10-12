using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform gridBottomLeft;
    public int height, width;
    public float cellSize;
    public float gizmoSize;

    public Color gizmoColor;
    public bool drawGizmos;

    public Vector3[] cubeVertexs;

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

    public Vector3 GetRandomPosition()
    {
        return gridBottomLeft.position + new Vector3(Random.Range(0, width) * cellSize, Random.Range(0, height) * cellSize, 0);
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

            Vector3[] gridPositions = CalculateGridPositions();

            Gizmos.color = gizmoColor;
            for (int i = 0; i < gridPositions.Length; i++)
            {
                DrawCube(gridPositions[i]);
            }
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

        Gizmos.DrawLine(position + cubeVertexs[4], ((position + cubeVertexs[0]) - (position + cubeVertexs[4])) * gizmoSize + position + cubeVertexs[4]);
        Gizmos.DrawLine(position + cubeVertexs[4], ((position + cubeVertexs[7]) - (position + cubeVertexs[4])) * gizmoSize + position + cubeVertexs[4]);
        Gizmos.DrawLine(position + cubeVertexs[4], ((position + cubeVertexs[5]) - (position + cubeVertexs[4])) * gizmoSize + position + cubeVertexs[4]);

        Gizmos.DrawLine(position + cubeVertexs[5], ((position + cubeVertexs[1]) - (position + cubeVertexs[5])) * gizmoSize + position + cubeVertexs[5]);
        Gizmos.DrawLine(position + cubeVertexs[5], ((position + cubeVertexs[4]) - (position + cubeVertexs[5])) * gizmoSize + position + cubeVertexs[5]);
        Gizmos.DrawLine(position + cubeVertexs[5], ((position + cubeVertexs[6]) - (position + cubeVertexs[5])) * gizmoSize + position + cubeVertexs[5]);

        Gizmos.DrawLine(position + cubeVertexs[6], ((position + cubeVertexs[7]) - (position + cubeVertexs[6])) * gizmoSize + position + cubeVertexs[6]);
        Gizmos.DrawLine(position + cubeVertexs[6], ((position + cubeVertexs[2]) - (position + cubeVertexs[6])) * gizmoSize + position + cubeVertexs[6]);
        Gizmos.DrawLine(position + cubeVertexs[6], ((position + cubeVertexs[5]) - (position + cubeVertexs[6])) * gizmoSize + position + cubeVertexs[6]);

        Gizmos.DrawLine(position + cubeVertexs[7], ((position + cubeVertexs[3]) - (position + cubeVertexs[7])) * gizmoSize + position + cubeVertexs[7]);
        Gizmos.DrawLine(position + cubeVertexs[7], ((position + cubeVertexs[4]) - (position + cubeVertexs[7])) * gizmoSize + position + cubeVertexs[7]);
        Gizmos.DrawLine(position + cubeVertexs[7], ((position + cubeVertexs[6]) - (position + cubeVertexs[7])) * gizmoSize + position + cubeVertexs[7]);

    }

}
