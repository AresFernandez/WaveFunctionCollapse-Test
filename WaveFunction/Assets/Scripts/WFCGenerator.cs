using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public struct Cell
{
    public Vector3 position;
    public PieceInfo piece;

    public Cell(Vector3 _position, PieceInfo _piece)
    {
        position = _position;
        piece = _piece;
    }
}

public class WFCGenerator : MonoBehaviour
{
    public Grid grid;

    [SerializeField] TileCollection tileCollection;

    Dictionary<Vector3, List<PieceInfo>> nonCollapsedCells;
    List<Cell> cells;

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

        cells = new List<Cell>();

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

            if(entropyValue != -1)
                SelectPiece(lowestEntropyCell);
        }

        if (cells.Count != grid.CalculateGridSize())
            Debug.LogError("ERROR ON GENERATION");
        else
        {
            foreach (Cell cell in cells)
                InstantiatePiece(cell);
        }
    }

     void OnDrawGizmos()
     {
        //Handles.color = Color.white;
        //if(nonCollapsedCells != null && nonCollapsedCells.Count > 0)
        //    foreach(KeyValuePair<Vector3, List<PieceInfo>> cell in nonCollapsedCells)
        //        Handles.Label(cell.Key, cell.Key + " / " + cell.Value.Count);
     }

    void SelectPiece(Vector3 cellPosition)
    {
        PieceInfo selectedPiece = nonCollapsedCells[cellPosition][Random.Range(0, nonCollapsedCells[cellPosition].Count)];
        nonCollapsedCells[cellPosition].Clear();
        nonCollapsedCells[cellPosition].Add(selectedPiece);

        cells.Add(new Cell(cellPosition, selectedPiece));

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
        while (affectedPieces.Count > 0)
        {
            //safeCounter++;
            //if (safeCounter >= 1000)
            //{
            //    Debug.LogError("MAX ITERATIONS");
            //    break;
            //}
            Propagate(affectedPieces.Dequeue());
        }

        affectedPieces.Clear();

        nonCollapsedCells.Remove(cellPosition);
    }

    void InstantiatePiece(Cell _cell)
    {
        GameObject newPiece = Instantiate<GameObject>(_cell.piece.piecePrefab, _cell.position, _cell.piece.piecePrefab.transform.rotation, transform);
        newPiece.transform.position -= new Vector3(0, grid.cellSize / 2.0f, 0);
        newPiece.transform.eulerAngles += new Vector3(0, 0, 90) * _cell.piece.rotation;
    }

    void Propagate(CellLink affectedCell)
    {
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
            if(nonCollapsedCells[alreadyCollapsedCells[i]][0] != null)
                cells.Add(new Cell(alreadyCollapsedCells[i], nonCollapsedCells[alreadyCollapsedCells[i]][0]));
            nonCollapsedCells.Remove(alreadyCollapsedCells[i]);
        }

        entropyValue = lowestEntropyValue;
        return lowestEntropyCell;
    }
}

