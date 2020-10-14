using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PieceEditor : EditorWindow
{
    Transform root;

    PieceInfo[] allPieces;
    int currentPieceIndex;

    Editor pieceEditor;


    GameObject instantiatedPreviewPiece;
    float numberOfPiecesForRow = 3;

   [MenuItem("Tools/Piece Editor")]
    static void Init()
    {
        PieceEditor window = GetWindow<PieceEditor>();

        window.minSize = new Vector2(200, 500);

        window.allPieces = Resources.FindObjectsOfTypeAll(typeof(PieceInfo)) as PieceInfo[];
        if (window.allPieces.Length > 0)
        {
            window.currentPieceIndex = 0;
            window.pieceEditor = Editor.CreateEditor(window.allPieces[window.currentPieceIndex].piecePrefab);
        }
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

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Piece Editor", header);

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Current Pìece: " + allPieces[currentPieceIndex].name, bold);

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

        if (GUILayout.Button("Focus on selected Piece"))
            if (instantiatedPreviewPiece != null)
                ShowCurrentPiece();
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

            List<PieceInfo> otherPieces = new List<PieceInfo>();
            for (int i = 0; i < allPieces.Length; i++)
                if (i != currentPieceIndex)
                    otherPieces.Add(allPieces[i]);

            float numberOfRows = Mathf.Ceil(otherPieces.Count / numberOfPiecesForRow);

            for (int i = 0; i < numberOfRows; i++)
                for (int j = 0; j < numberOfPiecesForRow; j++)
                    SpawnPiece(otherPieces[i * (int)numberOfRows + j], new Vector3(root.position.x + 4 * j + 4, root.position.y, root.position.z - 4 * i), true);
        }
    }

    private GameObject SpawnPiece(PieceInfo _piece, Vector3 _position, bool _spawnSides)
    {
        GameObject spawnedPiece = Instantiate(_piece.piecePrefab, _position, Quaternion.identity, root);
        PieceOnSceneEditor pieceOnSceneEditorScript = spawnedPiece.AddComponent<PieceOnSceneEditor>();
        pieceOnSceneEditorScript.piece = _piece;

        if (!_spawnSides)
            return spawnedPiece;

        SpawnPieceSide(spawnedPiece, PiecesSideOnSceneEditor.Side.Forward);
        SpawnPieceSide(spawnedPiece, PiecesSideOnSceneEditor.Side.Backward);
        SpawnPieceSide(spawnedPiece, PiecesSideOnSceneEditor.Side.Right);
        SpawnPieceSide(spawnedPiece, PiecesSideOnSceneEditor.Side.Left);
        SpawnPieceSide(spawnedPiece, PiecesSideOnSceneEditor.Side.Up);
        SpawnPieceSide(spawnedPiece, PiecesSideOnSceneEditor.Side.Down);

        return spawnedPiece;
    }

    private void SpawnPieceSide(GameObject _parent, PiecesSideOnSceneEditor.Side _side)
    {
        GameObject pieceSide = new GameObject();
        pieceSide.transform.parent = _parent.transform;

        PiecesSideOnSceneEditor pieceSideOnSceneEditorScript = pieceSide.AddComponent<PiecesSideOnSceneEditor>();
        pieceSideOnSceneEditorScript.pieceOnSceneEditorScript = _parent.GetComponent<PieceOnSceneEditor>();
        pieceSide.name = pieceSideOnSceneEditorScript.MoveToSide(_side);
    }
}
