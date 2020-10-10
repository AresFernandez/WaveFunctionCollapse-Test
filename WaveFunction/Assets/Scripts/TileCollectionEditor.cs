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
        EditorGUILayout.LabelField("Pieces   (" + tileCollection.piecesOnCollection.Length.ToString() + ")", bold);

        EditorGUILayout.BeginVertical("box");
        for(int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            DrawPiece(i, bold);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("+"))
            AddPiecesButton();
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
        TileCollectionEditorWindow.Open(tileCollection);
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
}


public class TileCollectionEditorWindow : EditorWindow
{
    TileCollection tileCollection;
    PieceInfo[] allPieces;
    List<PieceInfo> availablePieces;
    bool[] selectedPieces;

    public static void Open(TileCollection _tileCollection)
    {
        TileCollectionEditorWindow window = GetWindow<TileCollectionEditorWindow>("Add new pieces");
        window.tileCollection = _tileCollection;

        window.allPieces = Resources.FindObjectsOfTypeAll(typeof(PieceInfo)) as PieceInfo[];
        window.availablePieces = new List<PieceInfo>();

        for (int i = 0; i < window.allPieces.Length; i++)
        {
            if (!window.tileCollection.piecesOnCollection.Contains<PieceInfo>(window.allPieces[i]))
                window.availablePieces.Add(window.allPieces[i]);
        }

        window.selectedPieces = new bool[window.availablePieces.Count];
        window.minSize = new Vector2(200, 200);
    }

    private void OnGUI()
    {
        int width = 200;

        GUILayout.BeginVertical();
        int currentPiece = 0;
        while (currentPiece < availablePieces.Count)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < Screen.width / width; i++)
            {
                if(currentPiece < availablePieces.Count)
                {
                    DrawPiece(availablePieces[currentPiece], width);
                    currentPiece++;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private void DrawPiece(PieceInfo _piece, int _width)
    {
        GUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.MaxHeight(_width * 0.4f), GUILayout.MaxWidth(_width));
        GUILayout.BeginVertical();
        GUILayout.Label("Piece");
        GUILayout.EndVertical();
        GUILayout.Button("2", GUILayout.ExpandWidth(false), GUILayout.MaxHeight(_width * 0.4f), GUILayout.MaxWidth(_width * 0.4f));
        GUILayout.EndHorizontal();
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
}