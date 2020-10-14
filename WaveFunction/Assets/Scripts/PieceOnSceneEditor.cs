using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceOnSceneEditor : MonoBehaviour
{
    public PieceInfo piece;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }
}
