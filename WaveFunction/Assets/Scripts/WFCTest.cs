using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WFCTest : WFCGenerator
{
    public PieceInfo ground;
    public PieceInfo sky;

    protected override Vector3 SelectFirstCell()
    {

        return Vector3.zero;
    }

    protected override PieceInfo SelectPiece(Vector3 _position, PieceInfo[] _pieces)
    {
        if (_position.y == grid.height - 1 && _pieces.Contains<PieceInfo>(sky))
            return sky;

        if (_pieces.Contains<PieceInfo>(ground))
        {
            //right
            if (cells.ContainsKey(_position + Vector3.right) && cells[_position + Vector3.right].piecePrefab.name.Contains("Wall"))
                return ground;
            //left
            if (cells.ContainsKey(_position - Vector3.right) && cells[_position - Vector3.right].piecePrefab.name.Contains("Wall"))
                return ground;
            //front
            if (cells.ContainsKey(_position + Vector3.forward) && cells[_position + Vector3.forward].piecePrefab.name.Contains("Wall"))
                return ground;
            //back
            if (cells.ContainsKey(_position - Vector3.forward) && cells[_position - Vector3.forward].piecePrefab.name.Contains("Wall"))
                return ground;
        }

        return _pieces[Random.Range(0, _pieces.Length)];
    }
}
