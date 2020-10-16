using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Piece Database", menuName = "", order = 1)]
public class PieceDatabase : ScriptableObject
{
    public PieceInfo[] allPieces;
}
