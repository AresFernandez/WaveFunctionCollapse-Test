using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "", order = 1)]
public class PieceInfo : ScriptableObject
{
    public GameObject GOprefab;
    public PieceInfo[] topPieces;
    public PieceInfo[] botPieces;
    public PieceInfo[] rightPieces;
    public PieceInfo[] leftPieces;
}
