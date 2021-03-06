using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    private MapManager target;
    private GameObject buttonPrefab;
    private int totalSpot = 11;
    private List<int> topOffsetList = new List<int>(){6, 4, 3, 2, 1, 1, 0, 0, 0, 0, 1};
    private List<int> bottomOffsetList = new List<int>(){1, 0, 0, 0, 0, 1, 1, 2, 3, 4, 6};
	private float buttonMargin;

    private Vector2 scrollPosition;

    [MenuItem("Map/Map Editor")]
    static void Init()
    {
        MapEditor map = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
        map.titleContent = new GUIContent("Map Editor");
        map.Show();
    }

    private void OnGUI()
    {
        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;

            target = EditorGUILayout.ObjectField("Target", target, typeof(MapManager), true) as MapManager;
            buttonPrefab = EditorGUILayout.ObjectField("buttonPrefab", buttonPrefab, typeof(GameObject), true) as GameObject;
            buttonMargin = EditorGUILayout.FloatField("Button Margin", buttonMargin);

            if (GUILayout.Button("Build"))
                Build();
        }
    }

    private void Build()
    {
        target.spotTable.Clear();
        for (int colIndex = 0; colIndex < totalSpot; ++colIndex)
        {
            var go = new GameObject("Anchor " + (char)((char)'A' + colIndex));
            go.transform.SetParent(target.transform);

            int bottomOffset = bottomOffsetList[colIndex];
            int topOffset = topOffsetList[colIndex];
            SerializableGameObjectList buttonList = new SerializableGameObjectList();
            for (int rowIndex = bottomOffset; rowIndex < totalSpot - topOffset; ++rowIndex)
            {
                var button = Instantiate(buttonPrefab) as GameObject;
                button.name = "Button (" + (char)((char)'A' + colIndex) + ", " + (rowIndex + 1) + ")";
                button.transform.SetParent(go.transform, false);
                button.transform.localPosition = CalcButtonPosition(colIndex, rowIndex);
                button.GetComponent<ButtonCell>().x = colIndex;
                button.GetComponent<ButtonCell>().y = rowIndex;
                buttonList.rawData.Add(button);
            }
           target.spotTable.Add(buttonList);
        }
    }

    private Vector3 CalcButtonPosition(int colIndex, int rowIndex)
    {
        float colOffset = buttonMargin * ((totalSpot / 2) - colIndex) * Mathf.Sqrt(3f) * 0.5f;
        float rowOffset = (buttonMargin * ((totalSpot / 2) - rowIndex)) - (buttonMargin * ((totalSpot / 2) - colIndex) * 0.5f);
        return new Vector3(rowOffset, 0, colOffset);
    }
}