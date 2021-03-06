﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class PieceEditor : EditorWindow
{
    public static PieceEditor Instance
    {
        get { return GetWindow<PieceEditor>(); }
    }
    Transform root;

    PieceInfo[] allPieces;
    int currentPieceIndex;

    bool showPieces = true;

    Editor pieceEditor;
    PieceDatabase pieceDatabase;

    GameObject instantiatedPreviewPiece;
    float numberOfPiecesForRow = 4;

   [MenuItem("Tools/Piece Editor")]
    static void Init()
    {
        PieceEditor window = GetWindow<PieceEditor>();

        window.minSize = new Vector2(200, 500);
    }

    private void OnGUI()
    {
        GUIStyle header = new GUIStyle("BoldLabel")
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };
        GUIStyle bold = new GUIStyle("BoldLabel") { fontSize = 15 };

        if (root == null)
            CreateRoot();

        EditorGUI.BeginChangeCheck();

        if(!pieceDatabase)
        {
            pieceDatabase = EditorGUILayout.ObjectField("Piece Database", pieceDatabase, typeof(PieceDatabase), true) as PieceDatabase;

            if(!EditorGUI.EndChangeCheck())
                return;
        }

        if(allPieces == null)
        {
            RefreshAll();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Piece Editor", header);

        EditorGUILayout.Space(20);
        EditorGUI.BeginChangeCheck();
        showPieces = EditorGUILayout.Toggle("Show Pieces", showPieces);
        if (EditorGUI.EndChangeCheck())
            ShowPieces(showPieces);

        if (!showPieces)
            return;

        string[] allPiecesNames = new string[allPieces.Length];
        for (int i = 0; i < allPiecesNames.Length; i++)
            allPiecesNames[i] = allPieces[i].name;

        EditorGUI.BeginChangeCheck();
        currentPieceIndex = EditorGUILayout.Popup("Current Pìece: ", currentPieceIndex, allPiecesNames, bold);
        if(EditorGUI.EndChangeCheck())
        {
            pieceEditor = Editor.CreateEditor(allPieces[currentPieceIndex].piecePrefab);

            DeleteAllSpawnedPieces();
            SpawnPieces();
        }

        //EditorGUILayout.LabelField("Current Pìece: " + allPieces[currentPieceIndex].name, bold);

        EditorGUILayout.Space(15);

        if(GUILayout.Button("Force refresh"))
            RefreshAll();

        EditorGUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();
        if(pieceEditor == null)
        {
            pieceEditor = Editor.CreateEditor(allPieces[currentPieceIndex].piecePrefab);
            SpawnPieces();
        }

        float height = Screen.width - (Screen.width/10.0f) * 3;

        if (GUILayout.Button("<", GUILayout.Width(Screen.width / 10.0f), GUILayout.Height(height)))
            PreviousPiece();

        pieceEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(height, height), new GUIStyle());

        if (GUILayout.Button(">", GUILayout.Width(Screen.width / 10.0f), GUILayout.Height(height)))
            NextPiece();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(15);

        numberOfPiecesForRow = (int)Mathf.Abs(EditorGUILayout.IntSlider("Number of Pieces for Row", (int)numberOfPiecesForRow, 3, 10));

        if (GUILayout.Button("Focus on selected Piece"))
            if (instantiatedPreviewPiece != null)
                ShowCurrentPiece();
    }

    private void OnDestroy()
    {
        DestroyImmediate(root.gameObject);
    }

    private void CreateRoot()
    {
        GameObject pieceEditorRoot = GameObject.Find("PieceEditor");
        if (pieceEditorRoot == null)
            pieceEditorRoot = new GameObject("PieceEditor");

        root = pieceEditorRoot.transform;
    }

    private void PreviousPiece()
    {
        currentPieceIndex--;
        if (currentPieceIndex < 0)
            currentPieceIndex = allPieces.Length - 1;

        pieceEditor = Editor.CreateEditor(allPieces[currentPieceIndex].piecePrefab);

        DeleteAllSpawnedPieces();
        SpawnPieces();
    }

    private void NextPiece()
    {
        currentPieceIndex++;
        if (currentPieceIndex >= allPieces.Length)
            currentPieceIndex = 0;

        pieceEditor = Editor.CreateEditor(allPieces[currentPieceIndex].piecePrefab);

        DeleteAllSpawnedPieces();
        SpawnPieces();
    }

    private void ShowCurrentPiece()
    {
        Selection.activeGameObject = instantiatedPreviewPiece;
        SceneView.FrameLastActiveSceneView();
    }

    private void DeleteAllSpawnedPieces()
    {
        DestroyImmediate(root.gameObject);
        CreateRoot();
    }

    private void SpawnPieces()
    {
        if(root != null)
        {
            instantiatedPreviewPiece = SpawnPiece(allPieces[currentPieceIndex], root.position, false);

            float numberOfRows = Mathf.Ceil(allPieces.Length / numberOfPiecesForRow);

            int pieceNumber = 0;
            for (int i = 0; i < numberOfRows; i++)
                for (int j = 0; j < numberOfPiecesForRow; j++)
                    if(pieceNumber < allPieces.Length)
                    {
                        SpawnPiece(allPieces[pieceNumber], new Vector3(root.position.x + 4 * j + 4, root.position.y, root.position.z - 4 * i), true);
                        pieceNumber++;
                    }
        }
    }

    private GameObject SpawnPiece(PieceInfo _piece, Vector3 _position, bool _spawnSides)
    {
        GameObject spawnedPiece = Instantiate(_piece.piecePrefab, _position, _piece.piecePrefab.transform.rotation, root);
        spawnedPiece.transform.eulerAngles += new Vector3(0, 0, 90) * _piece.rotation; 
        PieceOnSceneEditor pieceOnSceneEditorScript = spawnedPiece.AddComponent<PieceOnSceneEditor>();
        pieceOnSceneEditorScript.piece = _piece;

        if (!_spawnSides)
            return spawnedPiece;

        SpawnPieceSide(spawnedPiece, _piece, PiecesSideOnSceneEditor.Side.Forward);
        SpawnPieceSide(spawnedPiece, _piece, PiecesSideOnSceneEditor.Side.Backward);
        SpawnPieceSide(spawnedPiece, _piece, PiecesSideOnSceneEditor.Side.Right);
        SpawnPieceSide(spawnedPiece, _piece, PiecesSideOnSceneEditor.Side.Left);
        SpawnPieceSide(spawnedPiece, _piece, PiecesSideOnSceneEditor.Side.Up);
        SpawnPieceSide(spawnedPiece, _piece, PiecesSideOnSceneEditor.Side.Down);

        return spawnedPiece;
    }

    private void SpawnPieceSide(GameObject _parent, PieceInfo _currentPiece, PiecesSideOnSceneEditor.Side _side)
    {
        GameObject pieceSide = new GameObject();
        pieceSide.transform.parent = _parent.transform;

        PiecesSideOnSceneEditor pieceSideOnSceneEditorScript = pieceSide.AddComponent<PiecesSideOnSceneEditor>();
        pieceSideOnSceneEditorScript.pieceOnSceneEditorScript = _parent.GetComponent<PieceOnSceneEditor>();
        pieceSide.name = pieceSideOnSceneEditorScript.MoveToSide(_side);
        pieceSideOnSceneEditorScript.piece = allPieces[currentPieceIndex];

        GameObject instantiatedPiece = null;

        switch (_side)
        {
            case PiecesSideOnSceneEditor.Side.Forward:
                if (_currentPiece.frontPieces.Contains<PieceInfo>(allPieces[currentPieceIndex]))
                {
                    instantiatedPiece = Instantiate(allPieces[currentPieceIndex].piecePrefab, pieceSide.transform.position, allPieces[currentPieceIndex].piecePrefab.transform.rotation, pieceSide.transform);
                    pieceSideOnSceneEditorScript.spawned = true;
                }
                break;

            case PiecesSideOnSceneEditor.Side.Backward:
                if (_currentPiece.backPieces.Contains<PieceInfo>(allPieces[currentPieceIndex]))
                {
                    instantiatedPiece = Instantiate(allPieces[currentPieceIndex].piecePrefab, pieceSide.transform.position, allPieces[currentPieceIndex].piecePrefab.transform.rotation, pieceSide.transform);
                    pieceSideOnSceneEditorScript.spawned = true;
                }
                break;

            case PiecesSideOnSceneEditor.Side.Right:
                if(_currentPiece.rightPieces.Contains<PieceInfo>(allPieces[currentPieceIndex]))
                {
                    instantiatedPiece = Instantiate(allPieces[currentPieceIndex].piecePrefab, pieceSide.transform.position, allPieces[currentPieceIndex].piecePrefab.transform.rotation, pieceSide.transform);
                    pieceSideOnSceneEditorScript.spawned = true;
                }
                break;

            case PiecesSideOnSceneEditor.Side.Left:
                if (_currentPiece.leftPieces.Contains<PieceInfo>(allPieces[currentPieceIndex]))
                {
                    instantiatedPiece = Instantiate(allPieces[currentPieceIndex].piecePrefab, pieceSide.transform.position, allPieces[currentPieceIndex].piecePrefab.transform.rotation, pieceSide.transform);
                    pieceSideOnSceneEditorScript.spawned = true;
                }
                break;

            case PiecesSideOnSceneEditor.Side.Up:
                if (_currentPiece.topPieces.Contains<PieceInfo>(allPieces[currentPieceIndex]))
                {
                    instantiatedPiece = Instantiate(allPieces[currentPieceIndex].piecePrefab, pieceSide.transform.position, allPieces[currentPieceIndex].piecePrefab.transform.rotation, pieceSide.transform);
                    pieceSideOnSceneEditorScript.spawned = true;
                }
                break;

            case PiecesSideOnSceneEditor.Side.Down:
                if (_currentPiece.botPieces.Contains<PieceInfo>(allPieces[currentPieceIndex]))
                {
                    instantiatedPiece = Instantiate(allPieces[currentPieceIndex].piecePrefab, pieceSide.transform.position, allPieces[currentPieceIndex].piecePrefab.transform.rotation, pieceSide.transform);
                    pieceSideOnSceneEditorScript.spawned = true;
                }
                break;

            default:
                break;
        }

        if(instantiatedPiece)
        {
            instantiatedPiece.transform.eulerAngles += new Vector3(0, 0, 90) * allPieces[currentPieceIndex].rotation;
            instantiatedPiece.transform.position -= new Vector3(0, 0.5f, 0);
        }
    }

    private void ShowPieces(bool _show)
    {
        for (int i = 0; i < root.childCount; i++)
            root.GetChild(i).gameObject.SetActive(_show);
    }

    private void RefreshAll()
    {
        allPieces = pieceDatabase.allPieces;
        currentPieceIndex = 0;
        pieceEditor = Editor.CreateEditor(allPieces[currentPieceIndex].piecePrefab);
        DeleteAllSpawnedPieces();
        SpawnPieces();
    }

    public void RefreshWindow()
    {
        DeleteAllSpawnedPieces();
        SpawnPieces();
    }
}
