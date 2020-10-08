using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    public Grid grid;

    public PieceInfo[] piecesOnGeneration;

    Dictionary<Vector3,List<PieceInfo>> nonCollapsedCells;

    enum Direction { TOP, BOT, RIGHT, LEFT };

    struct CellLink
    {
        public Vector3 cellPosition;
        public Direction comingFrom;

        public CellLink(Vector3 _cellPosition, Direction _comingFrom)
        {
            cellPosition = _cellPosition;
            comingFrom = _comingFrom;
        }
    }

    //Queue  <  cellPosition , comingFrom  >
    Queue<CellLink> affectedPieces = new Queue<CellLink>();

    // Start is called before the first frame update
    void Start()
    {
        nonCollapsedCells = new Dictionary<Vector3, List<PieceInfo>>();
        Vector3[] cellPositions = grid.CalculateGridPositions();
        for (int i = 0; i < cellPositions.Length; i++)
        {
            nonCollapsedCells.Add(cellPositions[i], new List<PieceInfo>(piecesOnGeneration));
        }

        SelectPiece(grid.GetRandomPosition());

    }

    void SelectPiece(Vector3 cellPosition)
    {
        PieceInfo selectedPiece = nonCollapsedCells[cellPosition][Random.Range(0, nonCollapsedCells[cellPosition].Count)];
        nonCollapsedCells[cellPosition].Clear();
        nonCollapsedCells[cellPosition].Add(selectedPiece);
        InstantiatePiece(cellPosition, selectedPiece.GOprefab);

        //Add to Queue of affected pieces
            //top
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0,grid.cellSize,0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(0, grid.cellSize, 0), Direction.BOT));
        }
            //bot
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, -grid.cellSize, 0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(0, -grid.cellSize, 0), Direction.TOP));
        }
            //right
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(grid.cellSize,0, 0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(grid.cellSize, 0, 0), Direction.LEFT));
        }
            //left
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(-grid.cellSize, 0, 0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(-grid.cellSize, 0, 0), Direction.RIGHT));
        }

        //Propagate recursively?
        while (affectedPieces.Count > 0)
        {
            Propagate(affectedPieces.Dequeue());
        }

        nonCollapsedCells.Remove(cellPosition);
    }

    void InstantiatePiece(Vector3 cellPosition, GameObject piece)
    {
        Instantiate<GameObject>(piece, cellPosition, Quaternion.identity, transform);
    }

    void Propagate(CellLink affectedCell)
    {

    }



}

