using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileCollection))]
public class TileCollectionEditor : Editor
{
    TileCollection tileCollection;
    bool[] selectedPieces;
    Vector2 scrollPos;
    int numberOfSelectedPieces;

    void OnEnable()
    {
        tileCollection = (TileCollection)target;

        UpdateSelectedPieces();
    }

    private void UpdateSelectedPieces()
    {
        selectedPieces = new bool[tileCollection.piecesOnCollection.Length];
        for (int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            selectedPieces[i] = false;

        numberOfSelectedPieces = 0;
    }

    public override void OnInspectorGUI()
    {
        GUIStyle header = new GUIStyle("BoldLabel")
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };
        GUIStyle bold = new GUIStyle("BoldLabel") { fontSize = 15 };

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tile Collection", header);

        EditorGUILayout.Space(20);
        numberOfSelectedPieces = 0;
        for (int i = 0; i < selectedPieces.Length; i++)
            if (selectedPieces[i])
                numberOfSelectedPieces++;

        EditorGUILayout.LabelField("Pieces   (" + tileCollection.piecesOnCollection.Length.ToString() + ")" + "  -  Selected Pieces   (" + numberOfSelectedPieces.ToString() + ")", bold);

        EditorGUILayout.Space(20);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width * 0.95f), GUILayout.Height(600));
        EditorGUILayout.BeginVertical("box");
        for(int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            DrawPiece(i, bold);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("+"))
            AddPiecesButton();
        if(numberOfSelectedPieces > 0)
            if (GUILayout.Button("-"))
                RemovePiecesButton();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPiece(int _position, GUIStyle _boldStyle)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(tileCollection.piecesOnCollection[_position].GOprefab.name, _boldStyle);

        EditorGUILayout.Space(10);

        selectedPieces[_position] = EditorGUILayout.Toggle("Select", selectedPieces[_position]);

        EditorGUILayout.Space(5);

        if(GUILayout.Button("Go to Piece"))
            EditorGUIUtility.PingObject(tileCollection.piecesOnCollection[_position]);
        if (GUILayout.Button("Go to Prefab"))
            EditorGUIUtility.PingObject(tileCollection.piecesOnCollection[_position].GOprefab);

        EditorGUILayout.EndVertical();

        GUILayout.Box(AssetPreview.GetAssetPreview(tileCollection.piecesOnCollection[_position].GOprefab), GUILayout.Height(100), GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
    }

    private void AddPiecesButton()
    {
        TileCollectionEditorWindow.Open(tileCollection, this);
    }

    private void RemovePiecesButton()
    {
        if (AnyPieceSelected())
        {
            string piecesSelectedNames = "";

            for (int i = 0; i < selectedPieces.Length; i++)
                if (selectedPieces[i])
                    piecesSelectedNames += "Piece: " + tileCollection.piecesOnCollection[i].name + "\n";

            if (EditorUtility.DisplayDialog("Remove all this pieces?", piecesSelectedNames, "Yes", "Cancel"))
                RemoveSelectedPieces();
        }
    }

    private void RemoveSelectedPieces()
    {
        List<PieceInfo> piecesToKeep = new List<PieceInfo>();
        for(int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
        {
            if (!selectedPieces[i])
                piecesToKeep.Add(tileCollection.piecesOnCollection[i]);
        }

        tileCollection.piecesOnCollection = new PieceInfo[piecesToKeep.Count];
        for (int i = 0; i < piecesToKeep.Count; i++)
            tileCollection.piecesOnCollection[i] = piecesToKeep[i];

        UpdateSelectedPieces();
    }

    private bool AnyPieceSelected()
    {
        bool oneSelected = false;
        for(int i = 0; i < selectedPieces.Length; i++)
        {
            if(selectedPieces[i])
            {
                oneSelected = true;
                break;
            }
        }

        return oneSelected;
    }

    public void AddPieces(List<PieceInfo> _piecesToAdd)
    {
        PieceInfo[] finalPieces = new PieceInfo[tileCollection.piecesOnCollection.Length + _piecesToAdd.Count];

        for (int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            finalPieces[i] = tileCollection.piecesOnCollection[i];
        for (int i = 0; i < _piecesToAdd.Count; i++)
            finalPieces[i + tileCollection.piecesOnCollection.Length] = _piecesToAdd[i];

        tileCollection.piecesOnCollection = finalPieces;

        UpdateSelectedPieces();
    }
}


public class TileCollectionEditorWindow : EditorWindow
{
    TileCollection tileCollection;
    TileCollectionEditor tileCollectionEditor;

    PieceInfo[] allPieces;
    List<PieceInfo> availablePieces;
    bool[] selectedPieces;
    int numberOfSelectedPieces;

    Vector2 scrollPos;
    int width;

    public static void Open(TileCollection _tileCollection, TileCollectionEditor _tileCollectionEditor)
    {
        TileCollectionEditorWindow window = GetWindow<TileCollectionEditorWindow>("Add new pieces");
        window.tileCollection = _tileCollection;
        window.tileCollectionEditor = _tileCollectionEditor;

        window.allPieces = Resources.FindObjectsOfTypeAll(typeof(PieceInfo)) as PieceInfo[];
        window.availablePieces = new List<PieceInfo>();

        for (int i = 0; i < window.allPieces.Length; i++)
        {
            if (!window.tileCollection.piecesOnCollection.Contains<PieceInfo>(window.allPieces[i]))
                window.availablePieces.Add(window.allPieces[i]);
        }

        window.selectedPieces = new bool[window.availablePieces.Count];
        window.numberOfSelectedPieces = 0;
        window.width = 200;
        window.minSize = new Vector2(window.width, window.width);
    }

    private void OnGUI()
    {
        int width = 200;

        GUIStyle header = new GUIStyle("BoldLabel")
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };
        GUIStyle bold = new GUIStyle("BoldLabel") { fontSize = 15 };

        GUILayout.Label("Add pieces", header);
        GUILayout.Space(10);

        numberOfSelectedPieces = 0;
        for (int i = 0; i < selectedPieces.Length; i++)
            if (selectedPieces[i])
                numberOfSelectedPieces++;

        GUILayout.Label("Available Pieces   (" + availablePieces.Count.ToString() + ")  -  Selected Pieces   (" + numberOfSelectedPieces.ToString() + ")", bold);
        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width * 0.95f), GUILayout.Height(Screen.height * 0.8f));
        GUILayout.BeginVertical();

        int currentPiece = 0;
        while (currentPiece < availablePieces.Count)
        {
            GUILayout.BeginHorizontal();
            for (int i = width; i < (Screen.width - width) * 0.95f; i += width)
            {
                if (currentPiece < availablePieces.Count)
                {
                    DrawPiece(currentPiece, width);
                    currentPiece++;
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Selected Pieces"))
            AddSelectedPiecesToCollection();
        if (GUILayout.Button("Cancel Selection"))
            CancelSelection();
        GUILayout.EndHorizontal();
    }

    private void DrawPiece(int _position, int _width)
    {
        GUILayout.BeginHorizontal("box", GUILayout.Height(_width * 0.4f), GUILayout.Width(_width));
        GUILayout.BeginVertical();

        GUILayout.Label(availablePieces[_position].name);

        GUILayout.Space(10);

        selectedPieces[_position] = EditorGUILayout.Toggle("Select", selectedPieces[_position]);

        GUILayout.Space(5);

        if (GUILayout.Button("Go to Piece"))
            EditorGUIUtility.PingObject(availablePieces[_position]);
        if (GUILayout.Button("Go to Prefab"))
            EditorGUIUtility.PingObject(availablePieces[_position].GOprefab);

        GUILayout.EndVertical();

        GUILayout.Box(AssetPreview.GetAssetPreview(availablePieces[_position].GOprefab), GUILayout.Height(100), GUILayout.Width(100));

        GUILayout.EndHorizontal();
    }

    private void AddSelectedPiecesToCollection()
    {
        if (numberOfSelectedPieces == 0)
            return;

        List<PieceInfo> piecesToAdd = new List<PieceInfo>();
        for (int i = 0; i < selectedPieces.Length; i++)
            if (selectedPieces[i])
                piecesToAdd.Add(availablePieces[i]);

        tileCollectionEditor.AddPieces(piecesToAdd);

        UpdateAvailablePieces();
    }

    private void UpdateAvailablePieces()
    {
        availablePieces = new List<PieceInfo>();

        for (int i = 0; i < allPieces.Length; i++)
        {
            if (!tileCollection.piecesOnCollection.Contains<PieceInfo>(allPieces[i]))
                availablePieces.Add(allPieces[i]);
        }

        selectedPieces = new bool[availablePieces.Count];
    }

    private void CancelSelection()
    {
        for (int i = 0; i < selectedPieces.Length; i++)
            selectedPieces[i] = false;
    }
}