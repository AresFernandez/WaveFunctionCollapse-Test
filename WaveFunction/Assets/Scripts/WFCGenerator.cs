using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;



public abstract class WFCGenerator : MonoBehaviour
{
    public Grid grid;

    [SerializeField] TileCollection tileCollection;

    protected Dictionary<Vector3, List<PieceInfo>> nonCollapsedCells;
    protected Dictionary<Vector3, PieceInfo> cells;

    public bool useSeed;
    public int seed;

    //public Vector3[] piecesToSpawnPos;

    enum Direction { TOP, BOT, RIGHT, LEFT, FRONT, BACK };

    struct CellLink
    {
        public Vector3 cellPosition;
        public Direction comingFrom;
    }

    //Queue  <  cellPosition , comingFrom  >
    Queue<CellLink> affectedPieces = new Queue<CellLink>();

    // Start is called before the first frame update
    void Start()
    {
        if (!useSeed)
            seed = Random.Range(0, 1000);
        Random.InitState(seed);

        nonCollapsedCells = new Dictionary<Vector3, List<PieceInfo>>();
        Vector3[] cellPositions = grid.Calculate3DGridPositions();

        cells = new Dictionary<Vector3, PieceInfo>();

        List<PieceInfo> allPieces = new List<PieceInfo>();
        for (int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            allPieces.Add(tileCollection.piecesOnCollection[i].piece);

        for (int i = 0; i < cellPositions.Length; i++)
        {
            nonCollapsedCells.Add(cellPositions[i], new List<PieceInfo>(allPieces));
        }

        // We select the first cell randomly because at the begginning they have all the same entropy
        Vector3 startPosition = SelectFirstCell();
        SelectPiece(startPosition);

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
            foreach (KeyValuePair<Vector3, PieceInfo> cell in cells)
                InstantiatePiece(cell.Key, cell.Value);
        }
    }

    virtual protected Vector3 SelectFirstCell()
    {
        return grid.GetRandom3DPosition();
    }

    void SelectPiece(Vector3 cellPosition)
    {
        PieceInfo selectedPiece = SelectPiece(cellPosition, nonCollapsedCells[cellPosition].ToArray()); 
        nonCollapsedCells[cellPosition].Clear();
        nonCollapsedCells[cellPosition].Add(selectedPiece);

        cells.Add(cellPosition, selectedPiece);

        //Add to Queue of affected pieces
        //top
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, grid.cellSize, 0)))
        {
            affectedPieces.Enqueue(new CellLink() { cellPosition = cellPosition + new Vector3(0, grid.cellSize, 0), comingFrom = Direction.BOT });
        }
        //bot
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, -grid.cellSize, 0)))
        {
            affectedPieces.Enqueue(new CellLink() { cellPosition = cellPosition + new Vector3(0, -grid.cellSize, 0), comingFrom = Direction.TOP });
        }
        //right
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(grid.cellSize, 0, 0)))
        {
            affectedPieces.Enqueue(new CellLink() { cellPosition = cellPosition + new Vector3(grid.cellSize, 0, 0), comingFrom = Direction.LEFT });
        }
        //left
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(-grid.cellSize, 0, 0)))
        {
            affectedPieces.Enqueue(new CellLink() { cellPosition = cellPosition + new Vector3(-grid.cellSize, 0, 0), comingFrom = Direction.RIGHT });
        }
        //front
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, 0, grid.cellSize)))
        {
            affectedPieces.Enqueue(new CellLink() { cellPosition = cellPosition + new Vector3(0, 0, grid.cellSize), comingFrom = Direction.BACK });
        }
        //back
        if (nonCollapsedCells.ContainsKey(cellPosition + new Vector3(0, 0, -grid.cellSize)))
        {
            affectedPieces.Enqueue(new CellLink() { cellPosition = cellPosition + new Vector3(0, 0, -grid.cellSize), comingFrom = Direction.FRONT });
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

    virtual protected PieceInfo SelectPiece(Vector3 _position, PieceInfo[] _pieces)
    {
        return _pieces[Random.Range(0, _pieces.Length)];
    }

    void InstantiatePiece(Vector3 _position, PieceInfo _piece)
    {
        if (_piece.piecePrefab.name == "Empty")
            return;

        GameObject newPiece = Instantiate<GameObject>(_piece.piecePrefab, _position, _piece.piecePrefab.transform.rotation, transform);
        newPiece.transform.position -= new Vector3(0, grid.cellSize / 2.0f, 0);
        newPiece.transform.eulerAngles += new Vector3(0, 0, 90) * _piece.rotation;
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
                affectedPieces.Enqueue(new CellLink() { cellPosition = affectedCell.cellPosition + new Vector3(0, grid.cellSize, 0), comingFrom = Direction.BOT });
            }
            //bot
            if (affectedCell.comingFrom != Direction.BOT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(0, -grid.cellSize, 0)))
            {
                affectedPieces.Enqueue(new CellLink() { cellPosition = affectedCell.cellPosition + new Vector3(0, -grid.cellSize, 0), comingFrom = Direction.TOP });
            }
            //right
            if (affectedCell.comingFrom != Direction.RIGHT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(grid.cellSize, 0, 0)))
            {
                affectedPieces.Enqueue(new CellLink() { cellPosition = affectedCell.cellPosition + new Vector3(grid.cellSize, 0, 0), comingFrom = Direction.LEFT });
            }
            //left
            if (affectedCell.comingFrom != Direction.LEFT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(-grid.cellSize, 0, 0)))
            {
                affectedPieces.Enqueue(new CellLink() { cellPosition = affectedCell.cellPosition + new Vector3(-grid.cellSize, 0, 0), comingFrom = Direction.RIGHT });
            }
            //front 
            if (affectedCell.comingFrom != Direction.FRONT && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(0, 0, grid.cellSize)))
            {
                affectedPieces.Enqueue(new CellLink() { cellPosition = affectedCell.cellPosition + new Vector3(0, 0, grid.cellSize), comingFrom = Direction.BACK });
            }
            //back
            if (affectedCell.comingFrom != Direction.BACK && nonCollapsedCells.ContainsKey(affectedCell.cellPosition + new Vector3(0, 0, -grid.cellSize)))
            {
                affectedPieces.Enqueue(new CellLink() { cellPosition = affectedCell.cellPosition + new Vector3(0, 0, -grid.cellSize), comingFrom = Direction.FRONT });
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
            if (nonCollapsedCells[alreadyCollapsedCells[i]][0] != null)
                cells.Add(alreadyCollapsedCells[i], nonCollapsedCells[alreadyCollapsedCells[i]][0]);
            nonCollapsedCells.Remove(alreadyCollapsedCells[i]);
        }

        entropyValue = lowestEntropyValue;
        return lowestEntropyCell;
    }

    void OnDrawGizmos()
    {
        //Handles.color = Color.white;
        //if(nonCollapsedCells != null && nonCollapsedCells.Count > 0)
        //    foreach(KeyValuePair<Vector3, List<PieceInfo>> cell in nonCollapsedCells)
        //        Handles.Label(cell.Key, cell.Key + " / " + cell.Value.Count);
    }
}

