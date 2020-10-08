using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform gridBottomLeft;
    public int height, width;
    public float cellSize;

    public GameObject prefabGOTest;
    public bool instantiatePrefabsTest;

    public Vector3[] CalculateGridPositions()
    {
        Vector3[] gridPositions = new Vector3[height*width];
        Vector3 initialPosition = gridBottomLeft.position;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                gridPositions[j+i*width] = initialPosition + new Vector3(j * cellSize, i * cellSize, 0);
                if (instantiatePrefabsTest)
                {
                    Instantiate<GameObject>(prefabGOTest, gridPositions[j+i*width], Quaternion.identity, transform);
                }
            }
        }

        return gridPositions;
    }

    public Vector3 GetRandomPosition()
    {
        return gridBottomLeft.position + new Vector3(Random.Range(0, width) * cellSize, Random.Range(0, height) * cellSize, 0);
    }

    //private void Start()
    //{
    //    CalculateGridPositions();
    //}

}
