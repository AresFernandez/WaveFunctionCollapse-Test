using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    public Grid grid;

    [SerializeField] TileCollection tileCollection;

    Dictionary<Vector3, List<PieceInfo>> nonCollapsedCells;

    public PieceInfo emptyPiece;

    enum Direction { TOP, BOT, RIGHT, LEFT, FRONT, BACK };

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
        Vector3[] cellPositions = grid.Calculate3DGridPositions();

        List<PieceInfo> allPieces = new List<PieceInfo>();
        for (int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            allPieces.Add(tileCollection.piecesOnCollection[i].piece);

        for (int i = 0; i < cellPositions.Length; i++)
        {
            nonCollapsedCells.Add(cellPositions[i], new List<PieceInfo>(allPieces));
        }

        // We select the first cell randomly because at the begginning they have all the same entropy
        Vector3 randomPosition = grid.GetRandom3DPosition();
        SelectPiece(randomPosition);

        // We keep collapsing cells priorizing those with the lowest entropy
        int entropyValue = 0;
        Vector3 lowestEntropyCell = Vector3.zero;
        while (nonCollapsedCells.Count > 0)
        {
            lowestEntropyCell = SearchForLowerEntropy(out entropyValue);
            Debug.Log("Lowest Entropy Cell is: " + lowestEntropyCell + " with value: " + entropyValue);

            if(entropyValue != -1)
                SelectPiece(lowestEntropyCell);
        }
    }

     void OnDrawGizmos()
     {
        Handles.color = Color.white;
        if(nonCollapsedCells.Count > 0)
            foreach(KeyValuePair<Vector3, List<PieceInfo>> cell in nonCollapsedCells)
                Handles.Label(cell.Key, cell.Key + " / " + cell.Value.Count);
     }

    void SelectPiece(Vector3 cellPosition)
    {
        PieceInfo selectedPiece = nonCollapsedCells[cellPosition][Random.Range(0, nonCollapsedCells[cellPosition].Count)];
        nonCollapsedCells[cellPosition].Clear();
        nonCollapsedCells[cellPosition].Add(selectedPiece);
        //Debug.Log("Piece Selected for position: " + cellPosition + " is: " + selectedPiece.name);

        InstantiatePiece(cellPosition, selectedPiece);

        //Add to Queue of affected pieces
        //top
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, grid.cellSize, 0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(0, grid.cellSize, 0), Direction.BOT));
        }
        //bot
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, -grid.cellSize, 0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(0, -grid.cellSize, 0), Direction.TOP));
        }
        //right
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(grid.cellSize, 0, 0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(grid.cellSize, 0, 0), Direction.LEFT));
        }
        //left
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(-grid.cellSize, 0, 0)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(-grid.cellSize, 0, 0), Direction.RIGHT));
        }
        //front
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, 0, grid.cellSize)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(0, 0, grid.cellSize), Direction.BACK));
        }
        //back
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, 0, -grid.cellSize)))
        {
            affectedPieces.Enqueue(new CellLink(cellPosition + new Vector3(0, 0, -grid.cellSize), Direction.FRONT));
        }

        //Propagate recursively?
        int safeCounter = 0;
        while (affectedPieces.Count > 0 && safeCounter < 10000)
        {
            safeCounter++;
            if (safeCounter >= 10000)
                Debug.LogError("MAX ITERATIONS");
            Propagate(affectedPieces.Dequeue());
        }

        nonCollapsedCells.Remove(cellPosition);
    }

    void InstantiatePiece(Vector3 cellPosition, PieceInfo piece)
    {
        GameObject newPiece = Instantiate<GameObject>(piece.piecePrefab, cellPosition, piece.piecePrefab.transform.rotation, transform);
        newPiece.transform.position -= new Vector3(0, grid.cellSize / 2.0f, 0);
        newPiece.transform.eulerAngles += new Vector3(0, 0, 90) * piece.rotation;
    }

    void Propagate(CellLink affectedCell)
    {
        //Debug.Log("Affected Cell: " + affectedCell.cellPosition + " has " + nonCollapsedCells[affectedCell.cellPosition].Count);

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
            case Direction.FRONT:
                // We add to the list of correct pieces only the ones that can be on this cell based on the cell that we come from
                comingFromVector += new Vector3(0, 0, grid.cellSize);
                for (int i = 0; i < nonCollapsedCells[comingFromVector].Count; i++)
                {
                    for (int j = 0; j < nonCollapsedCells[comingFromVector][i].backPieces.Length; j++)
                    {
                        if (!correctPieces.Contains(nonCollapsedCells[comingFromVector][i].backPieces[j]))
                        {
                            correctPieces.Add(nonCollapsedCells[comingFromVector][i].backPieces[j]);
                        }
                    }
                }
                break;
            case Direction.BACK:
                // We add to the list of correct pieces only the ones that can be on this cell based on the cell that we come from
                comingFromVector += new Vector3(0, 0, -grid.cellSize);
                for (int i = 0; i < nonCollapsedCells[comingFromVector].Count; i++)
                {
                    for (int j = 0; j < nonCollapsedCells[comingFromVector][i].frontPieces.Length; j++)
                    {
                        if (!correctPieces.Contains(nonCollapsedCells[comingFromVector][i].frontPieces[j]))
                        {
                            correctPieces.Add(nonCollapsedCells[comingFromVector][i].frontPieces[j]);
                        }
                    }
                }
                break;
            default:
                break;
        }

        Debug.Log("Cell with position: " + affectedCell.cellPosition + " has " + correctPieces.Count + " possible pieces");

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
            Debug.Log("Now has " + nonCollapsedCells[affectedCell.cellPosition].Count + " possible pieces");
            if (nonCollapsedCells[affectedCell.cellPosition].Count > 1)
                for (int i = 0; i < nonCollapsedCells[affectedCell.cellPosition].Count; i++)
                    Debug.LogWarning("Piece :" + (i - 1).ToString() +nonCollapsedCells[affectedCell.cellPosition][i].name);
            
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
            //front 
            if (affectedCell.comingFrom != Direction.FRONT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(0, 0, grid.cellSize)))
            {
                affectedPieces.Enqueue(new CellLink(affectedCell.cellPosition + new Vector3(0, 0, grid.cellSize), Direction.BACK));
            }
            //back
            if (affectedCell.comingFrom != Direction.BACK && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(0, 0, -grid.cellSize)))
            {
                affectedPieces.Enqueue(new CellLink(affectedCell.cellPosition + new Vector3(0, 0, -grid.cellSize), Direction.FRONT));
            }
        }


        //Debug.Log("Affected Cell: " + affectedCell.cellPosition + " has " + nonCollapsedCells[affectedCell.cellPosition].Count);

    }

    Vector3 SearchForLowerEntropy(out int entropyValue)
    {
        int lowestEntropyValue = -1;
        Vector3 lowestEntropyCell = -Vector3.one * 1000;
        List<Vector3> alreadyCollapsedCells = new List<Vector3>();

        foreach (KeyValuePair<Vector3, List<PieceInfo>> cell in nonCollapsedCells)
        {
            if (lowestEntropyValue == -1)
            {
                lowestEntropyCell = cell.Key;
                lowestEntropyValue = cell.Value.Count;
            }
            else
            {
                if(cell.Value.Count == 1)
                    alreadyCollapsedCells.Add(cell.Key);
                else if(cell.Value.Count < lowestEntropyValue)
                {
                    lowestEntropyValue = cell.Value.Count;
                    lowestEntropyCell = cell.Key;
                }
            }
        }

        for (int i = 0; i < alreadyCollapsedCells.Count; i++)
        {
            //Debug.Log("Instantiating an already collapsed cell at: " + alreadyCollapsedCells[i] + " with piece: " + nonCollapsedCells[alreadyCollapsedCells[i]][0].name);
            InstantiatePiece(alreadyCollapsedCells[i], nonCollapsedCells[alreadyCollapsedCells[i]][0]);
            nonCollapsedCells.Remove(alreadyCollapsedCells[i]);
        }

        entropyValue = lowestEntropyValue;
        return lowestEntropyCell;
    }
}

