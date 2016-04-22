using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (TileMapGenerator))]
public class TileMapGeneratorEditor : Editor {
    public override void OnInspectorGUI()
    {
        


        TileMapGenerator map = target as TileMapGenerator;
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        if(GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
