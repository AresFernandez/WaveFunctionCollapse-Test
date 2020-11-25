using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileCollection", menuName = "", order = 1)]
public class TileCollection : ScriptableObject
{
    public PieceInfo[] piecesOnCollection;
}

[System.Serializable]
public class PieceInCollection
{
    public PieceInfo piece;
    public float weight;

    public PieceInCollection(PieceInfo _piece, float _weight = 1)
    {
        piece = _piece;
        weight = _weight;
    }
}
