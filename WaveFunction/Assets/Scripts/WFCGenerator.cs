using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    public Grid grid;

    public PieceInfo[] piecesOnGeneration;

    Dictionary<Vector3,List<PieceInfo>> nonCollapsedCells;

    // Start is called before the first frame update
    void Start()
    {
        nonCollapsedCells = new Dictionary<Vector3, List<PieceInfo>>();
        Vector3[] cellPositions = grid.CalculateGridPositions();
        for (int i = 0; i < cellPositions.Length; i++)
        {
            nonCollapsedCells.Add(cellPositions[i], new List<PieceInfo>(piecesOnGeneration));
        }

        CollapseCell(grid.GetRandomPosition());

    }

    void CollapseCell(Vector3 cellPosition)
    {
        PieceInfo pieceToCollapse = nonCollapsedCells[cellPosition][Random.Range(0, nonCollapsedCells[cellPosition].Count)];
        Debug.Log(pieceToCollapse.name);
    }

}

