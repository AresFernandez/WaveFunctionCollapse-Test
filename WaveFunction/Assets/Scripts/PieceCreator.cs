using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PieceCreator : EditorWindow
{
    GameObject piecePrefab;
    string pieceName;

    bool useCustomPath;
    string customPath;
    string path;

    [MenuItem("Tools/Piece Creator")]
    static void Init()
    {
        PieceCreator window = GetWindow<PieceCreator>();

        window.minSize = new Vector2(200, 500);

        window.customPath = window.path = "Assets/Pieces SO/NEW 3D Pieces";
    }

    private void OnGUI()
    {
        GUIStyle header = new GUIStyle("BoldLabel")
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };
        GUIStyle bold = new GUIStyle("BoldLabel") { fontSize = 15 };

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Piece Crator", header);

        EditorGUILayout.Space(20);
        piecePrefab = EditorGUILayout.ObjectField("Piece Prefab", piecePrefab, typeof(GameObject), false) as GameObject;
        pieceName = EditorGUILayout.TextField("Piece Name", pieceName);

        EditorGUILayout.Space(10);

        useCustomPath = EditorGUILayout.Toggle("Use custom path", useCustomPath);
        if(useCustomPath)
            customPath = EditorGUILayout.TextField("Path", customPath);

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Create 3D Piece"))
            Create3DPiece();
        if (GUILayout.Button("Create Single Piece"))
            CreateSinglePiece();
    }

    private void Create3DPiece()
    {
        if(!piecePrefab || pieceName == "")
        {
            Debug.LogError("Some fields are not completed");
            return;
        }

        for(int i = 0; i <= 3; i++)
            CreateSinglePiece(i);
    }

    private void CreateSinglePiece(int _rotation = 0)
    {
        PieceInfo newPiece = ScriptableObjectUtility.CreateAsset<PieceInfo>(pieceName + " " + _rotation.ToString(), useCustomPath ? customPath : path);
        newPiece.name = pieceName + " " + _rotation.ToString();

        newPiece.piecePrefab = piecePrefab;
        newPiece.rotation = _rotation;
    }
}
