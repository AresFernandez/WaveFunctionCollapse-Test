using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PiecesSideOnSceneEditor : MonoBehaviour
{
    public enum Side { Forward, Backward, Right, Left, Up, Down }
    public Side side;

    public PieceOnSceneEditor pieceOnSceneEditorScript;

    public string MoveToSide(Side _side)
    {
        switch(_side)
        {
            case Side.Forward:
                transform.localPosition = Vector3.forward;
                return "Forward";
            case Side.Backward:
                transform.localPosition = -Vector3.forward;
                return "Backward";
            case Side.Right:
                transform.localPosition = Vector3.right;
                return "Right";
            case Side.Left:
                transform.localPosition = -Vector3.right;
                return "Left";
            case Side.Up:
                transform.localPosition = Vector3.up;
                return "Up";
            case Side.Down:
                transform.localPosition = -Vector3.up;
                return "Down";
            default:
                return "";
        }
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(PiecesSideOnSceneEditor pieceSide, GizmoType gizmoType)
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(pieceSide.transform.position, 0.12f);

        if ((gizmoType & GizmoType.Selected) != 0)
            AddPiece();
        else
            Gizmos.color = Color.white * 0.5f;

        Gizmos.DrawSphere(pieceSide.transform.position, 0.1f);
    }

    private static void AddPiece()
    {
        Debug.Log("Add piece");
        Selection.activeGameObject = null;
    }
}
