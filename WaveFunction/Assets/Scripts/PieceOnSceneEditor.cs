using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceOnSceneEditor : MonoBehaviour
{
    public PieceInfo piece;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector3(1, 1, 1));
    }
}
