using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PieceEditor : EditorWindow
{
    PieceInfo[] allPieces;

    [MenuItem("Tools/Piece Editor")]
    static void Init()
    {
        PieceEditor window = GetWindow<PieceEditor>();

        window.allPieces = Resources.FindObjectsOfTypeAll(typeof(PieceInfo)) as PieceInfo[];

        Debug.Log(window.allPieces.Length);
    }

    public Transform root;
    public PieceInfo piece;
    Editor gameObjectEditor;
    Texture2D previewBackgroundTexture;

    private void OnGUI()
    {
        root = EditorGUILayout.ObjectField("Root", root, typeof(Transform), true) as Transform;

        if(!root)
        {
            EditorGUILayout.HelpBox("Root must be assigned. Please assign a Transform.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space(20);

        EditorGUI.BeginChangeCheck();

        piece = EditorGUILayout.ObjectField("Piece", piece, typeof(PieceInfo), false) as PieceInfo;

        if (EditorGUI.EndChangeCheck())
        {
            if (gameObjectEditor != null) DestroyImmediate(gameObjectEditor);
        }

        GUIStyle bgColor = new GUIStyle();

        bgColor.normal.background = previewBackgroundTexture;

        if (piece != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(piece.piecePrefab);

            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 200), bgColor);
        }
    }
}
