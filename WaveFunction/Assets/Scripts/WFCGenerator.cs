﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor.UIElements;
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

        int safeCounter = 0;
        //Propagate recursively?
        while (affectedPieces.Count > 0 && safeCounter < 1000)
        {
            safeCounter++;
            if (safeCounter >= 1000)
            {
                Debug.LogError("Maximum iterations reached");
            }
            Propagate(affectedPieces.Dequeue());
        }
        nonCollapsedCells.Remove(cellPosition);

        SearchForLowerEntropy();
        
    }

    void InstantiatePiece(Vector3 cellPosition, GameObject piece)
    {
        Instantiate<GameObject>(piece, cellPosition, Quaternion.identity, transform);
    }

    void Propagate(CellLink affectedCell)
    {
        Debug.Log("Affected Cell: " + affectedCell.cellPosition + " has " + nonCollapsedCells[affectedCell.cellPosition].Count);


        Vector3 comingFromVector = affectedCell.cellPosition;
        List<PieceInfo> correctPieces = new List<PieceInfo>();
        switch (affectedCell.comingFrom)
        {
            case Direction.TOP:
                // We add to the list of correct pieces only the ones that can be on this cell based on the cell that we come from
                comingFromVector += new Vector3(0, grid.cellSize, 0);
                for (int i = 0; i < nonCollapsedCells[comingFromVector].Count; i++)
                {
                    for (int j = 0; j < nonCollapsedCells[comingFromVector][i].botPieces.Length; j++)
                    {
                        if (!correctPieces.Contains(nonCollapsedCells[comingFromVector][i].botPieces[j]))
                        {
                            correctPieces.Add(nonCollapsedCells[comingFromVector][i].botPieces[j]);
                        }
                    }
                }

                break;
            case Direction.BOT:
                // We add to the list of correct pieces only the ones that can be on this cell based on the cell that we come from
                comingFromVector += new Vector3(0, -grid.cellSize, 0);
                for (int i = 0; i < nonCollapsedCells[comingFromVector].Count; i++)
                {
                    for (int j = 0; j < nonCollapsedCells[comingFromVector][i].topPieces.Length; j++)
                    {
                        if (!correctPieces.Contains(nonCollapsedCells[comingFromVector][i].topPieces[j]))
                        {
                            correctPieces.Add(nonCollapsedCells[comingFromVector][i].topPieces[j]);
                        }
                    }
                }
                break;
            case Direction.RIGHT:
                // We add to the list of correct pieces only the ones that can be on this cell based on the cell that we come from
                comingFromVector += new Vector3(grid.cellSize, 0, 0);
                for (int i = 0; i < nonCollapsedCells[comingFromVector].Count; i++)
                {
                    for (int j = 0; j < nonCollapsedCells[comingFromVector][i].leftPieces.Length; j++)
                    {
                        if (!correctPieces.Contains(nonCollapsedCells[comingFromVector][i].leftPieces[j]))
                        {
                            correctPieces.Add(nonCollapsedCells[comingFromVector][i].leftPieces[j]);
                        }
                    }
                }
                break;
            case Direction.LEFT:
                // We add to the list of correct pieces only the ones that can be on this cell based on the cell that we come from
                comingFromVector += new Vector3(-grid.cellSize, 0, 0);                
                for (int i = 0; i < nonCollapsedCells[comingFromVector].Count; i++)
                {
                    for (int j = 0; j < nonCollapsedCells[comingFromVector][i].rightPieces.Length; j++)
                    {
                        if (!correctPieces.Contains(nonCollapsedCells[comingFromVector][i].rightPieces[j]))
                        {
                            correctPieces.Add(nonCollapsedCells[comingFromVector][i].rightPieces[j]);
                        }
                    }
                }
                break;
            default:
                break;
        }

        bool wasAffected = false;
        // We delete all the possible pieces in our position that don't match the correct pieces
        for (int i = nonCollapsedCells[affectedCell.cellPosition].Count - 1; i >= 0; i--)
        {
            if (!correctPieces.Contains(nonCollapsedCells[affectedCell.cellPosition][i]))
            {
                nonCollapsedCells[affectedCell.cellPosition].RemoveAt(i);
                wasAffected = true;
            }
        }


        if (wasAffected)
        {
            //We propagate to the surrounding affected cells except the one where we come from
            //top
            if (affectedCell.comingFrom != Direction.TOP && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(0, grid.cellSize, 0)))
            {
                affectedPieces.Enqueue(new CellLink(affectedCell.cellPosition + new Vector3(0, grid.cellSize, 0), Direction.BOT));
            }
            //bot
            if (affectedCell.comingFrom != Direction.BOT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(0, -grid.cellSize, 0)))
            {
                affectedPieces.Enqueue(new CellLink(affectedCell.cellPosition + new Vector3(0, -grid.cellSize, 0), Direction.TOP));
            }
            //right
            if (affectedCell.comingFrom != Direction.RIGHT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(grid.cellSize, 0, 0)))
            {
                affectedPieces.Enqueue(new CellLink(affectedCell.cellPosition + new Vector3(grid.cellSize, 0, 0), Direction.LEFT));
            }
            //left
            if (affectedCell.comingFrom != Direction.LEFT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(-grid.cellSize, 0, 0)))
            {
                affectedPieces.Enqueue(new CellLink(affectedCell.cellPosition + new Vector3(-grid.cellSize, 0, 0), Direction.RIGHT));
            }
        }
        

        Debug.Log("Affected Cell: " + affectedCell.cellPosition + " has " + nonCollapsedCells[affectedCell.cellPosition].Count);

    }


    Vector3 SearchForLowerEntropy()
    {
        Vector3 lowestEntropyCell = Vector3.zero;
        bool t = false;
        List<Vector3> alreadyCollapsedCells = new List<Vector3>();

        foreach (var cell in nonCollapsedCells)
        {
            if (!t)
            {
                t = true;
                lowestEntropyCell = cell.Key;
            }
            if (cell.Value.Count == 1)
            {
                alreadyCollapsedCells.Add(cell.Key);
            }
            else if (cell.Value.Count < nonCollapsedCells[lowestEntropyCell].Count)
            {
                lowestEntropyCell = cell.Key;
            }
        }

        for (int i = 0; i < alreadyCollapsedCells.Count; i++)
        {

            InstantiatePiece(alreadyCollapsedCells[i], nonCollapsedCells[alreadyCollapsedCells[i]][0].GOprefab);
            nonCollapsedCells.Remove(alreadyCollapsedCells[i]);
        }

        return lowestEntropyCell;
    }



}
