using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProvinceExtractor
{
    [MenuItem("Tools/Generate Provinces")]
    static void Generate()
    {
        //GameObject map = Selection.activeObject as GameObject;
        //SpriteRenderer sprite = map.GetComponent<SpriteRenderer>();
        Texture2D map = Selection.activeObject as Texture2D;

        HashSet<Color32> colors = new HashSet<Color32>();
        colors.Clear();

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                colors.Add(map.GetPixel(x, y));
            }
        }

        Debug.Log("Found provinces: " + colors.Count);

        foreach (var c in colors)
        {
            Debug.Log(c);
        }
    }
}
