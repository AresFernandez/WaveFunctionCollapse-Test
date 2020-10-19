using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PieceOnSceneEditor : MonoBehaviour
{
    public PieceInfo piece;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.65f, 0, 1);
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector3(1, 1, 1));

        if(piece.piecePrefab.name == "Empty")
            Handles.Label(transform.position + new Vector3(0, -1, 0), piece.name);
    }
}
