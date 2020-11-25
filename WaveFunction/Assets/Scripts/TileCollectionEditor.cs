using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[CustomEditor(typeof(TileCollection))]
public class TileCollectionEditor : Editor
{
    TileCollection tileCollection;
    bool[] selectedPieces;
    //Editor[] pieceEditors; 
    Vector2 scrollPos;
    int numberOfSelectedPieces;

    GUIStyle bgColor;
    PieceDatabase pieceDatabase;

    void OnEnable()
    {
        tileCollection = (TileCollection)target;

        UpdateSelectedPieces();

        bgColor = new GUIStyle();
    }

    private void UpdateSelectedPieces()
    {
        selectedPieces = new bool[tileCollection.piecesOnCollection.Length];
        for (int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            selectedPieces[i] = false;

        numberOfSelectedPieces = 0;

        //pieceEditors = new Editor[tileCollection.piecesOnCollection.Length];
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        return;
        
        GUIStyle header = new GUIStyle("BoldLabel")
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter
        };
        GUIStyle bold = new GUIStyle("BoldLabel") { fontSize = 15 };

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tile Collection", header);

        EditorGUILayout.Space(20);

        pieceDatabase = EditorGUILayout.ObjectField("Piece Database", pieceDatabase, typeof(PieceDatabase), true) as PieceDatabase;

        if (!pieceDatabase)
            return;

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

        if(selectedPieces.Length > 0)
        {
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Select All"))
                SelectAll(true);
            if (GUILayout.Button("Cancel Selection"))
                SelectAll(false);
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawPiece(int _position, GUIStyle _boldStyle)
    {
        /*
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(tileCollection.piecesOnCollection[_position].piece.piecePrefab.name, _boldStyle);

        EditorGUILayout.Space(10);

        selectedPieces[_position] = EditorGUILayout.Toggle("Select", selectedPieces[_position]);

        EditorGUILayout.Space(5);

        if(GUILayout.Button("Go to Piece"))
            EditorGUIUtility.PingObject(tileCollection.piecesOnCollection[_position].piece);
        if (GUILayout.Button("Go to Prefab"))
            EditorGUIUtility.PingObject(tileCollection.piecesOnCollection[_position].piece.piecePrefab);

        EditorGUILayout.EndVertical();

        //if (pieceEditors[_position] == null)
        //    pieceEditors[_position] = Editor.CreateEditor(tileCollection.piecesOnCollection[_position].piece.piecePrefab);
        //
        //pieceEditors[_position].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 100), new GUIStyle());

        EditorGUILayout.EndHorizontal();
        */
    }

    private void AddPiecesButton()
    {
        TileCollectionEditorWindow.Open(tileCollection, this, pieceDatabase);
    }

    private void RemovePiecesButton()
    {
        if (AnyPieceSelected())
        {
            string piecesSelectedNames = "";

            /*for (int i = 0; i < selectedPieces.Length; i++)
                if (selectedPieces[i])
                    piecesSelectedNames += "Piece: " + tileCollection.piecesOnCollection[i].piece.name + "\n";*/

            if (EditorUtility.DisplayDialog("Remove all this pieces?", piecesSelectedNames, "Yes", "Cancel"))
                RemoveSelectedPieces();
        }
    }

    private void RemoveSelectedPieces()
    {
        /*
        List<PieceInCollection> piecesToKeep = new List<PieceInCollection>();
        for(int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
        {
            if (!selectedPieces[i])
                piecesToKeep.Add(tileCollection.piecesOnCollection[i]);
        }

        tileCollection.piecesOnCollection = new PieceInCollection[piecesToKeep.Count];
        for (int i = 0; i < piecesToKeep.Count; i++)
            tileCollection.piecesOnCollection[i] = piecesToKeep[i];

        UpdateSelectedPieces();

        EditorUtility.SetDirty(tileCollection);
        */
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
        /*
        PieceInCollection[] finalPieces = new PieceInCollection[tileCollection.piecesOnCollection.Length + _piecesToAdd.Count];

        for (int i = 0; i < tileCollection.piecesOnCollection.Length; i++)
            finalPieces[i] = tileCollection.piecesOnCollection[i];

        for (int i = 0; i < _piecesToAdd.Count; i++)
            finalPieces[i + tileCollection.piecesOnCollection.Length] = new PieceInCollection(_piecesToAdd[i]);

        tileCollection.piecesOnCollection = finalPieces;

        UpdateSelectedPieces();

        EditorUtility.SetDirty(tileCollection);
        */
    }

    private void SelectAll(bool _value)
    {
        for (int i = 0; i < selectedPieces.Length; i++)
            selectedPieces[i] = _value;
    }
}


public class TileCollectionEditorWindow : EditorWindow
{
    TileCollection tileCollection;
    TileCollectionEditor tileCollectionEditor;

    PieceInfo[] allPieces;
    List<PieceInfo> availablePieces;
    bool[] selectedPieces;
    //Editor[] pieceEditors;
    int numberOfSelectedPieces;

    Vector2 scrollPos;
    int width;
    PieceDatabase pieceDatabase;

    public static void Open(TileCollection _tileCollection, TileCollectionEditor _tileCollectionEditor, PieceDatabase _pieceDatabase)
    {
        TileCollectionEditorWindow window = GetWindow<TileCollectionEditorWindow>("Add new pieces");
        window.tileCollection = _tileCollection;
        window.tileCollectionEditor = _tileCollectionEditor;

        window.pieceDatabase = _pieceDatabase;
        window.allPieces = _pieceDatabase.allPieces;
        window.availablePieces = new List<PieceInfo>();

        for (int i = 0; i < window.allPieces.Length; i++)
        {
            bool contains = false;
            for(int j = 0; j < window.tileCollection.piecesOnCollection.Length; j++)
            {
                /*
                if(window.allPieces[i] == window.tileCollection.piecesOnCollection[j].piece)
                {
                    contains = true;
                    break;
                }
                */
            }

            if(!contains)
                window.availablePieces.Add(window.allPieces[i]);
        }

        window.selectedPieces = new bool[window.availablePieces.Count];
        //window.pieceEditors = new Editor[window.availablePieces.Count];
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

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select all"))
            SelectAll();
        if (GUILayout.Button("Cancel Selection"))
            CancelSelection();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Add Selected Pieces"))
            AddSelectedPiecesToCollection();
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
            EditorGUIUtility.PingObject(availablePieces[_position].piecePrefab);

        GUILayout.EndVertical();

        //if (pieceEditors[_position] == null)
        //    pieceEditors[_position] = Editor.CreateEditor(availablePieces[_position].piecePrefab);
        //pieceEditors[_position].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 100), new GUIStyle());

        //GUILayout.Box(AssetPreview.GetAssetPreview(availablePieces[_position].piecePrefab), GUILayout.Height(100), GUILayout.Width(100));

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

        this.Close();
    }

    private void UpdateAvailablePieces()
    {
        availablePieces = new List<PieceInfo>();

        for (int i = 0; i < allPieces.Length; i++)
        {
            bool contains = false;
            for(int j = 0; j < tileCollection.piecesOnCollection.Length; j++)
            {
                /*
                if(allPieces[i] == tileCollection.piecesOnCollection[j].piece)
                {
                    contains = true;
                    break;
                }
                */
            }

            if (!contains)
                availablePieces.Add(allPieces[i]);
        }

        selectedPieces = new bool[availablePieces.Count];
        //pieceEditors = new Editor[availablePieces.Count];
    }

    private void SelectAll()
    {
        for (int i = 0; i < selectedPieces.Length; i++)
            selectedPieces[i] = true;
    }

    private void CancelSelection()
    {
        for (int i = 0; i < selectedPieces.Length; i++)
            selectedPieces[i] = false;
    }
}