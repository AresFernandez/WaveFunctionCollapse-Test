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
    public bool selected;

    public PieceInfo piece;
    public bool spawned;

    public string MoveToSide(Side _side)
    {
        side = _side;

        transform.position = transform.parent.position + new Vector3(0, 0.5f, 0);

        switch (side)
        {
            case Side.Forward:
                transform.position += Vector3.forward;
                return "Forward";
            case Side.Backward:
                transform.position += -Vector3.forward;
                return "Backward";
            case Side.Right:
                transform.position += Vector3.right;
                return "Right";
            case Side.Left:
                transform.position += -Vector3.right;
                return "Left";
            case Side.Up:
                transform.position += Vector3.up;
                return "Up";
            case Side.Down:
                transform.position += -Vector3.up;
                return "Down";
            default:
                return "";
        }
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(PiecesSideOnSceneEditor pieceSide, GizmoType gizmoType)
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(pieceSide.transform.position, 0.24f);

        if ((gizmoType & GizmoType.Selected) != 0 && !pieceSide.selected)
        {
            pieceSide.selected = true;
            Selection.activeGameObject = null;

            List<PieceInfo> piecesLeft;

            if (pieceSide.spawned)
            {
                switch(pieceSide.side)
                {
                    case Side.Forward:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.backPieces);
                        piecesLeft.Remove(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.backPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.frontPieces);
                        piecesLeft.Remove(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.frontPieces = piecesLeft.ToArray();
                        break;
                    case Side.Backward:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.frontPieces);
                        piecesLeft.Remove(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.frontPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.backPieces);
                        piecesLeft.Remove(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.backPieces = piecesLeft.ToArray();
                        break;
                    case Side.Right:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.leftPieces);
                        piecesLeft.Remove(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.leftPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.rightPieces);
                        piecesLeft.Remove(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.rightPieces = piecesLeft.ToArray();

                        break;
                    case Side.Left:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.rightPieces);
                        piecesLeft.Remove(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.rightPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.leftPieces);
                        piecesLeft.Remove(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.leftPieces = piecesLeft.ToArray();

                        break;
                    case Side.Up:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.botPieces);
                        piecesLeft.Remove(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.botPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.topPieces);
                        piecesLeft.Remove(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.topPieces = piecesLeft.ToArray();

                        break;
                    case Side.Down:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.topPieces);
                        piecesLeft.Remove(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.topPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.botPieces);
                        piecesLeft.Remove(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.botPieces = piecesLeft.ToArray();

                        break;
                    default:
                        break;
                }

                EditorUtility.SetDirty(pieceSide.piece);
                EditorUtility.SetDirty(pieceSide.pieceOnSceneEditorScript.piece);

                if(pieceSide.transform.childCount > 0)
                    DestroyImmediate(pieceSide.transform.GetChild(0).gameObject);
                pieceSide.spawned = false;
            }
            else
            {
                switch (pieceSide.side)
                {
                    case Side.Forward:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.backPieces);
                        piecesLeft.Add(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.backPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.frontPieces);
                        piecesLeft.Add(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.frontPieces = piecesLeft.ToArray();
                        break;
                    case Side.Backward:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.frontPieces);
                        piecesLeft.Add(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.frontPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.backPieces);
                        piecesLeft.Add(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.backPieces = piecesLeft.ToArray();
                        break;
                    case Side.Right:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.leftPieces);
                        piecesLeft.Add(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.leftPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.rightPieces);
                        piecesLeft.Add(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.rightPieces = piecesLeft.ToArray();
                        break;
                    case Side.Left:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.rightPieces);
                        piecesLeft.Add(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.rightPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.leftPieces);
                        piecesLeft.Add(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.leftPieces = piecesLeft.ToArray();
                        break;
                    case Side.Up:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.botPieces);
                        piecesLeft.Add(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.botPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.topPieces);
                        piecesLeft.Add(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.topPieces = piecesLeft.ToArray();
                        break;
                    case Side.Down:
                        piecesLeft = new List<PieceInfo>(pieceSide.piece.topPieces);
                        piecesLeft.Add(pieceSide.pieceOnSceneEditorScript.piece);
                        pieceSide.piece.topPieces = piecesLeft.ToArray();

                        piecesLeft = new List<PieceInfo>(pieceSide.pieceOnSceneEditorScript.piece.botPieces);
                        piecesLeft.Add(pieceSide.piece);
                        pieceSide.pieceOnSceneEditorScript.piece.botPieces = piecesLeft.ToArray();
                        break;
                    default:
                        break;
                }

                EditorUtility.SetDirty(pieceSide.piece);
                EditorUtility.SetDirty(pieceSide.pieceOnSceneEditorScript.piece);

                GameObject instantiatedPiece = Instantiate(pieceSide.piece.piecePrefab, pieceSide.transform.position - new Vector3(0, 0.5f, 0), pieceSide.piece.piecePrefab.transform.rotation, pieceSide.transform);
                instantiatedPiece.transform.eulerAngles += new Vector3(0, 0, 90) * pieceSide.piece.rotation;
                pieceSide.spawned = true;
            }

            if (pieceSide.piece == pieceSide.pieceOnSceneEditorScript.piece)
            {
                PieceEditor.RefreshWindow();
                return;
            }
        }
        else if((gizmoType & GizmoType.NonSelected) != 0 && pieceSide.selected)
        {
            pieceSide.selected = false;
        }

        switch (pieceSide.side)
        {
            case Side.Forward:
                Gizmos.color = Color.blue;
                break;
            case Side.Backward:
                Gizmos.color = new Color(0.5f, 0.5f, 1, 1);
                break;
            case Side.Right:
                Gizmos.color = Color.red;
                break;
            case Side.Left:
                Gizmos.color = new Color(1, 0.5f, 0.5f, 1);
                break;
            case Side.Up:
                Gizmos.color = Color.green;
                break;
            case Side.Down:
                Gizmos.color = new Color(0.5f, 1, 0.5f, 1);
                break;
            default:
                break;
        }

        Gizmos.DrawSphere(pieceSide.transform.position, 0.20f);
    }

    private void OnDrawGizmos()
    {
        if(spawned)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        }
    }
}
