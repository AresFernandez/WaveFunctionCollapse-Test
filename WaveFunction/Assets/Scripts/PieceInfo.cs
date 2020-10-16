using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "", order = 1)]
public class PieceInfo : ScriptableObject
{
    public GameObject piecePrefab;

    [Range(0, 3)]
    public int rotation;

    public PieceInfo[] topPieces;
    public PieceInfo[] botPieces;
    public PieceInfo[] rightPieces;
    public PieceInfo[] leftPieces;
    public PieceInfo[] frontPieces;
    public PieceInfo[] backPieces;
}
