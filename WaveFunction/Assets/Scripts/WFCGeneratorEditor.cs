using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//[CustomEditor(typeof(WFCGenerator), true)]
public class WFCGeneratorEditor : Editor
{
    WFCGenerator wfcGenerator;
    bool spawnPiece;
    PieceInfo pieceToSpawn;

    int width, depth, height;



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        wfcGenerator = (WFCGenerator)target;

        EditorGUI.BeginChangeCheck();
        spawnPiece = EditorGUILayout.BeginFoldoutHeaderGroup(spawnPiece, "Spawn Piece");

        if(EditorGUI.EndChangeCheck() && spawnPiece)
        {
            wfcGenerator.transform.position = wfcGenerator.grid.transform.position;
        }

        depth = height = 1;

        if(spawnPiece)
        {
            pieceToSpawn = EditorGUILayout.ObjectField("Piece to Spawn", pieceToSpawn, typeof(PieceInfo), false) as PieceInfo;

            if(pieceToSpawn)
            {
                //width = EditorGUILayout.IntSlider(width, 1, wfcGenerator.grid.width);
                //wfcGenerator.piecesToSpawnPos = new Vector3[width * depth * height];
                //for (int i = 0; i < wfcGenerator.piecesToSpawnPos.Length; i++)
                //    wfcGenerator.piecesToSpawnPos[i] = wfcGenerator.transform.position + Vector3.right * i;
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
